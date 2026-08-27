using System;
using System.Collections.Generic;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{
    [Serializable]
    public class ReviewData
    {
        [Header("작성자 이름")]
        public string userName;

        [Header("리뷰 내용")]
        [TextArea]
        public string content;

        [Range(0, 5)]
        public int starCount;
    }


    [Header("리뷰 프리팹")]
    [SerializeField]
    private ReviewItemUI reviewPrefab;


    [Header("리뷰가 생성될 부모")]
    [SerializeField]
    private Transform reviewListParent;


    [Header("랜덤으로 등장할 리뷰")]
    [SerializeField]
    private List<ReviewData> randomReviews =
        new List<ReviewData>();


    /// <summary>
    /// 미션 성공 등의 신호를 받았을 때 호출.
    /// 등록된 리뷰 중 하나를 랜덤으로 생성한다.
    /// </summary>
    public void AddRandomReview()
    {
        if (randomReviews.Count == 0)
        {
            Debug.LogWarning(
                "생성할 리뷰 데이터가 없습니다."
            );

            return;
        }


        int randomIndex =
            UnityEngine.Random.Range(
                0,
                randomReviews.Count
            );


        ReviewData review =
            randomReviews[randomIndex];


        CreateReview(review);
    }


    /// <summary>
    /// 원하는 리뷰 데이터를 직접 생성할 때 사용.
    /// 나중에 플레이 결과에 따른 리뷰 생성에 사용할 수 있음.
    /// </summary>
    public void AddReview(
        string userName,
        string content,
        int starCount)
    {
        ReviewData review =
            new ReviewData
            {
                userName = userName,
                content = content,
                starCount = starCount
            };


        CreateReview(review);
    }


    private void CreateReview(
        ReviewData review)
    {
        if (reviewPrefab == null)
        {
            Debug.LogError(
                "ReviewPrefab이 연결되지 않았습니다."
            );

            return;
        }


        if (reviewListParent == null)
        {
            Debug.LogError(
                "ReviewListParent가 연결되지 않았습니다."
            );

            return;
        }


        ReviewItemUI newReview =
            Instantiate(
                reviewPrefab,
                reviewListParent
            );


        newReview.Setup(
            review.userName,
            review.content,
            review.starCount
        );
    }
}