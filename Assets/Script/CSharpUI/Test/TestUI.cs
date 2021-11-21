
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    public Transform testEmpty;
    public Text title;
    public Button testBtn;
    public Toggle testToggle;
    public Image testImg;
    public ScrollRect testScroll;
    public RawImage testRimg;

    private void Awake()
    {
        testEmpty = transform.Find("testEmpty");
        title = transform.Find("testEmpty/title").GetComponent<Text>();
        testBtn = transform.Find("testBtn").GetComponent<Button>();
        testToggle = transform.Find("testToggle").GetComponent<Toggle>();
        testImg = transform.Find("testImg").GetComponent<Image>();
        testScroll = transform.Find("testScroll").GetComponent<ScrollRect>();
        testRimg = transform.Find("testRimg").GetComponent<RawImage>();

        testBtn.AddListener(EventTriggerType.PointerClick, TestBtnOnPointerClick);
        testImg.AddListener(EventTriggerType.Drag, TestImgOnDrag);
        testRimg.AddListener(EventTriggerType.PointerEnter, TestRimgOnPointerEnter);
    }
    

    private void TestBtnOnPointerClick(BaseEventData eventData)
    {
            
    }

    private void TestImgOnDrag(BaseEventData eventData)
    {
            
    }

    private void TestRimgOnPointerEnter(BaseEventData eventData)
    {
            
    }
}
