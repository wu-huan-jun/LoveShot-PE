using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CamData
{
    public int photoCount;
    public CamData(int photoCount)
    {
        this.photoCount = photoCount;
    }
};
public class CameraManagerPE : MonoBehaviour//相机控制脚本 PE代表手机版（pocket edition）,与不带后缀的电脑版区分开
{
    [SerializeField] MainManagerPE mainManager;
    [Header("\nCam & Postprocessing")]
    [SerializeField] Quaternion attitude;//相机的朝向
    [SerializeField] Vector3 attitudeRaw;//设备在空间中的朝向
    [SerializeField] MyButton UpButton;
    [SerializeField] MyButton DownButton;//蹲起按钮
    public float SpeedY;//蹲起速度
    public float PosY;//相机的上下位移
    [SerializeField] VolumeProfile m_Profile;//后处理配置文件
    [SerializeField] Camera CamView;//屏幕相机
    [SerializeField] Camera RenderCam;//渲染相机
    [SerializeField] Transform t_camera;
    [SerializeField] CamImageCreate imageCreate;

    [Header("CamRot&IK")]
    [SerializeField] Vector3 CamRot;//相机三轴旋转（由手机陀螺仪提供）
    [SerializeField] Transform rightHandIK;//右手IK位置
    [SerializeField] Transform rightHandIKParentHolder;//在chest下随角色移动和旋转
    [SerializeField] Transform RightHandIKParentParent;//parent的父级，负责在进入相机时初始化相机朝向为玩家方向
    [SerializeField] float orientation;//进入相机时玩家的Y旋转
    [SerializeField] PlayerCharacterPE player;
    [SerializeField] Transform rightHandIkParent;//跟随Holder的位置但不跟随旋转，接受手机陀螺仪的旋转
    [SerializeField] Transform rightHandIkTarget;//绑在parent下，IK跟随IKtarget

    [Header("\nCamUI")]
    [SerializeField] CameraUIPE cameraUI;
    public float CamRectPosL = 102;
    public float CamRectPosR = 132;//相机实时画面的中心在UI上，距离屏幕左侧和右侧距离占比
    [SerializeField] Vector2 CamRectSize = Vector2.zero;//相机在屏幕上显示的像素数

    [Header("Lens Component镜头参数")]
    public float minFocusDistance = 0.35f;//最近对焦距离（m）
    public float maxFocusDistance = 1000f;
    public float minAperture = 4;
    public float maxAperture = 22;//光圈范围

    [Header("Camera Body Components机身参数")]//PE版其实没必要留换机身的功能，无所谓了
    public float focusSpeed = 2;//对焦速度
    public bool faceFocus = false;//人脸对焦（暂时还没做qwq）
    [SerializeField] float OISLevel=1;//防抖等级

    [Header("Exposure Components曝光要素")]
    public ExposureMode exposureMode = ExposureMode.Auto;
    public enum ExposureMode { Auto, P, S, A };//曝光模式
    public float exposureLevel;//曝光补偿（EV）
    public float Fov = 60;//用Fov反算焦距
    public float focalLength;//焦距
    public float aperture = 4;//实际光圈
    //[SerializeField] float shotSpeed = 100;//快门速度，用于换算动态模糊

    [Header("FocusPoint焦点与对焦")]
    [SerializeField] float focusDistance = 3;
    public Vector3 focusAngle = Vector3.zero;//对焦点选择转换出的相对于相机正前方的角度
    public bool focusOn;//对焦成功提示,可以被CameraUI修改
    [SerializeField] Transform focusOrientation;//对焦射线朝向
    
    [Header("\nImage OutPut照片生成")]
    public int pixelX = 5184;//cmos像素宽度
    public int pixelY = 3888;//cmos像素高度
    public bool Raw_outPut;//是否输出raw
    [SerializeField] Vector3 gravity;
    public int photographSerial;//照片计数


