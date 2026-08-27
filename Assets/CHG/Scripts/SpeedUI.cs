using rayzngames;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{
    [SerializeField] BicycleVehicle bicycle;
    [SerializeField] TMPro.TextMeshProUGUI speedText;
    [SerializeField] private Image speedImage;

    void Update()
    {
        speedText.text = $"{bicycle.currentSpeed * 3.6f:0} km/h";
        speedImage.fillAmount = (bicycle.currentSpeed * 3.6f) / 100f;
    }
}
