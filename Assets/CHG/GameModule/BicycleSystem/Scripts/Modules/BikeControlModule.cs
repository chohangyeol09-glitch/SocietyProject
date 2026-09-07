using DevLib.ModuleSystem;
using rayzngames;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 바이크의 조종 권한(InControl)과 Z축 회전 제어(ConstrainRotation)를 한 곳에서 관리한다.
    /// - 평상시: 지면에 있으면 회전을 고정해 똑바로 서게 하고, 플레이어 입력을 받는다.
    /// - LaunchIntoAir() 호출 시: 회전 고정을 풀고 입력을 끊어 자유롭게 날아가게 한다.
    ///   다시 지면에 착지하면 자동으로 회전 제어를 켜서 똑바로 세운다.
    ///
    /// 입력 드라이버(BikeControlsExample 등)는 직접 ConstrainRotation/InControl 을 호출하지 말고
    /// 이 모듈의 <see cref="PlayerWantsControl"/> 만 세팅한다. 그래야 넉백 연출과 충돌하지 않는다.
    /// </summary>
    public class BikeControlModule : Module
    {
        [Tooltip("플레이어가 바이크를 조종하려는 상태인지. 입력 드라이버가 세팅한다.")]
        [SerializeField] private bool playerWantsControl = true;

        [Tooltip("넉백 후 이 시간이 지나도 지면을 벗어나지 못하면 조종을 강제로 복구한다 (초). 조종 불능으로 갇히는 것을 막는 안전장치")]
        [SerializeField] private float maxLaunchTime = 2f;

        /// <summary>날아가는 중인지 여부. 이 동안에는 입력/회전 고정이 모두 해제된다.</summary>
        public bool IsLaunched { get; private set; }

        private bool _hasLeftGround;
        private float _launchStartTime;

        private BicycleVehicle _bike;

        /// <summary>플레이어 조종 의사. 입력 드라이버가 이 값을 세팅한다.</summary>
        public bool PlayerWantsControl
        {
            get => playerWantsControl;
            set => playerWantsControl = value;
        }

        private void Awake()
        {
            // BikeModuleOwner 가 없는 씬에서도 동작하도록 부모에서 직접 찾아둔다.
            // owner 가 있으면 Initialize 에서 같은 참조로 다시 채워진다.
            _bike = GetComponentInParent<BicycleVehicle>();
        }

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);

            BicycleVehicle fromOwner = (owner as BikeModuleOwner)?.Bike;
            if (fromOwner == null)
                fromOwner = owner.GetComponent<BicycleVehicle>();
            if (fromOwner != null)
                _bike = fromOwner;
        }

        /// <summary>
        /// 바이크를 공중으로 띄운다(넉백 연출용). 회전 고정과 입력을 해제하고,
        /// 한 번 지면을 벗어났다가 다시 착지하면 자동으로 제어를 복구한다.
        /// </summary>
        public void LaunchIntoAir()
        {
            IsLaunched = true;
            _hasLeftGround = false;
            _launchStartTime = Time.time;
        }

        /// <summary>강제로 제어를 즉시 복구한다(연출 취소 등).</summary>
        public void ResetControl()
        {
            IsLaunched = false;
            _hasLeftGround = false;
        }

        private void Update()
        {
            if (_bike == null)
                return;

            if (IsLaunched)
            {
                // 날아가는 중: 입력 차단 + 회전 자유
                _bike.InControl(false);
                _bike.ConstrainRotation(false);

                bool onGround = _bike.OnGround();
                if (!onGround)
                    _hasLeftGround = true;
                else if (_hasLeftGround)
                    // 지면을 벗어났다가 다시 착지 → 제어 복구
                    IsLaunched = false;
                else if (Time.time - _launchStartTime > maxLaunchTime)
                    // 지면을 아예 벗어나지 못한 약한 넉백 → 조종 불능으로 갇히지 않게 복구
                    IsLaunched = false;

                return;
            }

            // 평상시: 지면에 있을 때만 회전 고정(똑바로 서기), 공중이면 자유
            _bike.InControl(playerWantsControl);
            _bike.ConstrainRotation(playerWantsControl && _bike.OnGround());
        }
    }
}