    void Start()
    {
        t_camera = CamView.GetComponent<Transform>();
        Input.gyro.enabled = true;//打开陀螺仪
        CamRectSize = SetCameraScreenSize();//调整CamView的长宽比
    }
    public void SaveToJson()
    {
        CamData data= new CamData(photographSerial);
        SaveData.SaveAtDefaultPath("CamData.json", data);
    }
    public void LoadFromJson()
    {
        photographSerial = SaveData.loadJsonFromDefaultPath<CamData>("CamData.json").photoCount;
    }
    private void OnEnable()
    {
        orientation = player.yRot;
        RightHandIKParentParent.localRotation = Quaternion.Euler(90, orientation, 0);
    }
    private void OnDisable()
    {
    }
    private Vector2 SetCameraScreenSize()//调整Camview的大小以适配屏幕空间
    {
#if UNITY_EDITOR
        Vector2 ScreenRes = UnityEditor.Handles.GetMainGameViewSize();//在编辑器中返回Gameview的大小
#else
            Vector2 ScreenRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);//在设备上返回屏幕的大小
#endif
        CamView.rect = new Rect(CamRectPosL/(CamRectPosL+CamRectPosR)-ScreenRes.y*4/3/2/ScreenRes.x, 0, ScreenRes.y * 4 / 3 / ScreenRes.x, 1);
        return new Vector2(ScreenRes.y * 4 / 3, ScreenRes.y);
    }

    private Quaternion CameraTransform()//陀螺仪控制相机（其实是手IK和玩家mesh）转动
    {
        //位置控制
        if (UpButton.pressed)
        {
            PosY += SpeedY * Time.fixedDeltaTime;
        }
        else if (DownButton.pressed)
        {
            PosY -= SpeedY * Time.fixedDeltaTime;
        }
        PosY = MainManagerPE.ClampNew(PosY, -0.6f, 0.3f, Time.fixedDeltaTime * 10);

        //旋转控制
        //我承认这个是shit 但是考虑到手机陀螺仪的奇怪输入就先放在这吧(能用就行，绷)
        attitudeRaw = Input.gyro.attitude.eulerAngles;
        if (attitudeRaw.x > 350)
        {
            attitudeRaw.y = Mathf.Clamp(attitudeRaw.y, 10, 350);
        }
        if (attitudeRaw.y > 350)
        {
            attitudeRaw.x = Mathf.Clamp(attitudeRaw.x, 10, 350);
        }
        attitude = Quaternion.Slerp(attitude,Quaternion.Euler(attitudeRaw),700/OISLevel/focalLength*Time.fixedDeltaTime);//获取手机在空间中的朝向并应用防抖
        rightHandIkParent.localRotation = attitude;
        rightHandIkParent.position = rightHandIKParentHolder.position + new Vector3(0,PosY,0);
        rightHandIK.position += (rightHandIkTarget.position-rightHandIK.position) * 4f * Time.fixedDeltaTime;
        rightHandIK.rotation = rightHandIkTarget.rotation;
        return rightHandIK.rotation;
        /*
         * 简单地说一下这坨屎山是什么
         * 主要原因是我搞不懂手机陀螺仪输入的角度的一些复杂旋转……
         * 
         * 现在的写法是：Player的骨骼上有一个right Hand IK Parent Holder,仅跟随玩家动画进行变换
         * 玩家prefab下放right Hand IK Parent,因为prefab不转所以这个parent不会跟随玩家旋转
         * Parent的父级RightHandIK Parent带有一个（90，Orientation，0）的旋转。 将陀螺仪输入值插值应用为parent的localRot
         * Parent下有IKTarget和CamManager
         * 其中，IK Target带有一个相对parent的恒定旋转角，真正的右手IK会在角度和位置上跟随这个IK Target
         * CamManager就是手持相机，也带有一个恒定旋转角（180，180，0），来使相机的旋转正确
         * 沿着Cam视线20m处放了一个PlayerCamviewTarget（跟随相机旋转），PlayerCharacter脚本里会在这个空物体投影到玩家所在xz平面处放置PlayerCamviewTargetHorizontal
         * 然后让玩家lookAt这个PlayerCamviewTargetHorizontal，来实现玩家的旋转……
         * 后面应该得把陀螺仪输入值搞明白之后，通过某个变换转成x俯仰y航向z横滚，这样就可以简单很多了
         * 但是在这之前，先让这坨屎山顶一顶
         * （反正好像也没多几行代码？）
         */
    }

    private void ApertureControll(float muti)//虚化控制
    {
        DepthOfField depthOfField;
        if (m_Profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.aperture.Override(aperture / muti);
            depthOfField.focalLength.Override(focalLength);
        }
    }
    private void FovControll()//焦距
    {
        Fov = 60 / cameraUI.fovMultiplier;
        CamView.fieldOfView = Fov;
        focalLength = Mathf.Abs(18 / (Mathf.Tan(0.5f * Fov * 3.141f / 180)));
    }
    private void AF()//自动对焦
    {
        focusAngle = new Vector3((Mathf.Atan(cameraUI.focusLocalPos.x) * 4 / 3.14159f/*弧度制的45°*/)/*距离中心的角度比例*/, Mathf.Atan(cameraUI.focusLocalPos.y) * 4 / 3.14159f, 0);
        focusAngle *= CamView.fieldOfView;
        focusOrientation.localRotation = Quaternion.Euler(focusAngle.y * 0.8f, -focusAngle.x * 0.65f, 0);//0.8和0.65是实测出来的纠正系数，测试发现和屏幕分辨率无关
        Ray ray = new Ray(CamView.GetComponent<Transform>().position, focusOrientation.forward);//发出测距线
        RaycastHit hit;
        Physics.Raycast(ray, out hit);//用raycast探测对焦距离
        if (focusDistance == 0)
        {
            focusDistance = 10000;
        }
        else
        {
            focusDistance += (hit.distance - focusDistance) * focusSpeed * Time.fixedDeltaTime;//对焦
            if (Mathf.Abs(focusDistance - hit.distance) < 0.05)
                focusOn = true;
            else
                focusOn = false;
        }
        focusDistance = Mathf.Clamp(focusDistance, minFocusDistance, maxFocusDistance);//对焦距离限制
        DepthOfField depthOfField;//将对焦数据赋予Postprocess
        if (m_Profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.focusDistance.Override(focusDistance);
        }
    }
    public void EV()
    {
        ColorAdjustments color;
        if (m_Profile.TryGet<ColorAdjustments>(out color))
        {
            color.postExposure.Override(exposureLevel);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        CameraTransform();
        FovControll();
        ApertureControll(1);
        if (!focusOn)
        {
            AF();
        }
    }
}
