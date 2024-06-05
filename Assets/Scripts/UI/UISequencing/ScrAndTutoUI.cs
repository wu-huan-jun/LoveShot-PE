using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//负责处理具体每一张全屏剧情/教程
public class ScrAndTutoUI : UIManager
{
    public bool triggedByTouch = true;//是否能通过触摸屏幕任意位置切换到下一张
    public bool triggedByButton = false;//需要通过点击按钮才能切换到下一张
    [SerializeField] Button button;//触发按钮
    [SerializeField] UnityEvent Event;
    CanvasGroup group;
    [SerializeField] ScrAndTutoUISequencer sequencer;
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        sequencer = GetComponent<Transform>().GetComponentInParent<ScrAndTutoUISequencer>();
        if (triggedByButton)
        {
            button.onClick.AddListener(StartFade);
        }
    }
    private void StartFade()
    {
        RunEvent();
        sequencer.NextObject();
    }
    private void RunEvent()
    {
        Event.Invoke();
    }
    private void FixedUpdate()
    {
        if (triggedByTouch&&(ScreenTouchHandler.m_touchState!= TouchState.none||Input.GetKeyDown(KeyCode.Mouse0))&&group.alpha == 1)
            StartFade();
    }
}
