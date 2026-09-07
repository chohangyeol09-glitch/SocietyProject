using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CHG.Scripts.DeliverySystem;

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


    public void Setup(QuestDataSO quest)
    {
        if (quest == null)
            return;


        // 미션 이름
        if (requesterNameText != null)
        {
            requesterNameText.text =
                quest.DisplayName;
        }


        // 배달 내용
        if (deliveryContentText != null)
        {
            deliveryContentText.text =
                quest.OrderFormat;
        }


        // 아이콘
        if (profileIconImage != null)
        {
            profileIconImage.sprite =
                quest.Icon;
        }
    }
}