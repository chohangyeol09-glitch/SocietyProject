using System;
using CHG.Scripts.DeliverySystem;
using rayzngames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BikeUI : MonoBehaviour
{
    [SerializeField] BicycleVehicle bicycle;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] private Image speedImage;
    [SerializeField] private DeliveryManager deliveryManager;

    [SerializeField] private TextMeshProUGUI pXText;
    [SerializeField] private TextMeshProUGUI pYText;
    [SerializeField] private TextMeshProUGUI tXText;
    [SerializeField] private TextMeshProUGUI tYText;
    [SerializeField] private TextMeshProUGUI timeText;

    private void Start()
    {
        deliveryManager.OnDestinationPositionChanged += HandleDestinationPositionChanged;
        deliveryManager.OnRemainingTimeChanged += HandleRemainingTimeChanged;
    }

    private void OnDestroy()
    {
        deliveryManager.OnDestinationPositionChanged -= HandleDestinationPositionChanged;
        deliveryManager.OnRemainingTimeChanged -= HandleRemainingTimeChanged;
    }

    private void HandleDestinationPositionChanged(Vector2 pos)
    {
        tXText.text = pos.x.ToString("F1");
        tYText.text = pos.y.ToString("F1");
    }

    private void HandleRemainingTimeChanged(float remaining)
    {
        timeText.text = Mathf.CeilToInt(remaining).ToString();
    }


    void Update()
    {
        speedText.text = $"{bicycle.currentSpeed * 3.6f:0}";
        speedImage.fillAmount = (bicycle.currentSpeed * 3.6f) / 100f;
        pXText.text = $"X: {bicycle.transform.position.x:N0}";
        pYText.text = $"Y: {bicycle.transform.position.z:N0}";
    }
}
