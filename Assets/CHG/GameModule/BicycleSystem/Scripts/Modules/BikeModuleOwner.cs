using DevLib.ModuleSystem;
using rayzngames;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 바이크 루트에 부착하는 모듈 소유자.
    /// 자식/자기 자신에 붙은 Module 들을 모아 초기화하고, 공용 참조(BicycleVehicle, Rigidbody)를 제공한다.
    /// </summary>
    [RequireComponent(typeof(BicycleVehicle))]
    [RequireComponent(typeof(Rigidbody))]
    public class BikeModuleOwner : ModuleOwner
    {
        /// <summary>물리/속도 정보를 읽기 위한 바이크 본체 참조.</summary>
        public BicycleVehicle Bike { get; private set; }

        /// <summary>넉백 등 물리력을 가할 때 쓰는 Rigidbody 참조.</summary>
        public Rigidbody Body { get; private set; }

        protected override void Awake()
        {
            // 모듈들의 Initialize 안에서 Bike/Body 를 참조할 수 있도록 base.Awake() 전에 먼저 캐싱한다.
            Bike = GetComponent<BicycleVehicle>();
            Body = GetComponent<Rigidbody>();
            base.Awake();
        }
    }
}
