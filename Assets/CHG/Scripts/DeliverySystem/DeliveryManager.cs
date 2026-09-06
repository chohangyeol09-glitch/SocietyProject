using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CHG.Scripts.DeliverySystem
{
    public enum DeliveryPhase
    {
        None,
        Pickup,
        Delivery
    }

    public enum DeliveryTimerStart
    {
        QuestStart,
        Pickup
    }

    public class DeliveryManager : MonoBehaviour
    {
        [SerializeField] private List<Restaurant> restaurants;
        [SerializeField] private List<DeliveryDestination> deliveryPoints;
        [SerializeField] private float defaultTimeLimit = 180f;
        [SerializeField] private DeliveryTimerStart timerStart = DeliveryTimerStart.QuestStart;

        private DeliveryQuest _currentQuest;
        private DeliveryDestination _currentDestination;
        private bool _isTimerRunning;

        public DeliveryQuest CurrentQuest => _currentQuest;
        public DeliveryDestination CurrentDestination => _currentDestination;
        public DeliveryPhase Phase { get; private set; } = DeliveryPhase.None;

        public float RemainingTime { get; private set; }
        public bool IsTimerRunning => _isTimerRunning;
        public float RemainingRatio => _currentQuest == null || _currentQuest.TimeLimit <= 0f
            ? 0f
            : Mathf.Clamp01(RemainingTime / _currentQuest.TimeLimit);

        public event Action<DeliveryQuest> OnQuestStart;
        public event Action<DeliveryDestination> OnDestinationChanged;
        public event Action<DeliveryQuest> OnFoodReceived;
        public event Action<DeliveryQuest> OnQuestClear;
        public event Action<DeliveryQuest, DeliveryPhase> OnQuestFail;
        public event Action<float> OnRemainingTimeChanged;

        public IReadOnlyList<Restaurant> Restaurants => restaurants;

        private void Update()
        {
            if (!_isTimerRunning)
                return;

            RemainingTime -= Time.deltaTime;

            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;
                OnRemainingTimeChanged?.Invoke(RemainingTime);
                FailQuest();
                return;
            }

            OnRemainingTimeChanged?.Invoke(RemainingTime);
        }

        public List<FoodDataSO> GetAllFoods()
        {
            return restaurants.SelectMany(r => r.Menu).Where(f => f != null).Distinct().ToList();
        }

        public List<DeliveryQuest> CreateRandomQuests(int count)
        {
            List<DeliveryQuest> result = new List<DeliveryQuest>();
            for (int i = 0; i < count; i++)
            {
                DeliveryQuest quest = CreateRandomQuest();
                if (quest != null)
                    result.Add(quest);
            }
            return result;
        }

        public DeliveryQuest CreateRandomQuest()
        {
            Restaurant restaurant = PickRandom(restaurants);
            if (restaurant == null)
                return null;

            return CreateQuest(restaurant.GetRandomFood(), restaurant);
        }

        public DeliveryQuest CreateQuest(string foodID)
        {
            List<Restaurant> sellers = restaurants.Where(r => r != null && r.HasFood(foodID)).ToList();
            Restaurant restaurant = PickRandom(sellers);
            if (restaurant == null)
                return null;

            return CreateQuest(restaurant.Menu.First(f => f != null && f.FoodID == foodID), restaurant);
        }

        public DeliveryQuest CreateQuest(FoodDataSO food)
        {
            List<Restaurant> sellers = restaurants.Where(r => r != null && r.HasFood(food)).ToList();
            return CreateQuest(food, PickRandom(sellers));
        }

        public DeliveryQuest CreateQuest(FoodDataSO food, Restaurant restaurant)
        {
            if (food == null || restaurant == null)
                return null;

            List<DeliveryDestination> candidates = deliveryPoints
                .Where(p => p != null && p != restaurant.Destination)
                .ToList();

            DeliveryDestination deliveryPoint = PickRandom(candidates);
            if (deliveryPoint == null)
                return null;

            float timeLimit = food.TimeLimit > 0f ? food.TimeLimit : defaultTimeLimit;
            return new DeliveryQuest(food, restaurant, deliveryPoint, timeLimit);
        }

        public bool ActiveQuest(DeliveryQuest quest)
        {
            if (quest == null)
                return false;

            if (Phase != DeliveryPhase.None)
            {
                Debug.LogWarning($"Delivery quest already running : {_currentQuest.QuestID}");
                return false;
            }

            _currentQuest = quest;
            Phase = DeliveryPhase.Pickup;
            RemainingTime = quest.TimeLimit;

            OnQuestStart?.Invoke(_currentQuest);
            SetDestination(quest.Restaurant.Destination);

            if (timerStart == DeliveryTimerStart.QuestStart)
                StartTimer();

            return true;
        }

        public bool ActiveRandomQuest() => ActiveQuest(CreateRandomQuest());

        public bool ActiveQuest(string foodID) => ActiveQuest(CreateQuest(foodID));

        public bool ActiveQuest(FoodDataSO food) => ActiveQuest(CreateQuest(food));

        public void FailQuest()
        {
            if (Phase == DeliveryPhase.None)
                return;

            DeliveryQuest failedQuest = _currentQuest;
            DeliveryPhase failedPhase = Phase;

            StopTimer();
            ClearDestination(true);

            Phase = DeliveryPhase.None;
            _currentQuest = null;

            OnDestinationChanged?.Invoke(null);
            OnQuestFail?.Invoke(failedQuest, failedPhase);
        }

        private void StartTimer()
        {
            _isTimerRunning = true;
            OnRemainingTimeChanged?.Invoke(RemainingTime);
        }

        private void StopTimer()
        {
            _isTimerRunning = false;
        }

        private void SetDestination(DeliveryDestination destination)
        {
            _currentDestination = destination;
            _currentDestination.OnClear += HandleDestinationClear;
            _currentDestination.Active();
            OnDestinationChanged?.Invoke(_currentDestination);
        }

        private void ClearDestination(bool fail)
        {
            if (_currentDestination == null)
                return;

            _currentDestination.OnClear -= HandleDestinationClear;

            if (fail)
                _currentDestination.Fail();

            _currentDestination = null;
        }

        private void HandleDestinationClear()
        {
            ClearDestination(false);

            if (Phase == DeliveryPhase.Pickup)
            {
                Phase = DeliveryPhase.Delivery;
                OnFoodReceived?.Invoke(_currentQuest);
                SetDestination(_currentQuest.DeliveryPoint);

                if (timerStart == DeliveryTimerStart.Pickup)
                    StartTimer();

                return;
            }

            DeliveryQuest clearedQuest = _currentQuest;

            StopTimer();
            Phase = DeliveryPhase.None;
            _currentQuest = null;

            OnDestinationChanged?.Invoke(null);
            OnQuestClear?.Invoke(clearedQuest);
        }

        private static T PickRandom<T>(IReadOnlyList<T> list) where T : class
        {
            return list == null || list.Count == 0 ? null : list[Random.Range(0, list.Count)];
        }


        #if UNITY_EDITOR
        [ContextMenu("TestRandomQuest")]
        private void TestRandomQuest()
        {
            DeliveryQuest quest = CreateRandomQuest();
            if (quest == null)
            {
                Debug.LogWarning("Failed to create quest : check restaurants / deliveryPoints / menu");
                return;
            }

            Debug.Log($"[{quest.QuestID}] {quest.OrderText}");
            ActiveQuest(quest);
        }

        [ContextMenu("TestFailQuest")]
        private void TestFailQuest()
        {
            FailQuest();
        }
        #endif

    }
}
