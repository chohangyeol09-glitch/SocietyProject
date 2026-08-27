using System;
using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 바이크 충돌을 감지해서 속도 구간(tier)을 계산하고 이벤트를 발행하는 모듈.
    /// 실제 효과(파티클/사운드/넉백 등)는 이 이벤트를 구독하는 별도 모듈이 처리한다.
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

            ContactPoint contact = collision.GetContact(0);
            var evt = new BikeCollisionEvent(tier, impactSpeed, contact.point, contact.normal, collision);
            OnCollision?.Invoke(evt);
        }
    }
}
