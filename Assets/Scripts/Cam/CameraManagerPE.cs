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
    [SerializeField] VolumeProfile m_Profile;//后处理配置文件
    [SerializeField] Camera CamView;//屏幕相机
    [SerializeField] Camera RenderCam;//渲染相机

    [Header("CamRot&IK")]
    public Vector3 camRot;//处理后的手机旋转
    [SerializeField] Transform rightHandIKPos;//右手IK位置
    [SerializeField] PlayerCharacterPE player;
    public float orientalYAtti;//拿起相机时的初始Y输入
    public float SpeedY;//蹲起速度
    public float PosY;//相机的上下位移

    [Header("\nCamUI")]
    [SerializeField] CameraUIPE cameraUI;
    public float CamRectPosL = 102;
    public float CamRectPosR = 132;//相机实时画面的中心在UI上，距离屏幕左侧和右侧距离占比
    public Vector2 CamRectSize = Vector2.zero;//相机在屏幕上显示的像素数
    [SerializeField] MyButton UpButton;
    [SerializeField] MyButton DownButton;//蹲起按钮

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
    public float apertureMuti = 1;//pp的虚化量似乎和分别率有关
    //[SerializeField] float shotSpeed = 100;//快门速度，用于换算动态模糊

    [Header("FocusPoint焦点与对焦")]
    [SerializeField] float focusDistance = 3;
    public Vector3 focusAngle = Vector3.zero;//对焦点选择转换出的相对于相机正前方的角度
    public bool focusOn;//对焦成功提示,可以被CameraUI修改
    [SerializeField] Transform focusOrientation;//对焦射线朝向
    
    [Header("\nImage OutPut照片生成")]
    public int pixelX = 4000;//cmos像素宽度
    public int pixelY = 3000;//cmos像素高度
    public bool Raw_outPut;//是否输出raw
    [SerializeField] Vector3 gravity;
    public int photographSerial;//照片计数


    void Start()
    {
        Input.gyro.enabled = true;//打开陀螺仪
        CamRectSize = SetCameraScreenSize();//调整CamView的长宽比
        apertureMuti = CamRectSize.y / 1080;
        rightHandIKPos = transform.parent;
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
        orientalYAtti = RotProcess().eulerAngles.y;
        player.yRotOri = player.yRot;
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

    private void CameraTransform()//陀螺仪控制相机（其实是手IK和玩家mesh）转动
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

        //新的旋转控制！
        Quaternion deviceRotation = RotProcess();
        camRot = Quaternion.Slerp(Quaternion.Euler(camRot), deviceRotation, 700 / OISLevel / focalLength * Time.fixedDeltaTime).eulerAngles;//防抖并转换到eularAngles
        rightHandIKPos.transform.localRotation = Quaternion.Euler(camRot.x, 0, camRot.z);//分离x（俯仰）z（横滚）到手IK的旋转
        rightHandIKPos.localPosition = new Vector3(0, 1.5f + PosY, 0.21f);

    }
    private Quaternion RotProcess()
    {
        Quaternion gyroAttitude = Input.gyro.attitude;//原始数据
        return (Quaternion.Euler(90, 0, 0) * new Quaternion(gyroAttitude.x, gyroAttitude.y, -gyroAttitude.z, -gyroAttitude.w));//转换到unity坐标系的朝向
    }
    public float getYRot()
    {
        return camRot.y - orientalYAtti;
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
        focusOrientation.localRotation = Quaternion.Euler(-focusAngle.y * 0.77f, focusAngle.x * 0.63f, 0);//实测出来的纠正系数，测试发现和屏幕分辨率无关
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
        ApertureControll(apertureMuti);
        if (!focusOn)
        {
            AF();
        }
    }
}
