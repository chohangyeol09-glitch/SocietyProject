using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 충돌 이벤트를 구독해서 속도 구간(tier)별로 다른 파티클 프리팹을 충돌 지점에 생성한다.
    /// </summary>
    public class CollisionParticleModule : Module, IAfterInitModule
    {
        [Header("Tier별 파티클 프리팹 (비워두면 해당 단계는 스킵)")]
        [Tooltip("약한 충돌 (먼지 등)")]
        [SerializeField] private ParticleSystem lowPrefab;
        [Tooltip("중간 충돌 (스파크 등)")]
        [SerializeField] private ParticleSystem mediumPrefab;
        [Tooltip("강한 충돌 (폭발/파편 등)")]
        [SerializeField] private ParticleSystem highPrefab;

        [Tooltip("생성된 파티클을 이 시간 뒤 자동 파괴 (초). 0 이하면 파괴하지 않음")]
        [SerializeField] private float autoDestroyDelay = 3f;

        public void AfterInit()
        {
            BikeCollisionModule collision = _owner.GetModule<BikeCollisionModule>();
            if (collision != null)
                collision.OnCollision += HandleCollision;
            else
                Debug.LogWarning($"{nameof(CollisionParticleModule)}: BikeCollisionModule 을 찾지 못했습니다.");
        }

        private void OnDestroy()
        {
            BikeCollisionModule collision = _owner != null ? _owner.GetModule<BikeCollisionModule>() : null;
            if (collision != null)
                collision.OnCollision -= HandleCollision;
        }

        private void HandleCollision(BikeCollisionEvent evt)
        {
            ParticleSystem prefab = GetPrefab(evt.Tier);
            if (prefab == null)
                return;

            // 충돌 면의 법선 방향을 바라보도록 회전시켜 생성
            Quaternion rotation = Quaternion.LookRotation(evt.Normal);
            ParticleSystem instance = Instantiate(prefab, evt.Point, rotation);
            instance.Play();

            if (autoDestroyDelay > 0f)
                Destroy(instance.gameObject, autoDestroyDelay);
        }

        private ParticleSystem GetPrefab(CollisionTier tier)
        {
            switch (tier)
            {
                case CollisionTier.Low: return lowPrefab;
                case CollisionTier.Medium: return mediumPrefab;
                case CollisionTier.High: return highPrefab;
                default: return null;
            }
        }
    }
}
