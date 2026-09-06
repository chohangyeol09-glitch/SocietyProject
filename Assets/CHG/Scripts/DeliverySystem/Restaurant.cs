using System.Collections.Generic;
using UnityEngine;

namespace CHG.Scripts.DeliverySystem
{
    [RequireComponent(typeof(DeliveryDestination))]
    public class Restaurant : MonoBehaviour
    {
        [SerializeField] private string restaurantName;
        [SerializeField] private List<FoodDataSO> menu = new List<FoodDataSO>();

        private DeliveryDestination _destination;

        public DeliveryDestination Destination => _destination;
        public IReadOnlyList<FoodDataSO> Menu => menu;
        public string RestaurantName => string.IsNullOrEmpty(restaurantName) ? name : restaurantName;

        private void Awake()
        {
            _destination = GetComponent<DeliveryDestination>();
        }

        public bool HasFood(FoodDataSO food) => menu.Contains(food);

        public bool HasFood(string foodID) => menu.Exists(f => f != null && f.FoodID == foodID);

        public FoodDataSO GetRandomFood()
        {
            return menu.Count == 0 ? null : menu[Random.Range(0, menu.Count)];
        }
    }
}
