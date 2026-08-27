using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 강한 충돌 시 바이크를 튕겨 날리는 모듈.
    /// 충돌 세기가 일정 단계 이상이면 회전 제어를 풀고(BikeControlModule.LaunchIntoAir) 충격량을 가한다.
    /// 착지하면 BikeControlModule 이 자동으로 제어를 복구한다.
    /// </summary>
    public class KnockbackModule : Module, IAfterInitModule
    {
        [Tooltip("이 단계 이상의 충돌에서만 넉백이 발생한다")]
        [SerializeField] private CollisionTier minTier = CollisionTier.High;

        [Header("넉백 힘 (질량과 무관한 속도 단위)")]
        [Tooltip("충돌 면에서 밀려나는 기본 속도 (m/s)")]
        [SerializeField] private float knockbackForce = 8f;
        [Tooltip("위로 띄우는 속도 (m/s, 공중 연출)")]
        [SerializeField] private float upwardForce = 5f;
        [Tooltip("충돌 속도에 비례해 넉백을 키우는 계수")]
        [SerializeField] private float speedScale = 0.5f;
        [Tooltip("날아갈 때 주는 회전 속도 (rad/s)")]
        [SerializeField] private float spinTorque = 6f;

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
            if (evt.Tier < minTier || _body == null)
                return;

            // 먼저 회전 제어를 풀어 자유롭게 날아가도록 한다.
            _control?.LaunchIntoAir();

            // 충돌 면 법선 방향 + 위쪽으로 밀어낸다. 충돌 속도가 클수록 더 강하게.
            // 질량(모터바이크 180kg)에 상관없이 일정하도록 VelocityChange 사용.
            float scaledForce = knockbackForce + evt.ImpactSpeed * speedScale;
            Vector3 velocityChange = evt.Normal * scaledForce + Vector3.up * upwardForce;
            _body.AddForce(velocityChange, ForceMode.VelocityChange);

            // 살짝 회전을 줘서 뒹구는 느낌을 준다. (관성 텐서 무시)
            if (spinTorque > 0f)
                _body.AddTorque(Random.onUnitSphere * spinTorque, ForceMode.VelocityChange);
        }
    }
}
