using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AppManager : MonoSingleton<AppManager>
{
    [Serializable]
    public class PhoneApp
    {
        [Header("앱 이름")]
        public string appName;

        [Header("홈 화면 아이콘 위치")]
        public RectTransform iconTransform;

        [Header("홈 화면 아이콘 이미지")]
        public Image iconImage;

        [Header("앱 전체 UI")]
        public GameObject appScreenObject;
    }


    [Header("앱 목록")]
    [SerializeField]
    private List<PhoneApp> apps =
        new List<PhoneApp>();


    [Header("홈 화면 전체")]
    [SerializeField]
    private GameObject homeScreen;


    [Header("홈 화면 선택 표시")]
    [SerializeField]
    private RectTransform selectionFrame;


    [Header("아이콘 투명도")]
    [Range(0f, 1f)]
    [SerializeField]
    private float selectedAlpha = 1f;

    [Range(0f, 1f)]
    [SerializeField]
    private float unselectedAlpha = 0.5f;


    private const int ColumnCount = 3;

    private int selectedIndex = 0;

    private bool isAppOpened = false;


    private void Start()
    {
        OpenHome();
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;


        // 앱이 실행 중이면
        // AppManager는 더 이상 입력을 처리하지 않음.
        if (isAppOpened)
            return;


        HandleNavigation();
        HandleSelect();
    }


    private void HandleNavigation()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            MoveHorizontal(-1);
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            MoveHorizontal(1);
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            MoveVertical(-1);
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            MoveVertical(1);
        }
    }


    private void MoveHorizontal(int direction)
    {
        if (apps.Count == 0)
            return;


        int currentRow =
            selectedIndex / ColumnCount;

        int targetIndex =
            selectedIndex + direction;


        if (targetIndex < 0 ||
            targetIndex >= apps.Count)
        {
            return;
        }


        int targetRow =
            targetIndex / ColumnCount;


        // 좌우 이동 중 다른 줄로 넘어가지 않도록 방지
        if (currentRow != targetRow)
            return;


        selectedIndex = targetIndex;

        UpdateSelection();
    }


    private void MoveVertical(int direction)
    {
        if (apps.Count == 0)
            return;


        int targetIndex =
            selectedIndex +
            (direction * ColumnCount);


        if (targetIndex < 0 ||
            targetIndex >= apps.Count)
        {
            return;
        }


        selectedIndex = targetIndex;

        UpdateSelection();
    }


    private void HandleSelect()
    {
        bool enterPressed =
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame;


        if (enterPressed)
        {
            OpenSelectedApp();
        }
    }


    private void UpdateSelection()
    {
        if (apps.Count == 0)
            return;


        // SelectionFrame 이동
        if (selectionFrame != null)
        {
            RectTransform target =
                apps[selectedIndex].iconTransform;


            if (target != null)
            {
                selectionFrame.position =
                    target.position;
            }
        }


        // 모든 앱 아이콘 투명도 갱신
        for (int i = 0; i < apps.Count; i++)
        {
            if (apps[i].iconImage == null)
                continue;


            Color iconColor =
                apps[i].iconImage.color;


            // 선택된 앱만 100%
            if (i == selectedIndex)
            {
                iconColor.a =
                    selectedAlpha;
            }
            else
            {
                // 나머지는 50%
                iconColor.a =
                    unselectedAlpha;
            }


            apps[i].iconImage.color =
                iconColor;
        }
    }


    private void OpenSelectedApp()
    {
        if (apps.Count == 0)
            return;


        PhoneApp selectedApp =
            apps[selectedIndex];


        Debug.Log(
            $"앱 실행 : {selectedApp.appName}"
        );


        // 홈 화면 전체 OFF
        if (homeScreen != null)
        {
            homeScreen.SetActive(false);
        }


        // 다른 앱 화면 전부 OFF
        CloseAllApps();


        // 선택한 앱만 ON
        if (selectedApp.appScreenObject != null)
        {
            selectedApp.appScreenObject.SetActive(true);
        }


        isAppOpened = true;
    }


    private void CloseAllApps()
    {
        foreach (PhoneApp app in apps)
        {
            if (app.appScreenObject == null)
                continue;


            app.appScreenObject.SetActive(false);
        }
    }


    public void OpenHome()
    {
        // 모든 앱 화면 OFF
        CloseAllApps();


        // 홈 화면만 ON
        if (homeScreen != null)
        {
            homeScreen.SetActive(true);
        }


        isAppOpened = false;


        // 선택 위치와 아이콘 투명도 다시 적용
        UpdateSelection();
    }
}