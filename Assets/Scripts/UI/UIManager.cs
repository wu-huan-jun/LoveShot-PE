using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhoneAtti {l,r,u,d,N};
public class UIManager : MonoBehaviour
{
    public static Vector2 ScreenRes ;//屏幕分辨率
    public void GetScreenSize()
    {
#if UNITY_EDITOR
        ScreenRes = UnityEditor.Handles.GetMainGameViewSize();
#else
        ScreenRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
#endif
    }
    public bool touchInArea(Vector2 touchPoint,float xMin, float xMax, float yMin, float yMax)
    {
        if (touchPoint.x > xMin && touchPoint.x < xMax && touchPoint.y > yMin && touchPoint.y < yMax) return true;
        else return false;
    }
    public PhoneAtti GetPhoneAtti()//获取手机粗略姿态，用于旋转UI
    {
        Vector3 gravity = Input.gyro.gravity;
        if (-1 <= gravity.y && gravity.y < -0.5)//Home在右
        {
            return PhoneAtti.r;
        }
        else if (1 >= gravity.y && gravity.y > 0.5)//Home在左
        {
            return PhoneAtti.l;
        }
        else if (-1 <= gravity.x && gravity.x < -0.5)//Home在上（反拿竖）
        {
            return PhoneAtti.u;
        }
        else if (1 >= gravity.x && gravity.x > 0.5)//Home在下（正拿竖）
        {
            return PhoneAtti.d;
        }
        else return PhoneAtti.N;
    }
    public bool FadeIn(CanvasGroup m_canvasGroup, float fadeTime)
    {
        m_canvasGroup.alpha += Time.fixedDeltaTime / fadeTime;
        if (m_canvasGroup.alpha >= 1)
        {
            m_canvasGroup.interactable = true;
            //m_canvasGroup.blocksRaycasts = true;
            return true;
        }
        else return false;
    }
    public bool FadeOut(CanvasGroup m_canvasGroup, float fadeTime)
    {
        m_canvasGroup.alpha -= Time.fixedDeltaTime / fadeTime;
        if (m_canvasGroup.alpha <= 0)
        {
            m_canvasGroup.interactable = false;
            //m_canvasGroup.blocksRaycasts = false;
            return true;
        }
        else return false;
    }

    static public bool FadeInAn(CanvasGroup canvasGroup, float fadeTime)
    {
        if (canvasGroup.gameObject.GetComponent<Animator>() != null)
        {
            Animator animator = canvasGroup.gameObject.GetComponent<Animator>();
            animator.speed = 1 / fadeTime;
            animator.Play("FadeIn");
            return true;
        }
        else return false;
    }static public bool FadeOutAn(CanvasGroup canvasGroup, float fadeTime)
    {
        if (canvasGroup.gameObject.GetComponent<Animator>() != null)
        {
            Animator animator = canvasGroup.gameObject.GetComponent<Animator>();
            animator.speed = 1 / fadeTime;
            animator.Play("FadeOut");
            return true;
        }
        else return false;
    }
    static public void enableCanavasGroup(CanvasGroup group, bool interactable = true)
    {
        group.alpha = 1;
        if (interactable)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
    }
    static public void disableCanavasGroup(CanvasGroup group, bool interactable = true)
    {
        group.alpha = 1;
        if (interactable)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
    
}
