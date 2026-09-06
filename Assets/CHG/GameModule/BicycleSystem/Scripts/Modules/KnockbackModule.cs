using System;
using DevLib.ModuleSystem;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace CHG.Bike
{
    /// <summary>
    /// 강한 충돌 시 바이크를 부딪힌 반대 방향으로 튕겨 날리는 모듈.
    /// 충돌 세기가 일정 단계 이상이면 회전 제어를 풀고(BikeControlModule.LaunchIntoAir) 충격량을 가한다.
    /// 착지하면 BikeControlModule 이 자동으로 제어를 복구한다.
    ///
    /// 점프 후 착지처럼 "밟고 서는 면"과의 충돌은 아무리 세도 튕기지 않는다.
    /// (BikeCollisionModule 이 접촉 면 각도로 바닥/벽을 구분해준다)
    /// </summary>
    public class KnockbackModule : Module, IAfterInitModule
    {
        [Tooltip("이 단계 이상의 충돌에서만 넉백이 발생한다")]
        [SerializeField] private CollisionTier minTier = CollisionTier.High;

        [Header("바닥 제외")]
        [Tooltip("바닥/경사면과의 충돌(점프 착지 등)은 넉백하지 않는다. 특별한 이유가 없으면 켜둔다.")]
        [SerializeField] private bool ignoreGroundCollisions = true;
        [Tooltip("충돌 면을 정면으로 파고든 속도가 이 값 미만이면 무시한다. 벽을 스치는 접촉으로 튕기는 것을 막는다 (m/s)")]
        [SerializeField] private float minNormalSpeed = 4f;

        [Header("넉백 힘 (질량과 무관한 속도 단위)")]
        [Tooltip("충돌 면에서 밀려나는 기본 속도 (m/s)")]
        [SerializeField] private float knockbackForce = 8f;
        [Tooltip("위로 띄우는 속도 (m/s, 공중 연출)")]
        [SerializeField] private float upwardForce = 5f;
        [Tooltip("충돌 속도에 비례해 넉백을 키우는 계수")]
        [SerializeField] private float speedScale = 0.5f;
        [Tooltip("날아갈 때 주는 회전 속도 (rad/s)")]
        [SerializeField] private float spinTorque = 6f;

        [Header("이벤트")]
        [Tooltip("정면 충돌 속도가 이 값 이상이면 '강한 넉백'으로 보고 onStrongKnockback 을 실행한다 (m/s)")]
        [SerializeField] private float strongNormalSpeed = 12f;
        [Tooltip("넉백이 발생할 때마다 실행된다")]
        [SerializeField] private UnityEvent onKnockback;
        [Tooltip("strongNormalSpeed 이상의 강한 넉백일 때만 실행된다 (카메라 흔들림, 사운드, 연출 등)")]
        [SerializeField] private UnityEvent onStrongKnockback;

        /// <summary>넉백이 발생했을 때 충돌 정보와 함께 발행된다. 코드에서 구독할 때 사용.</summary>
        public event Action<BikeCollisionEvent> Knockback;

        /// <summary>강한 넉백(정면 충돌 속도 >= strongNormalSpeed)일 때만 발행된다.</summary>
        public event Action<BikeCollisionEvent> StrongKnockback;

        private Rigidbody _body;
        private BikeControlModule _control;

        public void AfterInit()
        {
            _body = (_owner as BikeModuleOwner)?.Body;
            if (_body == null)
                _body = _owner.GetComponent<Rigidbody>();

            _control = _owner.GetModule<BikeControlModule>();

            BikeCollisionModule collision = _owner.GetModule<BikeCollisionModule>();
            if (collision != null)
                collision.OnCollision += HandleCollision;
            else
                Debug.LogWarning($"{nameof(KnockbackModule)}: BikeCollisionModule 을 찾지 못했습니다.");
        }

        private void OnDestroy()
        {
            BikeCollisionModule collision = _owner != null ? _owner.GetModule<BikeCollisionModule>() : null;
            if (collision != null)
                collision.OnCollision -= HandleCollision;
        }

        private void HandleCollision(BikeCollisionEvent evt)
        {
            
            Debug.Log("Knockback");
            if (evt.Tier < minTier || _body == null)
                return;

            // 점프 착지 등 밟고 서는 면과의 충돌은 튕기지 않는다.
            if (ignoreGroundCollisions && evt.IsGround)
                return;

            // 벽을 스치듯 지나간 접촉은 무시한다. (정면으로 파고든 속도 성분만 본다)
            if (evt.NormalSpeed < minNormalSpeed)
                return;

            // 먼저 회전 제어를 풀어 자유롭게 날아가도록 한다.
            _control?.LaunchIntoAir();

            // 충돌 면에서 멀어지는 방향으로 밀어낸다. 벽에 부딪힌 연출이므로 수평 성분만 쓰고,
            // 뜨는 높이는 upwardForce 로 따로 준다. 충돌이 정면일수록 더 강하게.
            Vector3 push = evt.Normal;
            push.y = 0f;
            push = push.sqrMagnitude > 0.0001f ? push.normalized : evt.Normal.normalized;

            // 질량(모터바이크 180kg)에 상관없이 일정하도록 VelocityChange 사용.
            float scaledForce = knockbackForce + evt.NormalSpeed * speedScale;
            Vector3 velocityChange = push * scaledForce + Vector3.up * upwardForce;
            _body.AddForce(velocityChange, ForceMode.VelocityChange);

            // 살짝 회전을 줘서 뒹구는 느낌을 준다. (관성 텐서 무시)
            if (spinTorque > 0f)
                _body.AddTorque(Random.onUnitSphere * spinTorque, ForceMode.VelocityChange);

            // 연출/게임 로직은 이벤트로 넘긴다. 구독자 예외가 물리 처리를 막지 않도록 마지막에 실행.
            onKnockback?.Invoke();
            Knockback?.Invoke(evt);

            Debug.Log("Knockback");
            if (evt.NormalSpeed >= strongNormalSpeed)
            {
                onStrongKnockback?.Invoke();
                StrongKnockback?.Invoke(evt);
                Debug.Log("StrongKnockback");
            }
        }
    }
}
