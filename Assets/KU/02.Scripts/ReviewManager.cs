using System.Collections.Generic;
using UnityEngine;

public class ReviewManager :
    MonoSingleton<ReviewManager>
{
    [Header("등장 가능한 리뷰")]
    [SerializeField]
    private List<ReviewSO> reviews =
        new List<ReviewSO>();


    [Header("리뷰 프리팹")]
    [SerializeField]
    private ReviewItemUI reviewPrefab;


    [Header("리뷰 생성 위치")]
    [SerializeField]
    private Transform reviewListParent;


    public void AddRandomReview(
        int starCount)
    {
        if (reviews.Count == 0)
        {
            Debug.LogWarning(
                "ReviewManager에 ReviewSO가 등록되어 있지 않습니다."
            );

            return;
        }


        if (reviewPrefab == null)
        {
            Debug.LogError(
                "ReviewPrefab이 연결되어 있지 않습니다."
            );

            return;
        }


        if (reviewListParent == null)
        {
            Debug.LogError(
                "ReviewListParent가 연결되어 있지 않습니다."
            );

            return;
        }


        int randomIndex =
            Random.Range(
                0,
                reviews.Count
            );


        ReviewSO selectedReview =
            reviews[randomIndex];


        CreateReview(
            selectedReview,
            starCount
        );
    }


    private void CreateReview(
        ReviewSO review,
        int starCount)
    {
        if (review == null)
            return;


        ReviewItemUI newReview =
            Instantiate(
                reviewPrefab,
                reviewListParent
            );


        newReview.Setup(
            review,
            starCount
        );


        Debug.Log(
            $"리뷰 생성 : " +
            $"{review.userName} / " +
            $"별 {starCount}개"
        );


        // 새 리뷰 알림
        SmartPhoneManager.Instance
            .PlayNotificationVibration();
    }
}