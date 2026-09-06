using DevLib.ModuleSystem;
using rayzngames;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 대시(순간 가속) 모듈. 대시 키를 누르면 짧은 시간(dashDuration) 동안만 진행 방향으로 강하게 가속하고,
    /// 이후 dashCooldown 동안은 다시 사용할 수 없다.
    /// 힘은 질량과 무관한 ForceMode.Acceleration 라서 dashAcceleration = 추가 가속도(m/s^2) 로 직관적으로 튜닝된다.
    /// (대시로 얻는 속도 ≈ dashAcceleration × dashDuration)
    /// </summary>
    public class BikeBoostModule : Module
    {
        [Tooltip("대시 입력 키 (신규 Input System 으로 옮길 땐 TryDash() 를 호출)")]
        [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
        [Tooltip("대시 중 추가 가속도 (m/s^2)")]
        [SerializeField] private float dashAcceleration = 60f;
        [Tooltip("대시가 발동되는 시간 (초). 0.2~0.3 정도의 짧은 순간")]
        [SerializeField] private float dashDuration = 0.25f;
        [Tooltip("대시가 끝난 뒤 다시 쓸 수 있을 때까지의 쿨타임 (초, 대시 종료 기준)")]
        [SerializeField] private float dashCooldown = 1.5f;
        [Tooltip("이 속도(m/s) 이상에서는 대시 가속이 더 붙지 않는다")]
        [SerializeField] private float maxBoostSpeed = 40f;
        [Tooltip("지면에 있을 때만 대시 허용")]
        [SerializeField] private bool requireGround = true;

        private Rigidbody _body;
        private BicycleVehicle _bike;

        private float _dashStartTime = -999f;

        /// <summary>현재 대시가 발동 중인지 여부.</summary>
        public bool IsDashing => Time.time < _dashStartTime + dashDuration;

        /// <summary>남은 쿨타임 (초). 0 이면 사용 가능. 대시가 끝난 시점(_dashStartTime + dashDuration)부터 dashCooldown 만큼 잰다.</summary>
        public float CooldownRemaining => Mathf.Max(0f, (_dashStartTime + dashDuration + dashCooldown) - Time.time);

        private void Awake()
        {
            _body = GetComponentInParent<Rigidbody>();
            _bike = GetComponentInParent<BicycleVehicle>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(dashKey))
                TryDash();
        }

        /// <summary>대시를 시도한다. 쿨타임이 끝났고 (설정 시) 지면에 있을 때만 발동된다.</summary>
        public void TryDash()
        {
            if (_bike == null || _body == null)
                return;
            if (CooldownRemaining > 0f) // 아직 쿨타임 중
                return;
            if (requireGround && !_bike.OnGround())
                return;

            _dashStartTime = Time.time;
        }

        private void FixedUpdate()
        {
            if (!IsDashing || _body == null || _bike == null)
                return;
            if (_body.linearVelocity.magnitude >= maxBoostSpeed)
                return;

            // 바이크가 바라보는 방향으로 가속. 수평 성분만 사용해 위로 뜨지 않게 한다.
            Vector3 forward = _bike.transform.forward;
            forward.y = 0f;
            _body.AddForce(forward.normalized * dashAcceleration, ForceMode.Acceleration);
        }
    }
}
