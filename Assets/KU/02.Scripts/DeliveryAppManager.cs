using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeliveryAppManager : MonoBehaviour
{
    [Serializable]
    public class DeliveryMenu
    {
        [Header("메뉴 이름")]
        public string menuName;

        [Header("메뉴 선택 위치")]
        public RectTransform menuTransform;

        [Header("선택 시 켜질 오브젝트")]
        public GameObject contentObject;
    }


    [Header("App Manager")]
    [SerializeField]
    private AppManager appManager;


    [Header("배달앱 메뉴")]
    [SerializeField]
    private List<DeliveryMenu> menus =
        new List<DeliveryMenu>();


    [Header("선택 표시")]
    [SerializeField]
    private RectTransform selectionFrame;


    private int selectedIndex = 0;

    private int openedFrame;


    private void OnEnable()
    {
        openedFrame = Time.frameCount;

        selectedIndex = 0;

        CloseAllContents();

        UpdateSelection();
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;


        // 홈 화면에서 Enter로 배달앱을 연
        // 같은 프레임의 Enter 입력 방지
        if (Time.frameCount == openedFrame)
            return;


        HandleNavigation();
        HandleSelect();


        // 배달앱에서 ESC → 핸드폰 홈
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            appManager.OpenHome();
        }
    }


    private void HandleNavigation()
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


    private void HandleSelect()
    {
        bool enterPressed =
            Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame;


        if (enterPressed)
        {
            OpenSelectedMenu();
        }
    }


    private void MoveSelection(int direction)
    {
        if (menus.Count == 0)
            return;


        selectedIndex += direction;


        if (selectedIndex < 0)
        {
            selectedIndex = menus.Count - 1;
        }


        if (selectedIndex >= menus.Count)
        {
            selectedIndex = 0;
        }


        UpdateSelection();
    }


    private void UpdateSelection()
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


    private void OpenSelectedMenu()
    {
        if (menus.Count == 0)
            return;


        DeliveryMenu selectedMenu =
            menus[selectedIndex];


        Debug.Log(
            $"배달앱 메뉴 선택 : {selectedMenu.menuName}"
        );


        // 리뷰 / 지도 / 임무 모두 끄기
        CloseAllContents();


        // 선택한 것 하나만 켜기
        if (selectedMenu.contentObject != null)
        {
            selectedMenu.contentObject.SetActive(true);
        }
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