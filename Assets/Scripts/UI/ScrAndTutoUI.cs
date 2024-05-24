using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScrAndTutoUI : UIManager
{
    [SerializeField] bool isLast = false;//�Ƿ���һ��ȫ������/�������׸�
    [SerializeField] bool isFirst = false;//�Ƿ���ĩ��
    [SerializeField] CanvasGroup m_canvasGroup;//�Լ���canvasGroup
    [SerializeField] CanvasGroup nextGroup;//�������ĩ��������һ����canvasGroup
    public bool triggedByTouch = true;//�Ƿ���ͨ��������Ļ����λ���л�����һ��
    public bool triggedByButton = false;//��Ҫͨ�������ť�����л�����һ��
    [SerializeField] Button button;//������ť
    public bool startFade;//Ϊtrueʱ�������canvasGroup�ĵ���������������һ��
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
            nextGroup.GetComponent<ScrAndTutoUI>().enabled = true;//������һ��
    }
    private void RunEvent()
    {
        Event.Invoke();
    }
    private void FixedUpdate()
    {
        if (!startFade && this.enabled)//����ʱ����
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
