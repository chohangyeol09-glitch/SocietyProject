using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHG.Scripts.DeliverySystem;

public class MissionManager :
    MonoSingleton<MissionManager>
{
    [Header("등장 가능한 퀘스트")]
    [SerializeField]
    private List<QuestDataSO> quests =
        new List<QuestDataSO>();


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


    // 현재 앱에 떠 있는 퀘스트
    private QuestDataSO currentOfferedQuest;


    // 현재 플레이어가 수락해서
    // 실제로 진행 중인 퀘스트
    private QuestDataSO acceptedQuest;


    // 현재 생성되어 있는 미션 UI
    private MissionItemUI currentMissionUI;


    private Coroutine spawnCoroutine;


    // 퀘스트가 수락되었을 때 호출되는 이벤트
    public event Action<QuestDataSO> MissionAccepted;


    // =====================================
    // 현재 제안된 미션 존재 여부
    // =====================================

    public bool HasActiveMission
    {
        get
        {
            return currentOfferedQuest != null &&
                   currentMissionUI != null;
        }
    }


    // =====================================
    // 현재 수행 중인 미션 존재 여부
    // =====================================

    public bool HasAcceptedMission
    {
        get
        {
            return acceptedQuest != null;
        }
    }


    // =====================================
    // 현재 수행 중인 퀘스트
    // =====================================

    public QuestDataSO AcceptedQuest
    {
        get
        {
            return acceptedQuest;
        }
    }


    // 기존 코드에서
    // AcceptedMission을 사용하고 있다면
    // 당장 오류 안 나게 이것도 남겨둘 수 있음
    public QuestDataSO AcceptedMission
    {
        get
        {
            return acceptedQuest;
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


        // 이미 받을 수 있는 퀘스트가 떠 있음
        if (currentOfferedQuest != null)
            return;


        // 이미 수행 중인 퀘스트가 있음
        if (acceptedQuest != null)
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
            $"다음 퀘스트까지 {delay:F1}초"
        );


        yield return new WaitForSeconds(delay);


        spawnCoroutine = null;


        SpawnRandomMission();
    }


    // =====================================
    // 랜덤 퀘스트 생성
    // =====================================

    public void SpawnRandomMission()
    {
        if (quests == null ||
            quests.Count == 0)
        {
            Debug.LogWarning(
                "QuestDataSO가 등록되어 있지 않습니다."
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
                quests.Count
            );


        currentOfferedQuest =
            quests[randomIndex];


        if (currentOfferedQuest == null)
        {
            Debug.LogWarning(
                "선택된 QuestDataSO가 비어 있습니다."
            );

            return;
        }


        currentMissionUI =
            Instantiate(
                missionPrefab,
                missionListParent
            );


        currentMissionUI.Setup(
            currentOfferedQuest
        );


        Debug.Log(
            $"새 퀘스트 등장 : " +
            $"{currentOfferedQuest.DisplayName}"
        );


        Debug.Log(
            $"음식 : {currentOfferedQuest.FoodID} / " +
            $"출발지 : {currentOfferedQuest.OriginID} / " +
            $"목적지 : {currentOfferedQuest.DestinationID}"
        );


        // 핸드폰 알림 진동
        if (SmartPhoneManager.Instance != null)
        {
            SmartPhoneManager.Instance
                .PlayNotificationVibration();
        }
    }


    // =====================================
    // 미션 수락
    // =====================================

    public void AcceptCurrentMission()
    {
        if (currentOfferedQuest == null)
            return;


        acceptedQuest =
            currentOfferedQuest;


        currentOfferedQuest = null;


        // 새로운 배달 시작
        // 별 5개로 초기화
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance
                .ResetHealth();
        }


        // 미션 UI 삭제
        if (currentMissionUI != null)
        {
            Destroy(
                currentMissionUI.gameObject
            );


            currentMissionUI = null;
        }


        Debug.Log(
            $"퀘스트 수락 : " +
            $"{acceptedQuest.DisplayName}"
        );


        Debug.Log(
            $"출발지 : {acceptedQuest.OriginID} / " +
            $"목적지 : {acceptedQuest.DestinationID} / " +
            $"제한시간 : {acceptedQuest.TimeLimit}초 / " +
            $"보상 : {acceptedQuest.Reward}"
        );


        MissionAccepted?.Invoke(
            acceptedQuest
        );
    }


    // =====================================
    // 현재 미션 수락 버튼 위치
    // =====================================

    public RectTransform GetAcceptTransform()
    {
        if (currentMissionUI == null)
            return null;


        return currentMissionUI
            .AcceptTransform;
    }


    // =====================================
    // 현재 수행 중인 퀘스트 가져오기
    // =====================================

    public QuestDataSO GetAcceptedQuest()
    {
        return acceptedQuest;
    }


    // =====================================
    // 현재 제안된 퀘스트 가져오기
    // =====================================

    public QuestDataSO GetOfferedQuest()
    {
        return currentOfferedQuest;
    }


    // =====================================
    // 미션 성공
    // =====================================

    public void CompleteAcceptedMission()
    {
        if (acceptedQuest == null)
        {
            Debug.LogWarning(
                "현재 수행 중인 퀘스트가 없습니다."
            );

            return;
        }


        Debug.Log(
            $"퀘스트 성공 : " +
            $"{acceptedQuest.DisplayName}"
        );


        Debug.Log(
            $"보상 : {acceptedQuest.Reward}"
        );


        // 현재 남은 체력
        // = 이번 배달의 리뷰 별점
        int starCount = 0;


        if (HealthManager.Instance != null)
        {
            starCount =
                HealthManager.Instance
                    .GetCurrentHealth();
        }


        Debug.Log(
            $"이번 배달 별점 : " +
            $"{starCount}"
        );


        // 현재 체력을 기준으로
        // 랜덤 리뷰 생성
        if (ReviewManager.Instance != null)
        {
            ReviewManager.Instance
                .AddRandomReview(
                    starCount
                );
        }


        // TODO:
        // 돈 시스템이 있다면 여기에서
        // acceptedQuest.Reward 만큼 지급하면 됨.
        //
        // 예:
        // MoneyManager.Instance.AddMoney(
        //     acceptedQuest.Reward
        // );


        // 수행 중인 퀘스트 제거
        acceptedQuest = null;


        // 다음 미션 타이머 시작
        StartNextMissionTimer();
    }


    // =====================================
    // 미션 실패
    // =====================================

    public void FailAcceptedMission()
    {
        if (acceptedQuest == null)
        {
            Debug.LogWarning(
                "현재 수행 중인 퀘스트가 없습니다."
            );

            return;
        }


        Debug.Log(
            $"퀘스트 실패 : " +
            $"{acceptedQuest.DisplayName}"
        );


        acceptedQuest = null;


        // 실패했으므로 리뷰는 생성하지 않음


        // 다음 미션 기다리기
        StartNextMissionTimer();
    }
}