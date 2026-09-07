using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItemUI : MonoBehaviour
{
    [Header("미션 UI")]
    [SerializeField]
    private Image profileIconImage;

    [SerializeField]
    private TMP_Text requesterNameText;

    [SerializeField]
    private TMP_Text deliveryContentText;


    [Header("수락 선택 위치")]
    [SerializeField]
    private RectTransform acceptTransform;


    public RectTransform AcceptTransform
    {
        get
        {
            return acceptTransform;
        }
    }


    public void Setup(MissionSO mission)
    {
        if (mission == null)
            return;


        if (requesterNameText != null)
        {
            requesterNameText.text =
                mission.requesterName;
        }


        if (deliveryContentText != null)
        {
            deliveryContentText.text =
                mission.deliveryContent;
        }


        if (profileIconImage != null)
        {
            profileIconImage.sprite =
                mission.profileIcon;
        }
    }
}