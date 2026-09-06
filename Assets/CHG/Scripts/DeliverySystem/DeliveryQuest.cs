using UnityEngine;

namespace CHG.Scripts.DeliverySystem
{
    public class DeliveryQuest
    {
        public const string FoodToken = "{food}";
        public const string RestaurantToken = "{restaurant}";
        public const string DestinationToken = "{destination}";
        public const string RewardToken = "{reward}";
        public const string TimeToken = "{time}";

        private const string DefaultOrderFormat = "{food}을(를) {destination}(으)로 배달";
        private const string PickupFormat = "{restaurant}에서 {food}을(를) 받기";
        private const string DeliveryFormat = "{food}을(를) {destination}(으)로 배달";

        private static int _idCounter;

        public string QuestID { get; }
        public FoodDataSO Food { get; }
        public Restaurant Restaurant { get; }
        public DeliveryDestination DeliveryPoint { get; }
        public float TimeLimit { get; }

        public DeliveryQuest(FoodDataSO food, Restaurant restaurant, DeliveryDestination deliveryPoint, float timeLimit)
        {
            QuestID = $"{food.FoodID}_{++_idCounter}";
            Food = food;
            Restaurant = restaurant;
            DeliveryPoint = deliveryPoint;
            TimeLimit = timeLimit;
        }

        public string FoodName => Food.DisplayName;

        public string OrderText => Format(string.IsNullOrEmpty(Food.OrderFormat) ? DefaultOrderFormat : Food.OrderFormat);

        public string PickupText => Format(PickupFormat);

        public string DeliveryText => Format(DeliveryFormat);

        public string Format(string format)
        {
            if (string.IsNullOrEmpty(format))
                return string.Empty;

            string text = format
                .Replace(FoodToken, FoodName)
                .Replace(RestaurantToken, Restaurant.RestaurantName)
                .Replace(DestinationToken, DeliveryPoint.DisplayName)
                .Replace(RewardToken, Food.Reward.ToString())
                .Replace(TimeToken, Mathf.RoundToInt(TimeLimit).ToString());

            return KoreanJosa.Resolve(text);
        }
    }
}
