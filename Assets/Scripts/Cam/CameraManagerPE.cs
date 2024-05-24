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
public class CameraManagerPE : MonoBehaviour//������ƽű� PE�����ֻ��棨pocket edition��,�벻����׺�ĵ��԰����ֿ�
{
    [SerializeField] MainManagerPE mainManager;
    [Header("\nCam & Postprocessing")]
    [SerializeField] Quaternion attitude;//����ĳ���
    [SerializeField] Vector3 attitudeRaw;//�豸�ڿռ��еĳ���
    [SerializeField] MyButton UpButton;
    [SerializeField] MyButton DownButton;//����ť
    public float SpeedY;//�����ٶ�
    public float PosY;//���������λ��
    [SerializeField] VolumeProfile m_Profile;//���������ļ�
    [SerializeField] Camera CamView;//��Ļ���
    [SerializeField] Camera RenderCam;//��Ⱦ���
    [SerializeField] Transform t_camera;
    [SerializeField] CamImageCreate imageCreate;

    [Header("CamRot&IK")]
    [SerializeField] Vector3 CamRot;//���������ת�����ֻ��������ṩ��
    [SerializeField] Transform rightHandIK;//����IKλ��
    [SerializeField] Transform rightHandIKParentHolder;//��chest�����ɫ�ƶ�����ת
    [SerializeField] Transform RightHandIKParentParent;//parent�ĸ����������ڽ������ʱ��ʼ���������Ϊ��ҷ���
    [SerializeField] float orientation;//�������ʱ��ҵ�Y��ת
    [SerializeField] PlayerCharacterPE player;
    [SerializeField] Transform rightHandIkParent;//����Holder��λ�õ���������ת�������ֻ������ǵ���ת
    [SerializeField] Transform rightHandIkTarget;//����parent�£�IK����IKtarget

    [Header("\nCamUI")]
    [SerializeField] CameraUIPE cameraUI;
    public float CamRectPosL = 102;
    public float CamRectPosR = 132;//���ʵʱ�����������UI�ϣ�������Ļ�����Ҳ����ռ��
    [SerializeField] Vector2 CamRectSize = Vector2.zero;//�������Ļ����ʾ��������

    [Header("Lens Component��ͷ����")]
    public float minFocusDistance = 0.35f;//����Խ����루m��
    public float maxFocusDistance = 1000f;
    public float minAperture = 4;
    public float maxAperture = 22;//��Ȧ��Χ

    [Header("Camera Body Components��������")]//PE����ʵû��Ҫ���������Ĺ��ܣ�����ν��
    public float focusSpeed = 2;//�Խ��ٶ�
    public bool faceFocus = false;//�����Խ�����ʱ��û��qwq��
    [SerializeField] float OISLevel=1;//�����ȼ�

    [Header("Exposure Components�ع�Ҫ��")]
    public ExposureMode exposureMode = ExposureMode.Auto;
    public enum ExposureMode { Auto, P, S, A };//�ع�ģʽ
    public float exposureLevel;//�عⲹ����EV��
    public float Fov = 60;//��Fov���㽹��
    public float focalLength;//����
    public float aperture = 4;//ʵ�ʹ�Ȧ
    //[SerializeField] float shotSpeed = 100;//�����ٶȣ����ڻ��㶯̬ģ��

    [Header("FocusPoint������Խ�")]
    [SerializeField] float focusDistance = 3;
    public Vector3 focusAngle = Vector3.zero;//�Խ���ѡ��ת����������������ǰ���ĽǶ�
    public bool focusOn;//�Խ��ɹ���ʾ,���Ա�CameraUI�޸�
    [SerializeField] Transform focusOrientation;//�Խ����߳���
    
    [Header("\nImage OutPut��Ƭ����")]
    public int pixelX = 5184;//cmos���ؿ���
    public int pixelY = 3888;//cmos���ظ߶�
    public bool Raw_outPut;//�Ƿ����raw
    [SerializeField] Vector3 gravity;
    public int photographSerial;//��Ƭ����


    void Start()
    {
        t_camera = CamView.GetComponent<Transform>();
        Input.gyro.enabled = true;//��������
        CamRectSize = SetCameraScreenSize();//����CamView�ĳ�����
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
    private Vector2 SetCameraScreenSize()//����Camview�Ĵ�С��������Ļ�ռ�
    {
#if UNITY_EDITOR
        Vector2 ScreenRes = UnityEditor.Handles.GetMainGameViewSize();//�ڱ༭���з���Gameview�Ĵ�С
#else
            Vector2 ScreenRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);//���豸�Ϸ�����Ļ�Ĵ�С
#endif
        CamView.rect = new Rect(CamRectPosL/(CamRectPosL+CamRectPosR)-ScreenRes.y*4/3/2/ScreenRes.x, 0, ScreenRes.y * 4 / 3 / ScreenRes.x, 1);
        return new Vector2(ScreenRes.y * 4 / 3, ScreenRes.y);
    }

