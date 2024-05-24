using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScrAndTutoUI : UIManager
{
    [SerializeField] bool isLast = false;//是否是一组全屏剧情/动画的首个
    [SerializeField] bool isFirst = false;//是否是末个
    [SerializeField] CanvasGroup m_canvasGroup;//自己的canvasGroup
    [SerializeField] CanvasGroup nextGroup;//如果不是末个，则下一个的canvasGroup
    public bool triggedByTouch = true;//是否能通过触摸屏幕任意位置切换到下一张
    public bool triggedByButton = false;//需要通过点击按钮才能切换到下一张
    [SerializeField] Button button;//触发按钮
    public bool startFade;//为true时运行这个canvasGroup的淡出，并且启用下一个
    bool trigged;
    [SerializeField] UnityEvent Event;
    // Start is called before the first frame update
    void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        if (!isFirst)
        {
            m_canvasGroup.alpha = 0;
            m_canvasGroup.interactable = false;
            this.enabled = false;
        }
        if (triggedByButton)
        {
            button.onClick.AddListener(StartFade);
        }
    }
    private void StartFade()
    {
        startFade = true;
        if (!trigged) RunEvent();
        if (!isLast)
            nextGroup.GetComponent<ScrAndTutoUI>().enabled = true;//启用下一个
    }
    private void RunEvent()
    {
        Event.Invoke();
    }
    private void FixedUpdate()
    {
        if (!startFade && this.enabled)//启用时淡入
        {
            FadeIn(m_canvasGroup, .45f);
        }
        if (startFade)
        {
            FadeOut(m_canvasGroup, .5f);
            if (m_canvasGroup.alpha == 0)
            {
                this.enabled = false;
            }
        }
        if (triggedByTouch&&(ScreenTouchHandler.m_touchState!= TouchState.none||Input.GetKeyDown(KeyCode.Mouse0))
        && m_canvasGroup.alpha==1)
            StartFade();
    }
}
