using UnityEngine;

namespace CHG.Scripts.DeliverySystem
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "CHG/Delivery/QuestData", order = 0)]
    public class QuestDataSO : ScriptableObject
    {
        [Header("Key")]
        [SerializeField] private string foodID;

        [Header("Route")]
        [Tooltip("DeliveryDestination의 DestinationID와 일치해야 합니다 (출발/픽업 지점).")]
        [SerializeField] private string originID;
        [Tooltip("DeliveryDestination의 DestinationID와 일치해야 합니다 (배달 목적지).")]
        [SerializeField] private string destinationID;

        [Header("UI")]
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField, TextArea]
        [Tooltip("{food} {restaurant} {destination} {reward} {time} 사용 가능. 조사는 을(를), 이(가), 은(는), 와(과), (으)로 형태로 쓰면 자동으로 맞춰집니다.\n예) {food}을(를) {destination}(으)로 배달해주세요!")]
        private string orderFormat;

        [Header("Rule")]
        [SerializeField] private float timeLimit = 180f;
        [SerializeField] private int reward = 1000;

        public string FoodID => string.IsNullOrEmpty(foodID) ? name : foodID;
        public string OriginID => originID;
        public string DestinationID => destinationID;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? FoodID : displayName;
        public Sprite Icon => icon;
        public string OrderFormat => orderFormat;
        public float TimeLimit => timeLimit;
        public int Reward => reward;
    }
}
