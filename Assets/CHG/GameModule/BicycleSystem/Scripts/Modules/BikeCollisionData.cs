using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 충돌 세기를 속도에 따라 구분하는 단계.
    /// None 은 이벤트를 발생시키지 않는(무시하는) 아주 약한 접촉을 의미한다.
    /// </summary>
    public enum CollisionTier
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
    }

    /// <summary>
    /// 충돌이 발생했을 때 각 효과 모듈로 전달되는 정보 묶음.
    /// 새 효과 모듈은 이 데이터만 보고 반응하면 되므로 확장이 쉽다.
    /// </summary>
    public readonly struct BikeCollisionEvent
    {
        /// <summary>속도 구간(Low/Medium/High).</summary>
        public readonly CollisionTier Tier;

        /// <summary>충돌 순간의 상대 속도 크기(m/s). tier 계산에 쓰인 값.</summary>
        public readonly float ImpactSpeed;

        /// <summary>
        /// 충돌 면을 정면으로 파고든 속도 성분(m/s). 스치는 충돌은 작고, 정면 충돌은 ImpactSpeed 에 가깝다.
        /// 넉백 세기 판단에는 이 값이 더 정확하다.
        /// </summary>
        public readonly float NormalSpeed;

        /// <summary>충돌 지점(월드 좌표). 파티클/사운드 스폰 위치로 사용.</summary>
        public readonly Vector3 Point;

        /// <summary>충돌 면의 법선(월드). 바이크에서 멀어지는(밀려나는) 방향으로 정렬되어 있다.</summary>
        public readonly Vector3 Normal;

        /// <summary>
        /// 밟고 서는 면(바닥/경사면)과의 충돌인지 여부. 점프 착지가 여기에 해당한다.
        /// 넉백처럼 "벽에 부딪혔을 때만" 반응해야 하는 모듈은 이 값이 true 면 무시하면 된다.
        /// </summary>
        public readonly bool IsGround;

        /// <summary>원본 Collision. 상대 오브젝트 태그 등 추가 정보가 필요할 때 사용(널일 수 있음).</summary>
        public readonly Collision Collision;

        public BikeCollisionEvent(CollisionTier tier, float impactSpeed, Vector3 point, Vector3 normal, Collision collision,
            float normalSpeed = 0f, bool isGround = false)
        {
            Tier = tier;
            ImpactSpeed = impactSpeed;
            NormalSpeed = normalSpeed;
            Point = point;
            Normal = normal;
            IsGround = isGround;
            Collision = collision;
        }
    }
}
