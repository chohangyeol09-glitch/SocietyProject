using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewItemUI : MonoBehaviour
{
    [Header("리뷰 텍스트")]
    [SerializeField]
    private TMP_Text userNameText;

    [SerializeField]
    private TMP_Text reviewText;


    [Header("별 이미지")]
    [SerializeField]
    private Image[] stars;


    [Header("별 스프라이트")]
    [SerializeField]
    private Sprite filledStarSprite;

    [SerializeField]
    private Sprite emptyStarSprite;


    public void Setup(
        string userName,
        string content,
        int starCount)
    {
        userNameText.text = userName;

        reviewText.text = content;


        starCount =
            Mathf.Clamp(starCount, 0, 5);


        for (int i = 0; i < stars.Length; i++)
        {
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