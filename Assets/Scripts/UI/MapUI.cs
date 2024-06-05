using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//全屏地图
public class MapUI : UIManager
{
    [SerializeField] GameObject m_mapCam;
    Camera mapCam;
    RectTransform t_mapCam;
    [SerializeField] ScreenTouchHandler screenTouchHandler;

    public float mapMultiplier=1;//缩放倍数，不是MapCam的尺寸
    [SerializeField] float lastMapMultiplier =1;//"之前的"缩放倍数
    [SerializeField] Vector2 mapCamPos;
    Vector2 lastMapCamPos;

    bool fadeout;

    // Start is called before the first frame update
    void Start()
    {
        GetScreenSize();
        mapCam = m_mapCam.GetComponent<Camera>();
        t_mapCam = m_mapCam.GetComponent<RectTransform>();
    }

    void InputHandler()
    {
        //接收屏幕输入
        if (ScreenTouchHandler.m_touchState == TouchState.Double)//复用自CameraUIPE
        {
            mapMultiplier = lastMapMultiplier * (screenTouchHandler.fingersDisatanceMultiplier * 0.5f + 0.5f);
            mapCamPos = lastMapCamPos + new Vector2(screenTouchHandler.entireMove.x / ScreenRes.x, screenTouchHandler.entireMove.y / ScreenRes.y)*0.5f;
        }
        if (ScreenTouchHandler.m_touchState == TouchState.singleMove)
        {
            mapCamPos = lastMapCamPos + new Vector2(screenTouchHandler.touchMove.x / ScreenRes.x, screenTouchHandler.touchMove.y / ScreenRes.y);
        }
        if ((mapMultiplier != lastMapMultiplier || mapCamPos != lastMapCamPos) && ScreenTouchHandler.m_touchState == TouchState.none)//手放开
        {
            lastMapMultiplier = mapMultiplier;
            lastMapCamPos = mapCamPos;
        }
    }
    void ApplyTransform()
    {
        //输入应用到相机变换
        mapMultiplier = MainManagerPE.ClampNew(mapMultiplier, 0.3f, 1.8f, Time.fixedDeltaTime * 10);
        mapCamPos.x = MainManagerPE.ClampNew(mapCamPos.x, 0.8f,1.2f, Time.fixedDeltaTime * 10);
        mapCamPos.y = MainManagerPE.ClampNew(mapCamPos.y, -1.0f, -0.5f, Time.fixedDeltaTime * 10);
        mapCam.orthographicSize = 81 / mapMultiplier;
        t_mapCam.localPosition = new Vector3(mapCamPos.x*4, 30, mapCamPos.y*4);
    }
    private void OnEnable()
    {
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        InputHandler();
        ApplyTransform();
        PictureOnMap.scale = mapMultiplier;
    }
}
