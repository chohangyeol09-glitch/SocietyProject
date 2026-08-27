using DevLib.ModuleSystem;
using rayzngames;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 점프 모듈. 지면에 있을 때 점프 키를 누르면 위로 튀어오른다.
    /// 힘은 질량과 무관한 ForceMode.VelocityChange 라서 jumpForce = 위쪽 초기 속도(m/s) 로 직관적으로 튜닝된다.
    /// </summary>
    public class BikeJumpModule : Module
    {
        [Tooltip("점프 입력 키 (신규 Input System 으로 옮길 땐 TryJump() 를 호출)")]
        [SerializeField] private KeyCode jumpKey = KeyCode.LeftControl;
        [Tooltip("점프 시 위쪽 초기 속도 (m/s)")]
        [SerializeField] private float jumpForce = 6f;
        [Tooltip("연속 점프 방지 간격 (초)")]
        [SerializeField] private float cooldown = 0.3f;

        private Rigidbody _body;
        private BicycleVehicle _bike;
        private float _lastJumpTime = -999f;

        private void Awake()
        {
            // 소유자(BikeModuleOwner) 유무와 상관없이 동작하도록 부모에서 직접 찾는다.
            _body = GetComponentInParent<Rigidbody>();
            _bike = GetComponentInParent<BicycleVehicle>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(jumpKey))
                TryJump();
        }

        /// <summary>점프를 시도한다. 지면에 있고 쿨다운이 지났을 때만 실행된다.</summary>
        public void TryJump()
        {
            if (_body == null || _bike == null)
                return;
            if (Time.time - _lastJumpTime < cooldown)
                return;
            if (!_bike.OnGround()) // 공중 중복 점프 방지
                return;

            _lastJumpTime = Time.time;
            _body.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }
}
