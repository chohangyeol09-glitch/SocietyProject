using System;
using UnityEngine;

namespace CHG.Scripts.DeliverySystem
{
    
    public class DeliveryDestination : MonoBehaviour
    {
        [SerializeField] private GameObject[] highlightObjs;
        
        public string DestinationID;
        public string DisplayName => DestinationID;
        public event Action OnClear;
        public event Action OnFail;

        private bool _isActive;

        public bool IsActive => _isActive;
        
        
        private void Awake()
        {
            SetHighlight(false);
        }

        public void Active()
        {
            _isActive = true;
            SetHighlight(true);
        }

        public void Deactive()
        {
            _isActive = false;
            SetHighlight(false);
        }

        public void Fail()
        {
            if (!_isActive)
                return;

            Deactive();
            OnFail?.Invoke();
        }

        private void SetHighlight(bool value)
        {
            foreach (GameObject obj in highlightObjs)
                obj.SetActive(value);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (!_isActive)
                return;
            
            if (collision.gameObject.CompareTag("Player"))
            {
                Deactive();
                OnClear?.Invoke();
            }
        }
    }
}
