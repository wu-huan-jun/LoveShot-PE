using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchState {none,singleTouch,singleStay,singleMove,Double}
public class ScreenTouchHandler : MonoBehaviour
{
    [SerializeField] MainManagerPE m_MainManager;
    static public TouchState m_touchState;//�ſ�������
    public TouchState lastTouchState;//�ſ�������
    public float touchTime;//�ſ�������
    public float lastTouchTime;//�ſ�������
    [Header("One Finger Touch")]
    public Vector2 touchMove;
    public Vector2 lastTouchMove;
    public Vector2 touchPoint;
    [Header("Two Fingers Touch")]
    public Vector2 entireMove;
    public Vector2 lastEntireMove;//������ָ�е���ƶ�
    public Vector2 originalRelativePos;//�ڶ�ָ�Ӵ���Ļʱ��Ե�һָ������
    public Vector2 relativePos;//�ڶ�ָ��Ե�һָ�����꣬��̬
    public float fingersDisatanceMultiplier;//������������ָ���ŵı���
    public Vector2 touchPoint1;//�ڶ�����ָ�Ĵ���

    float i; 

    // Start is called before the first frame update
    void Start()
    {
        m_MainManager = GetComponent<MainManagerPE>();
    }
    
    void OneFingerTouch()//��ָ����
    {
        Touch touch = Input.GetTouch(0);
        touchTime += Time.deltaTime;
        lastTouchTime = touchTime;
        touchMove += touch.deltaPosition;
        lastTouchMove = touchMove;
        if (touchMove.magnitude > 20)
        {
            m_touchState = TouchState.singleMove;
        }
        else
        {
            if (touchTime < 0.2)
            {
                m_touchState = TouchState.none;
                touchPoint = touch.position;
            }
            else
            {
                m_touchState = TouchState.singleStay;
                touchPoint = touch.position;
            }
        }
    }
    void TwoFingerTouch()//˫ָ����
    {
        m_touchState = TouchState.Double;
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        if (originalRelativePos == Vector2.zero)
            originalRelativePos = touch1.position - touch0.position;
        relativePos = touch1.position - touch0.position;
        fingersDisatanceMultiplier = relativePos.magnitude / originalRelativePos.magnitude;
        entireMove += touch0.deltaPosition / 2 + touch0.deltaPosition / 2;//��ָ�е��λ����
        
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.touchCount == 2)
        {
            TwoFingerTouch();
        }
        if(m_touchState!=TouchState.Double && Input.touchCount == 1)//˫ָ������ſ�һָ���ᱻ��Ϊ�ǵ�ָ����
        {
            OneFingerTouch();
        }/*
        else if (Input.touchCount == 1)
        {
            //����˫ָ
            originalRelativePos = relativePos;
            m_touchState = TouchState.singleStay;
        }*/
        if(Input.touchCount == 0)
        {
            if (m_touchState == TouchState.none && touchTime < 0.2 && touchTime >0)//ֻ���ڵ�ָѸ�ٵ��ʱ�򣬲Ż���touchstateΪnone��ͬʱ����Touchtime>0�Ŀ�����
            {
                m_touchState = TouchState.singleTouch;
                lastTouchState = m_touchState;
            }
            else { m_touchState = TouchState.none; }//�����������һ֡��TouchState.SingleTouch
            touchTime = 0;
            //���õ�ָ
            touchMove = Vector2.zero;

            //����˫ָ
            originalRelativePos = Vector2.zero;
            //relativePos = Vector2.zero;
            entireMove = Vector2.zero;
        }
    }
}
