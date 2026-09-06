using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 바이크 충돌을 감지해서 속도 구간(tier)을 계산하고 이벤트를 발행하는 모듈.
    /// 실제 효과(파티클/사운드/넉백 등)는 이 이벤트를 구독하는 별도 모듈이 처리한다.
    ///
    /// 접촉점이 여러 개면 "가장 정면으로 부딪힌" 접촉점을 대표로 고르되,
    /// 벽 접촉이 하나라도 있으면 벽을 우선한다. (착지와 벽 충돌이 동시에 일어나도 벽으로 판정)
    ///
    /// 주의: OnCollisionEnter 가 호출되려면 이 컴포넌트가 붙은 오브젝트에
    ///       (트리거가 아닌) Collider 와 Rigidbody 가 있어야 한다. 바이크 루트에 함께 두는 것을 권장.
    /// </summary>
    public class BikeCollisionModule : Module
    {
        [Header("속도 구간 임계값 (m/s, 상대 충돌 속도 기준)")]
        [Tooltip("이 값 미만의 충돌은 무시한다 (효과 없음)")]
        [SerializeField] private float lowThreshold = 3f;
        [Tooltip("이 값 이상이면 Medium 단계")]
        [SerializeField] private float mediumThreshold = 8f;
        [Tooltip("이 값 이상이면 High 단계")]
        [SerializeField] private float highThreshold = 15f;

        [Header("연속 충돌 방지")]
        [Tooltip("한 번 충돌 이벤트가 발생한 뒤 이 시간 동안은 다시 발생하지 않는다 (초)")]
        [SerializeField] private float cooldown = 0.25f;

        [Header("대상 필터 (선택)")]
        [Tooltip("특정 레이어와의 충돌만 감지한다. Nothing 으로 두면 모든 충돌 감지.")]
        [SerializeField] private LayerMask collidesWith = ~0;

        [Header("바닥 판정 (점프 착지 구분용)")]
        [Tooltip("충돌 면이 수평에 가까우면 '바닥'으로 본다. 0.5 ≈ 60도, 0.7 ≈ 45도 이하 경사까지 바닥 취급")]
        [Range(0f, 1f)]
        [SerializeField] private float groundNormalDot = 0.5f;
        [Tooltip("이 레이어와의 충돌은 각도와 상관없이 항상 바닥으로 본다. Nothing 이면 각도로만 판정.")]
        [SerializeField] private LayerMask alwaysGroundLayers = 0;

        /// <summary>충돌이 감지될 때(Low 이상) 발행된다. 각 효과 모듈이 구독한다.</summary>
        public event Action<BikeCollisionEvent> OnCollision;

        private float _lastCollisionTime = -999f;

        /// <summary>속도값으로부터 충돌 단계를 계산한다.</summary>
        public CollisionTier GetTier(float impactSpeed)
        {
            if (impactSpeed >= highThreshold) return CollisionTier.High;
            if (impactSpeed >= mediumThreshold) return CollisionTier.Medium;
            if (impactSpeed >= lowThreshold) return CollisionTier.Low;
            return CollisionTier.None;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 레이어 필터 (Nothing = 0 이면 모든 레이어 허용)
            if (collidesWith.value != 0 && (collidesWith.value & (1 << collision.gameObject.layer)) == 0)
                return;

            // 쿨다운
            if (Time.time - _lastCollisionTime < cooldown)
                return;

            float impactSpeed = collision.relativeVelocity.magnitude;
            CollisionTier tier = GetTier(impactSpeed);
            if (tier == CollisionTier.None)
                return;

            _lastCollisionTime = Time.time;

            bool forcedGround = alwaysGroundLayers.value != 0
                                && (alwaysGroundLayers.value & (1 << collision.gameObject.layer)) != 0;

            PickContact(collision, forcedGround, out Vector3 point, out Vector3 normal, out float normalSpeed, out bool isGround);

            var evt = new BikeCollisionEvent(tier, impactSpeed, point, normal, collision, normalSpeed, isGround);
            OnCollision?.Invoke(evt);
        }

        /// <summary>
        /// 대표 접촉점을 고른다. 벽(비-바닥) 접촉이 있으면 그 중 가장 정면인 것을,
        /// 전부 바닥이면 그 중 가장 정면인 것을 고른다.
        /// </summary>
        private void PickContact(Collision collision, bool forcedGround,
            out Vector3 point, out Vector3 normal, out float normalSpeed, out bool isGround)
        {
            Vector3 relativeVelocity = collision.relativeVelocity;

            point = collision.transform.position;
            normal = Vector3.up;
            normalSpeed = 0f;
            isGround = true;

            float bestWallSpeed = float.NegativeInfinity;
            float bestGroundSpeed = float.NegativeInfinity;
            bool foundWall = false;

            int count = collision.contactCount;
            for (int i = 0; i < count; i++)
            {
                ContactPoint contact = collision.GetContact(i);

                // 법선 부호는 상황에 따라 뒤집혀 들어올 수 있으므로
                // "충돌에서 멀어지는 방향"(relativeVelocity 쪽)으로 정렬해둔다.
                Vector3 n = contact.normal;
                float approach = Vector3.Dot(relativeVelocity, n);
                if (approach < 0f)
                {
                    n = -n;
                    approach = -approach;
                }

                // 수평에 가까운 면 = 밟고 서는 면. 부호와 무관하게 판정하려고 절댓값을 쓴다.
                bool contactIsGround = forcedGround || Mathf.Abs(Vector3.Dot(contact.normal, Vector3.up)) >= groundNormalDot;

                if (!contactIsGround)
                {
                    if (!foundWall || approach > bestWallSpeed)
                    {
                        foundWall = true;
                        bestWallSpeed = approach;
                        point = contact.point;
                        normal = n;
                        normalSpeed = approach;
                        isGround = false;
                    }
                }
                else if (!foundWall && approach > bestGroundSpeed)
                {
                    bestGroundSpeed = approach;
                    point = contact.point;
                    normal = n;
                    normalSpeed = approach;
                    isGround = true;
                }
            }
        }
    }
}
