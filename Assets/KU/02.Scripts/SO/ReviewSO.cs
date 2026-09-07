using UnityEngine;

[CreateAssetMenu(
    fileName = "Review",
    menuName = "Delivery/Review"
)]
public class ReviewSO : ScriptableObject
{
    [Header("작성자 이름")]
    public string userName;

    [Header("리뷰 내용")]
    [TextArea(2, 5)]
    public string reviewContent;

    [Header("프로필 아이콘")]
    public Sprite profileIcon;
}