using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//���������ÿһ��ȫ������/�̳�
public class ScrAndTutoUI : UIManager
{
    public bool triggedByTouch = true;//�Ƿ���ͨ��������Ļ����λ���л�����һ��
    public bool triggedByButton = false;//��Ҫͨ�������ť�����л�����һ��
    [SerializeField] Button button;//������ť
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
