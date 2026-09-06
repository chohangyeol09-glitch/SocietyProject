using rayzngames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{
    [SerializeField] BicycleVehicle bicycle;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] private Image speedImage;

    [SerializeField] private TextMeshProUGUI xText;
    [SerializeField] private TextMeshProUGUI yText;

    void Update()
    {
        speedText.text = $"{bicycle.currentSpeed * 3.6f:0}";
        speedImage.fillAmount = (bicycle.currentSpeed * 3.6f) / 100f;
        xText.text = $"X: {bicycle.transform.position.x:N0}";
        yText.text = $"Y: {bicycle.transform.position.z:N0}";
    }
}
