using DevLib.ModuleSystem;
using rayzngames;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 가속(부스트) 모듈. 부스트 키를 누르고 있는 동안 진행 방향으로 추가 가속을 준다.
    /// 힘은 질량과 무관한 ForceMode.Acceleration 라서 boostAcceleration = 추가 가속도(m/s^2) 로 직관적으로 튜닝된다.
    /// </summary>
    public class BikeBoostModule : Module
    {
        [Tooltip("부스트 입력 키 (신규 Input System 으로 옮길 땐 SetBoosting() 을 호출)")]
        [SerializeField] private KeyCode boostKey = KeyCode.LeftShift;
        [Tooltip("부스트 중 추가 가속도 (m/s^2)")]
        [SerializeField] private float boostAcceleration = 15f;
        [Tooltip("이 속도(m/s) 이상에서는 부스트가 더 붙지 않는다")]
        [SerializeField] private float maxBoostSpeed = 30f;
        [Tooltip("지면에 있을 때만 부스트 허용")]
        [SerializeField] private bool requireGround = true;

        private Rigidbody _body;
        private BicycleVehicle _bike;

        /// <summary>현재 부스트 중인지 여부.</summary>
        public bool Boosting { get; private set; }

        private void Awake()
        {
            _body = GetComponentInParent<Rigidbody>();
            _bike = GetComponentInParent<BicycleVehicle>();
        }

        private void Update()
        {
            Boosting = Input.GetKey(boostKey);
        }

        /// <summary>외부(신규 Input System 등)에서 부스트 상태를 제어할 때 사용.</summary>
        public void SetBoosting(bool on)
        {
            Boosting = on;
        }

        private void FixedUpdate()
        {
            if (!Boosting || _body == null || _bike == null)
                return;
            if (requireGround && !_bike.OnGround())
                return;
            if (_body.linearVelocity.magnitude >= maxBoostSpeed)
                return;

            // 바이크가 바라보는 방향으로 가속. 수평 성분만 사용해 위로 뜨지 않게 한다.
            Vector3 forward = _bike.transform.forward;
            forward.y = 0f;
            _body.AddForce(forward.normalized * boostAcceleration, ForceMode.Acceleration);
        }
    }
}
