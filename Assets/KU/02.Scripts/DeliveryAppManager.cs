using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CHG.Scripts.DeliverySystem;

public class DeliveryAppManager : MonoSingleton<DeliveryAppManager>
{
    public enum DeliveryMenuType
    {
        Review,
        Map,
        Mission
    }


    [Serializable]
    public class DeliveryMenu
    {
        [Header("메뉴 종류")]
        public DeliveryMenuType menuType;

        [Header("메뉴 이름")]
        public string menuName;

        [Header("메뉴 선택 위치")]
        public RectTransform menuTransform;

        [Header("선택하면 켜질 오브젝트")]
        public GameObject contentObject;
    }


    [Header("App Manager")]
    [SerializeField]
    private AppManager appManager;


    [Header("Mission Manager")]
    [SerializeField]
    private MissionManager missionManager;


    [Header("배달앱 메뉴")]
    [SerializeField]
    private List<DeliveryMenu> menus =
        new List<DeliveryMenu>();


    [Header("선택 표시")]
    [SerializeField]
    private RectTransform selectionFrame;


    private int selectedIndex = 0;

    private int openedFrame;

    private bool isAcceptSelected = false;


    private void OnEnable()
    {
        openedFrame =
            Time.frameCount;


        selectedIndex = 0;

        isAcceptSelected = false;


        CloseAllContents();

        UpdateMenuSelection();


        if (missionManager != null)
        {
            missionManager.MissionAccepted +=
                OnMissionAccepted;
        }
    }


    private void OnDisable()
    {
        if (missionManager != null)
        {
            missionManager.MissionAccepted -=
                OnMissionAccepted;
        }
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;


        if (Time.frameCount == openedFrame)
            return;


        // 수락 영역을 선택하고 있는 상태
        if (isAcceptSelected)
        {
            HandleAcceptSelection();

            return;
        }


        HandleMenuNavigation();

        HandleMenuSelect();


        // 현재 Mission 메뉴에서 아래로 내려가기
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            TryMoveToAccept();
        }


        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            appManager.OpenHome();
        }
    }


    private void HandleMenuNavigation()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            MoveSelection(-1);
        }


        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            MoveSelection(1);
        }
    }


    private void MoveSelection(int direction)
    {
        if (menus.Count == 0)
            return;


        selectedIndex += direction;


        if (selectedIndex < 0)
        {
            selectedIndex =
                menus.Count - 1;
        }


        if (selectedIndex >= menus.Count)
        {
            selectedIndex = 0;
        }


        UpdateMenuSelection();
    }


    private void HandleMenuSelect()
    {
        bool enterPressed =
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame;


        if (!enterPressed)
            return;


        OpenSelectedMenu();
    }


    private void OpenSelectedMenu()
    {
        if (menus.Count == 0)
            return;


        DeliveryMenu menu =
            menus[selectedIndex];


        CloseAllContents();


        if (menu.contentObject != null)
        {
            menu.contentObject.SetActive(true);
        }


        Debug.Log(
            $"배달앱 메뉴 선택 : {menu.menuName}"
        );
    }


    private void TryMoveToAccept()
    {
        if (menus.Count == 0)
            return;


        DeliveryMenu menu =
            menus[selectedIndex];


        // 현재 상단 선택이 Mission이 아니면 안 내려감
        if (menu.menuType != DeliveryMenuType.Mission)
            return;


        // MissionObject를 먼저 Enter로 열어둔 상태여야 함
        if (menu.contentObject == null ||
            !menu.contentObject.activeSelf)
        {
            return;
        }


        if (missionManager == null)
            return;


        if (!missionManager.HasActiveMission)
            return;


        RectTransform acceptTransform =
            missionManager.GetAcceptTransform();


        if (acceptTransform == null)
            return;


        isAcceptSelected = true;


        if (selectionFrame != null)
        {
            selectionFrame.position =
                acceptTransform.position;
        }
    }


    private void HandleAcceptSelection()
    {
        // ↑를 누르면 다시 상단 Mission 선택으로
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            ReturnToMenu();

            return;
        }


        // Enter = 수락
        bool enterPressed =
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame;


        if (enterPressed)
        {
            if (missionManager != null)
            {
                missionManager.AcceptCurrentMission();
            }

            return;
        }


        // ESC = 상단 메뉴로 복귀
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToMenu();
        }
    }


    // QuestDataSO 기준으로 변경
    private void OnMissionAccepted(
        QuestDataSO quest)
    {
        Debug.Log(
            $"배달앱에서 퀘스트 수락 확인 : " +
            $"{quest.DisplayName}"
        );


        // 미션 UI 프리팹이 사라졌으므로
        // SelectionFrame을 다시 Mission 탭으로 올림
        ReturnToMenu();
    }


    private void ReturnToMenu()
    {
        isAcceptSelected = false;

        UpdateMenuSelection();
    }


    private void UpdateMenuSelection()
    {
        if (menus.Count == 0)
            return;


        if (selectionFrame == null)
            return;


        RectTransform target =
            menus[selectedIndex].menuTransform;


        if (target == null)
            return;


        selectionFrame.position =
            target.position;
    }


    private void CloseAllContents()
    {
        foreach (DeliveryMenu menu in menus)
        {
            if (menu.contentObject == null)
                continue;


            menu.contentObject.SetActive(false);
        }
    }
}