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
        public QuestDataSO Data { get; }
        public DeliveryDestination Origin { get; }
        public DeliveryDestination DeliveryPoint { get; }
        public float TimeLimit { get; }

        public DeliveryQuest(QuestDataSO data, DeliveryDestination origin, DeliveryDestination deliveryPoint, float timeLimit)
        {
            QuestID = $"{data.FoodID}_{++_idCounter}";
            Data = data;
            Origin = origin;
            DeliveryPoint = deliveryPoint;
            TimeLimit = timeLimit;
        }

        public string FoodName => Data.DisplayName;

        public string OrderText => Format(string.IsNullOrEmpty(Data.OrderFormat) ? DefaultOrderFormat : Data.OrderFormat);

        public string PickupText => Format(PickupFormat);

        public string DeliveryText => Format(DeliveryFormat);

        public string Format(string format)
        {
            if (string.IsNullOrEmpty(format))
                return string.Empty;

            string text = format
                .Replace(FoodToken, FoodName)
                .Replace(RestaurantToken, Origin.DisplayName)
                .Replace(DestinationToken, DeliveryPoint.DisplayName)
                .Replace(RewardToken, Data.Reward.ToString())
                .Replace(TimeToken, Mathf.RoundToInt(TimeLimit).ToString());

            return KoreanJosa.Resolve(text);
        }
    }
}
