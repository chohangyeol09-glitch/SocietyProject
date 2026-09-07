using UnityEngine;

[CreateAssetMenu(
    fileName = "Mission",
    menuName = "Delivery/Mission"
)]
public class MissionSO : ScriptableObject
{
    [Header("신청자 이름")]
    public string requesterName;

    [Header("배달 내용")]
    [TextArea(2, 5)]
    public string deliveryContent;

    [Header("프로필 아이콘")]
    public Sprite profileIcon;
}