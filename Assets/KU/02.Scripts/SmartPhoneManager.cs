using System.Collections;
using UnityEngine;

public class SmartPhoneManager : MonoSingleton<SmartPhoneManager>
{
    [Header("핸드폰 전체 UI")]
    [SerializeField]
    private RectTransform smartPhoneRoot;


    [Header("진동 설정")]
    [SerializeField]
    private float vibrationDistance = 8f;

    [SerializeField]
    private float vibrationSpeed = 0.04f;

    [SerializeField]
    private int vibrationCount = 4;


    private Vector2 originalPosition;

    private Coroutine vibrationCoroutine;


    private void Start()
    {
        if (smartPhoneRoot != null)
        {
            originalPosition =
                smartPhoneRoot.anchoredPosition;
        }
    }


    // 알림이 왔을 때 호출
    public void PlayNotificationVibration()
    {
        if (smartPhoneRoot == null)
        {
            Debug.LogWarning(
                "SmartPhoneRoot가 연결되어 있지 않습니다."
            );

            return;
        }


        // 이미 진동 중이면 기존 진동 중지
        if (vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);

            smartPhoneRoot.anchoredPosition =
                originalPosition;
        }


        vibrationCoroutine =
            StartCoroutine(
                VibrationCoroutine()
            );
    }


    private IEnumerator VibrationCoroutine()
    {
        for (int i = 0;
             i < vibrationCount;
             i++)
        {
            // 위로
            smartPhoneRoot.anchoredPosition =
                originalPosition +
                Vector2.up * vibrationDistance;

            yield return new WaitForSeconds(
                vibrationSpeed
            );


            // 아래로
            smartPhoneRoot.anchoredPosition =
                originalPosition +
                Vector2.down * vibrationDistance;

            yield return new WaitForSeconds(
                vibrationSpeed
            );
        }


        // 원래 위치 복구
        smartPhoneRoot.anchoredPosition =
            originalPosition;


        vibrationCoroutine = null;
    }
}