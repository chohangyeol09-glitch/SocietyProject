using UnityEngine;

namespace CHG.Scripts.DeliverySystem
{
    [CreateAssetMenu(fileName = "FoodData", menuName = "CHG/Delivery/FoodData", order = 0)]
    public class FoodDataSO : ScriptableObject
    {
        [Header("Key")]
        [SerializeField] private string foodID;

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
        public string DisplayName => string.IsNullOrEmpty(displayName) ? FoodID : displayName;
        public Sprite Icon => icon;
        public string OrderFormat => orderFormat;
        public float TimeLimit => timeLimit;
        public int Reward => reward;
    }
}
