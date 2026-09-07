using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager :
    MonoSingleton<MissionManager>
{
    [Header("등장 가능한 미션")]
    [SerializeField]
    private List<MissionSO> missions =
        new List<MissionSO>();


    [Header("미션 UI")]
    [SerializeField]
    private MissionItemUI missionPrefab;

    [SerializeField]
    private Transform missionListParent;


    [Header("미션 등장 시간")]
    [SerializeField]
    private float minSpawnDelay = 10f;

    [SerializeField]
    private float maxSpawnDelay = 20f;


    // 현재 앱에 떠 있는 미션
    private MissionSO currentOfferedMission;


    // 현재 플레이어가 수락해서
    // 실제로 진행 중인 미션
    private MissionSO acceptedMission;


    // 현재 생성되어 있는 미션 UI
    private MissionItemUI currentMissionUI;


    private Coroutine spawnCoroutine;


    public event Action<MissionSO> MissionAccepted;


    public bool HasActiveMission
    {
        get
        {
            return currentOfferedMission != null &&
                   currentMissionUI != null;
        }
    }


    public bool HasAcceptedMission
    {
        get
        {
            return acceptedMission != null;
        }
    }


    public MissionSO AcceptedMission
    {
        get
        {
            return acceptedMission;
        }
    }


    private void Start()
    {
        StartNextMissionTimer();
    }


    // =====================================
    // 다음 미션 타이머
    // =====================================

    public void StartNextMissionTimer()
    {
        // 이미 타이머 작동 중
        if (spawnCoroutine != null)
            return;


        // 이미 받을 수 있는 미션이 떠 있음
        if (currentOfferedMission != null)
            return;


        // 이미 수행 중인 미션이 있음
        if (acceptedMission != null)
            return;


        spawnCoroutine =
            StartCoroutine(
                SpawnMissionAfterDelay()
            );
    }


    private IEnumerator SpawnMissionAfterDelay()
    {
        float delay =
            UnityEngine.Random.Range(
                minSpawnDelay,
                maxSpawnDelay
            );


        Debug.Log(
            $"다음 미션까지 {delay:F1}초"
        );


        yield return new WaitForSeconds(delay);


        spawnCoroutine = null;


        SpawnRandomMission();
    }


    // =====================================
    // 랜덤 미션 생성
    // =====================================

    public void SpawnRandomMission()
    {
        if (missions.Count == 0)
        {
            Debug.LogWarning(
                "MissionSO가 등록되어 있지 않습니다."
            );

            return;
        }


        if (missionPrefab == null)
        {
            Debug.LogError(
                "MissionPrefab이 연결되어 있지 않습니다."
            );

            return;
        }


        if (missionListParent == null)
        {
            Debug.LogError(
                "MissionListParent가 연결되어 있지 않습니다."
            );

            return;
        }


        // 이미 미션 UI가 있다면 생성하지 않음
        if (currentMissionUI != null)
            return;


        int randomIndex =
            UnityEngine.Random.Range(
                0,
                missions.Count
            );


        currentOfferedMission =
            missions[randomIndex];


        currentMissionUI =
            Instantiate(
                missionPrefab,
                missionListParent
            );


        currentMissionUI.Setup(
            currentOfferedMission
        );


        Debug.Log(
            $"새 미션 등장 : " +
            $"{currentOfferedMission.requesterName}"
        );


        // 핸드폰 알림 진동
        SmartPhoneManager.Instance
            .PlayNotificationVibration();
    }


    // =====================================
    // 미션 수락
    // =====================================

    public void AcceptCurrentMission()
    {
        if (currentOfferedMission == null)
            return;


        acceptedMission =
            currentOfferedMission;


        currentOfferedMission = null;


        // 새로운 배달 시작
        // 별 5개로 초기화
        HealthManager.Instance
            .ResetHealth();


        // 미션 UI 삭제
        if (currentMissionUI != null)
        {
            Destroy(
                currentMissionUI.gameObject
            );


            currentMissionUI = null;
        }


        Debug.Log(
            $"미션 수락 : " +
            $"{acceptedMission.requesterName}"
        );


        MissionAccepted?.Invoke(
            acceptedMission
        );
    }


    // =====================================
    // 현재 미션 수락 위치
    // =====================================

    public RectTransform GetAcceptTransform()
    {
        if (currentMissionUI == null)
            return null;


        return currentMissionUI
            .AcceptTransform;
    }


    // =====================================
    // 미션 성공
    // =====================================

    public void CompleteAcceptedMission()
    {
        if (acceptedMission == null)
        {
            Debug.LogWarning(
                "현재 수행 중인 미션이 없습니다."
            );

            return;
        }


        Debug.Log(
            $"미션 성공 : " +
            $"{acceptedMission.requesterName}"
        );


        // 현재 남은 체력
        // = 이번 배달의 리뷰 별점
        int starCount =
            HealthManager.Instance
                .GetCurrentHealth();


        Debug.Log(
            $"이번 배달 별점 : " +
            $"{starCount}"
        );


        // 현재 체력을 기준으로
        // 랜덤 리뷰 생성
        ReviewManager.Instance
            .AddRandomReview(
                starCount
            );


        // 수행 중인 미션 제거
        acceptedMission = null;


        // 다음 미션 타이머 시작
        StartNextMissionTimer();
    }


    // =====================================
    // 미션 실패
    // =====================================

    public void FailAcceptedMission()
    {
        if (acceptedMission == null)
        {
            Debug.LogWarning(
                "현재 수행 중인 미션이 없습니다."
            );

            return;
        }


        Debug.Log(
            $"미션 실패 : " +
            $"{acceptedMission.requesterName}"
        );


        acceptedMission = null;


        // 실패했으므로 리뷰는 생성하지 않음


        // 다음 미션 기다리기
        StartNextMissionTimer();
    }
}