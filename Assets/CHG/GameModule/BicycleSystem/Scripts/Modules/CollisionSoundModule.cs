using DevLib.ModuleSystem;
using UnityEngine;

namespace CHG.Bike
{
    /// <summary>
    /// 충돌 이벤트를 구독해서 속도 구간(tier)별로 다른 충돌음을 재생한다.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class CollisionSoundModule : Module, IAfterInitModule
    {
        [Header("Tier별 충돌음 (비워두면 해당 단계는 무음)")]
        [SerializeField] private AudioClip lowClip;
        [SerializeField] private AudioClip mediumClip;
        [SerializeField] private AudioClip highClip;

        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;

        private AudioSource _audioSource;

        public void AfterInit()
        {
            _audioSource = GetComponent<AudioSource>();

            BikeCollisionModule collision = _owner.GetModule<BikeCollisionModule>();
            if (collision != null)
                collision.OnCollision += HandleCollision;
            else
                Debug.LogWarning($"{nameof(CollisionSoundModule)}: BikeCollisionModule 을 찾지 못했습니다.");
        }

        private void OnDestroy()
        {
            BikeCollisionModule collision = _owner != null ? _owner.GetModule<BikeCollisionModule>() : null;
            if (collision != null)
                collision.OnCollision -= HandleCollision;
        }

        private void HandleCollision(BikeCollisionEvent evt)
        {
            AudioClip clip = GetClip(evt.Tier);
            if (clip == null || _audioSource == null)
                return;

            // 충돌 지점에서 한 번만 재생 (겹쳐 재생되어도 잘리지 않도록 PlayOneShot 사용)
            _audioSource.PlayOneShot(clip, volume);
        }

        private AudioClip GetClip(CollisionTier tier)
        {
            switch (tier)
            {
                case CollisionTier.Low: return lowClip;
                case CollisionTier.Medium: return mediumClip;
                case CollisionTier.High: return highClip;
                default: return null;
            }
        }
    }
}
