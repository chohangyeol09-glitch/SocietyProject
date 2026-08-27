using System;
using UnityEngine;

namespace CHG.Scripts.DeliverySystem
{
    
    public class DeliveryDestination : MonoBehaviour
    {
        [SerializeField] private GameObject[] highlightObjs;
        
        public string DestinationID;
        public event Action OnClear;
        public event Action OnFail;

        private float _timeLimit;
        
        
        private void Awake()
        {
            foreach (GameObject obj in highlightObjs)
            {
                obj.SetActive(false);
                Debug.Log(obj.name);
            }
        }

        public void Active(float timeLimit)
        {
            _timeLimit = timeLimit;
            foreach (GameObject obj in highlightObjs)
                obj.SetActive(true);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                OnClear?.Invoke();
                foreach (GameObject obj in highlightObjs)
                    obj.SetActive(false);
            }
        }
    }
}
