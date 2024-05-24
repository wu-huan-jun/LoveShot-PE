using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraUIPE : UIManager
{
    [SerializeField] ScreenTouchHandler m_screenTouch;
    [SerializeField] MainManagerPE m_mainManager;
    [SerializeField] CameraManagerPE m_cameraManager;
    [Header("BackGround���Һڱ�&�Ź���")]
    [SerializeField] RectTransform t_background;//���Һڱ�+�Ź�����
    [SerializeField] Vector2 backgroundSize = new Vector2(3104,1125);//�����õ�����ͼ���Һڱ�ͼ��3104*1125��
    public Vector2 CamRectPos = new Vector2(102,132);//����������ľ���ߺ��ұߵı�ֵ
    public Vector2 SideSize;//���ҺڱߵĿ��

    [Header("FocusArea�Խ�Ȧ")]
    [SerializeField] GameObject m_focusAreaUI;//�Խ�Ȧ����
    public Vector2 focusLocalPos = Vector2.zero;//�Խ�Ȧ��ȡ�������е��������
    [Header("FovMultiples����")]
    public float fovMultiplier;//���ű���������Fov�ı�������ʵ��Fov�ǣ�60/FovMultiplier��
    public float lastFovMultiplier;//"֮ǰ��"���ű���
    [SerializeField] TMP_Text fovMultiplierText;
    [SerializeField] CanvasGroup c_fovMultiplierText;
    [SerializeField] bool fadeOut;
    [Header("Ev")]
    [SerializeField] TMP_Text EvText;
    [SerializeField] Slider EvSlider;
    [SerializeField] CanvasGroup c_EvSlider;
    [Header("WhiteSlash���ź�����")]
    [SerializeField] CanvasGroup whiteSlash;
    [SerializeField] float fadeOutTime = 3;
    bool whiteSlash_faded = true;
    

    // Start is called before the first frame update
    void Start()
    {
        GetScreenSize();
        SideSize = SetBackgroundSize();
        whiteSlash_faded = true;
    }

    private Vector2 SetBackgroundSize()//���úڱ�λ��
    {
        t_background.sizeDelta = new Vector2(backgroundSize.x * ( ScreenRes.y/backgroundSize.y), ScreenRes.y);
        t_background.position = new Vector3(ScreenRes.x * CamRectPos.x / (CamRectPos.y + CamRectPos.x), ScreenRes.y/2, 0);//������ڱ߷��ں��ʵ�λ���ϣ�
        return new Vector2(ScreenRes.x * CamRectPos.x / (CamRectPos.y + CamRectPos.x) - ScreenRes.y * 4 / 3 / 2, ScreenRes.x * CamRectPos.x / (CamRectPos.y + CamRectPos.x) + ScreenRes.y * 4 / 3 / 2);//�������Һڱߵ�������
    }
    // Update is called once per frame

    static public float KeepDecimal(float input, int n)//���ڱ���nλС��
    {
        string fn = "F" + n.ToString();
        float a = float.Parse(input.ToString(fn));
        return a;
    }

    private void OnEnable()
    {
        m_focusAreaUI.GetComponent<CanvasGroup>().alpha = 0;
    }
    void Focus()
    {
        m_focusAreaUI.GetComponent<CanvasGroup>().alpha = 1;//��ʾ�Խ�����
        m_focusAreaUI.GetComponent<RectTransform>().position = m_screenTouch.touchPoint;//�Խ������Ƶ�����ĵط�
        focusLocalPos.x = (m_screenTouch.touchPoint.x - SideSize.x) / (ScreenRes.y / 3 * 4 / 2) - 1;
        focusLocalPos.y = (m_screenTouch.touchPoint.y - ScreenRes.y / 2) / (ScreenRes.y);//����Խ�λ��
        m_cameraManager.focusOn = false;
    }
    void Zoom()
    {
        if (ScreenTouchHandler.m_touchState == TouchState.Double)//�����ڻ�Ϊ������˫ָ�����ǲ��޶���ȡ�������
        {
            fadeOut = true;
            c_fovMultiplierText.alpha = 0.8f;
            fovMultiplier = lastFovMultiplier * (m_screenTouch.fingersDisatanceMultiplier * 0.5f + 0.5f);
            fovMultiplier = Mathf.Clamp(fovMultiplier, 0.6f, 15);
            fovMultiplierText.text = KeepDecimal(fovMultiplier, 2).ToString() + "x";//����Ļ�ϴ�����ű���
        }
        if (fovMultiplier != lastFovMultiplier && ScreenTouchHandler.m_touchState != TouchState.Double)
        {
            lastFovMultiplier = fovMultiplier;
            fadeOut = false;
        }
        if (!fadeOut)
        {
            fadeOut = FadeOut(c_fovMultiplierText, 0.3f);
        }
    }
    public void EvControll()
    {

        float value = KeepDecimal(EvSlider.value/10f, 1);
        m_cameraManager.exposureLevel = value;
        m_cameraManager.EV();
        if (value > 0)
        {
            EvText.text ="+"+ value.ToString();
        }
        else
        {
            EvText.text = value.ToString();
        }
    }
    public void WhiteSlash()//���¿��ź������
    {
        enableCanavasGroup(whiteSlash,false);
        whiteSlash_faded = false;
    }
    void FixedUpdate()
    {
        if (!PostureUIMaster.posturePanelMaskEnabled //posture���δ���ã�������posture�ϵ�������Խ���
            && ScreenTouchHandler.m_touchState == TouchState.singleTouch && SideSize.x < m_screenTouch.touchPoint.x && m_screenTouch.touchPoint.x < SideSize.y)//����ȡ�������Ϸ����˵���,
            Focus();
        Zoom();
        if(!whiteSlash_faded && FadeOut(whiteSlash, fadeOutTime))
        {
            whiteSlash_faded = true;
        }
    }
}
