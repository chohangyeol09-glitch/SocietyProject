using rayzngames;
using UnityEngine;

public class SpeedUI : MonoBehaviour
{
    [SerializeField] BicycleVehicle bicycle;
    [SerializeField] TMPro.TextMeshProUGUI speedText;

    void Update()
    {
        speedText.text = $"{bicycle.currentSpeed * 3.6f:0} km/h";
    }
}
