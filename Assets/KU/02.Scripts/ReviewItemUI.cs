using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewItemUI : MonoBehaviour
{
    [Header("작성자 정보")]
    [SerializeField]
    private Image profileIconImage;

    [SerializeField]
    private TMP_Text userNameText;


    [Header("리뷰 내용")]
    [SerializeField]
    private TMP_Text reviewText;


    [Header("별 UI")]
    [SerializeField]
    private Image[] stars;


    [Header("별 스프라이트")]
    [SerializeField]
    private Sprite filledStarSprite;

    [SerializeField]
    private Sprite emptyStarSprite;


    public void Setup(
        ReviewSO review,
        int starCount)
    {
        if (review == null)
            return;


        if (profileIconImage != null)
        {
            profileIconImage.sprite =
                review.profileIcon;
        }


        if (userNameText != null)
        {
            userNameText.text =
                review.userName;
        }


        if (reviewText != null)
        {
            reviewText.text =
                review.reviewContent;
        }


        SetStars(starCount);
    }


    private void SetStars(int starCount)
    {
        starCount =
            Mathf.Clamp(
                starCount,
                0,
                5
            );


        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null)
                continue;


            if (i < starCount)
            {
                stars[i].sprite =
                    filledStarSprite;
            }
            else
            {
                stars[i].sprite =
                    emptyStarSprite;
            }
        }
    }
}