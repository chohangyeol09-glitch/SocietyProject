using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CHG.Scripts.DeliverySystem
{
    public class DeliveryManager : MonoBehaviour
    {
        public Dictionary<string, DeliveryDestination> Quests { get; private set; }
        [SerializeField] private List<DeliveryDestination> destinations;

        private DeliveryDestination _currentDestination;

        public event Action<DeliveryDestination> OnDestinationChanged;
        
        private void Awake()
        {
            Quests = destinations.ToDictionary(k => k.DestinationID, v => v);
        }

        public void ActiveQuest(string questName)
        {
            if (Quests.TryGetValue(questName, out var quest))
            {
                _currentDestination = quest;
                _currentDestination.Active(180f);
                _currentDestination.OnClear += HandleDeliveryClear;
                OnDestinationChanged?.Invoke(_currentDestination);
                
            }
        }

        private void HandleDeliveryClear()
        {
            _currentDestination.OnClear -= HandleDeliveryClear;
            _currentDestination = null;
            OnDestinationChanged?.Invoke(_currentDestination);
        }
        
        
        #if UNITY_EDITOR
        [ContextMenu("TestQuest")]
        private void TestQuest()
        {
            ActiveQuest("TestQuest");
        }
        #endif
        
    }
}