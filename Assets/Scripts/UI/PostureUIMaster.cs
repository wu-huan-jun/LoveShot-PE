using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostureUIMaster : UIManager
{
    public static bool posturePanelMaskEnabled = false;
    [SerializeField] GameObject posturePanelMask;
    [SerializeField] RectTransform posturePanelSlider;
    [SerializeField] ScreenTouchHandler screenTouchHandler;
    [SerializeField] float sliderPosX;
    [SerializeField] float lerpSpeed = 1;
    float lastSliderPosX;
    [SerializeField] Animator fengling_animator;
    [SerializeField] Animator Face_animator;

    //这应该是第一个需要转的UI
    // Update is called once per frame
    private void Start()
    {
        GetScreenSize();
        posturePanelMask.GetComponent<RectTransform>().sizeDelta = new Vector2(ScreenRes.x / 1.8f, 270);
    }
    public void SetposturePanelMask()
    {
        posturePanelMaskEnabled = !posturePanelMaskEnabled;
        posturePanelMask.SetActive(posturePanelMaskEnabled);
    }
    public void setIdleIndex(int index)
    {
        fengling_animator.SetBool("IdleMode", true);
        fengling_animator.SetFloat("IdleIndex", 0);
        StartCoroutine(runSetIdleIndex(index));
    }
    private IEnumerator runSetIdleIndex(int index)
    {
        yield return new WaitForSeconds(0.5f);//等0.5s
        fengling_animator.SetFloat("IdleIndex", index);
    }
    private void FixedUpdate()
    {
        if (ScreenTouchHandler.m_touchState == TouchState.singleMove && touchInArea(screenTouchHandler.touchPoint,600,600+ ScreenRes.x / 1.8f,ScreenRes.y-15- 275,ScreenRes.y - 15)) 
        {
            sliderPosX += (lastSliderPosX + screenTouchHandler.touchMove.x - lastSliderPosX) *lerpSpeed * Time.fixedDeltaTime;
            sliderPosX = Mathf.Clamp(sliderPosX, -4000, 0);
            posturePanelSlider.localPosition = new Vector3(sliderPosX, posturePanelSlider.localPosition.y, 0f);
        }
        else if(ScreenTouchHandler.m_touchState==TouchState.none)
        {
            lastSliderPosX = sliderPosX;
        }
    }
}