    private Quaternion CameraTransform()//�����ǿ����������ʵ����IK�����mesh��ת��
    {
        //λ�ÿ���
        if (UpButton.pressed)
        {
            PosY += SpeedY * Time.fixedDeltaTime;
        }
        else if (DownButton.pressed)
        {
            PosY -= SpeedY * Time.fixedDeltaTime;
        }
        PosY = MainManagerPE.ClampNew(PosY, -0.6f, 0.3f, Time.fixedDeltaTime * 10);

        //��ת����
        //�ҳ��������shit ���ǿ��ǵ��ֻ������ǵ����������ȷ������(���þ��У���)
        attitudeRaw = Input.gyro.attitude.eulerAngles;
        if (attitudeRaw.x > 350)
        {
            attitudeRaw.y = Mathf.Clamp(attitudeRaw.y, 10, 350);
        }
        if (attitudeRaw.y > 350)
        {
            attitudeRaw.x = Mathf.Clamp(attitudeRaw.x, 10, 350);
        }
        attitude = Quaternion.Slerp(attitude,Quaternion.Euler(attitudeRaw),700/OISLevel/focalLength*Time.fixedDeltaTime);//��ȡ�ֻ��ڿռ��еĳ���Ӧ�÷���
        rightHandIkParent.localRotation = attitude;
        rightHandIkParent.position = rightHandIKParentHolder.position + new Vector3(0,PosY,0);
        rightHandIK.position += (rightHandIkTarget.position-rightHandIK.position) * 4f * Time.fixedDeltaTime;
        rightHandIK.rotation = rightHandIkTarget.rotation;
        return rightHandIK.rotation;
        /*
         * �򵥵�˵һ������ʺɽ��ʲô
         * ��Ҫԭ�����Ҹ㲻���ֻ�����������ĽǶȵ�һЩ������ת����
         * 
         * ���ڵ�д���ǣ�Player�Ĺ�������һ��right Hand IK Parent Holder,��������Ҷ������б任
         * ���prefab�·�right Hand IK Parent,��Ϊprefab��ת�������parent������������ת
         * Parent�ĸ���RightHandIK Parent����һ����90��Orientation��0������ת�� ������������ֵ��ֵӦ��Ϊparent��localRot
         * Parent����IKTarget��CamManager
         * ���У�IK Target����һ�����parent�ĺ㶨��ת�ǣ�����������IK���ڽǶȺ�λ���ϸ������IK Target
         * CamManager�����ֳ������Ҳ����һ���㶨��ת�ǣ�180��180��0������ʹ�������ת��ȷ
         * ����Cam����20m������һ��PlayerCamviewTarget�����������ת����PlayerCharacter�ű���������������ͶӰ���������xzƽ�洦����PlayerCamviewTargetHorizontal
         * Ȼ�������lookAt���PlayerCamviewTargetHorizontal����ʵ����ҵ���ת����
         * ����Ӧ�õð�����������ֵ������֮��ͨ��ĳ���任ת��x����y����z����������Ϳ��Լ򵥺ܶ���
         * ��������֮ǰ����������ʺɽ��һ��
         * ����������Ҳû�༸�д��룿��
         */
    }

    private void ApertureControll(float muti)//�黯����
    {
        DepthOfField depthOfField;
        if (m_Profile.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.aperture.Override(aperture / muti);
            depthOfField.focalLength.Override(focalLength);
        }
    }
    private void FovControll()//����
    {
        Fov = 60 / cameraUI.fovMultiplier;
        CamView.fieldOfView = Fov;
        focalLength = Mathf.Abs(18 / (Mathf.Tan(0.5f * Fov * 3.141f / 180)));
    }
    private void AF()//�Զ��Խ�
    {
        focusAngle = new Vector3((Mathf.Atan(cameraUI.focusLocalPos.x) * 4 / 3.14159f/*�����Ƶ�45��*/)/*�������ĵĽǶȱ���*/, Mathf.Atan(cameraUI.focusLocalPos.y) * 4 / 3.14159f, 0);
        focusAngle *= CamView.fieldOfView;
        focusOrientation.localRotation = Quaternion.Euler(focusAngle.y * 0.8f, -focusAngle.x * 0.65f, 0);//0.8��0.65��ʵ������ľ���ϵ�������Է��ֺ���Ļ�ֱ����޹�
        Ray ray = new Ray(CamView.GetComponent<Transform>().position, focusOrientation.forward);//���������
        RaycastHit hit;
        Physics.Raycast(ray, out hit);//��raycast̽��Խ�����
        if (focusDistance == 0)
        {
            focusDistance = 10000;
        }
        else
        {
            focusDistance += (hit.distance - focusDistance) * focusSpeed * Time.fixedDeltaTime;//�Խ�
            if (Mathf.Abs(focusDistance - hit.distance) < 0.05)
                focusOn = true;
            else
                focusOn = false;
        }
        focusDistance = Mathf.Clamp(focusDistance, minFocusDistance, maxFocusDistance);//�Խ���������
        DepthOfField depthOfField;//���Խ����ݸ���Postprocess
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