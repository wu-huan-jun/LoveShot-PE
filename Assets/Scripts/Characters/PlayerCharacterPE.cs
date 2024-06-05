using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaleData
{
    public float xRot;
    public float yRot;
    public Vector3 Pos;
    public PlayerMaleData(float xRot, float yRot, Vector3 Pos)
    {
        this.xRot = xRot;
        this.yRot = yRot;
        this.Pos = Pos;
    }
};
public class PlayerCharacterPE : MonoBehaviour
{
    float SpeedX;
    float SpeedZ;

    public float xRot;
    public float yRot;
    public float yRotOri;

    [SerializeField] MainManagerPE mainManagerPE;
    [SerializeField] CameraManagerPE cameraManager;
    public GameObject mesh;//animator+transform
    public Animator animator;
    Transform playerTransform;
    Rigidbody rb;
    [SerializeField] float playerDefRotY;//������ʱ��yRot
    [SerializeField] Transform FPVHolder;//FPV�ڹ����ϸ����λ��
    [SerializeField] GameObject m_vcam;
    Transform FPV_vcam;
    [SerializeField] Camera miniMapCam;//С��ͼ���
    public VariableJoystick moveController;//ҡ������
    public VariableJoystick viewController;

    [Header("Sens")]
    public float camFollowingSpeed = 20;
    public float rotSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = mesh.transform;
        animator = mesh.GetComponent<Animator>();
        yRot = playerDefRotY;
        rb = mesh.GetComponent<Rigidbody>();
        FPV_vcam = m_vcam.GetComponent<Transform>();
    }
    public void SaveToJson()
    {
        PlayerMaleData playerMaleData = new PlayerMaleData(xRot, yRot, mesh.transform.position);
        SaveData.SaveAtDefaultPath("PlayerMaleData.json", playerMaleData);
    }
    public void LoadFromJson()
    {
        PlayerMaleData playerMaleData = SaveData.loadJsonFromDefaultPath<PlayerMaleData>("PlayerMaleData.json");
        xRot = playerMaleData.xRot;
        yRot = playerMaleData.yRot;
        mesh.transform.position = playerMaleData.Pos;
    }
    private void OnEnable()
    {
        m_vcam.SetActive(true);
        miniMapCam.enabled = true;
    }
    private void OnDisable()
    {
        m_vcam.SetActive(false);
        miniMapCam.enabled = false;
        SpeedX = 0;
        SpeedZ = 0;
        animator.SetFloat("SpeedX", SpeedZ);
        animator.SetFloat("SpeedZ", SpeedX);
    }
    void MoveControll()
    {
        SpeedX = Mathf.Lerp(SpeedX, moveController.Vertical *2.7f, Time.fixedDeltaTime * 10);
        SpeedZ = Mathf.Lerp(SpeedZ, moveController.Horizontal * 2f, Time.fixedDeltaTime * 10);
        animator.SetFloat("SpeedX", SpeedZ);
        animator.SetFloat("SpeedZ", SpeedX);
        //rb.MovePosition(rb.position + animator.deltaPosition);//�Ѹ����ƶ����������
    }
    void viewControll()
    {
        xRot -= viewController.Vertical * Time.fixedDeltaTime * 20 * rotSpeed;
        yRot += viewController.Horizontal* Time.fixedDeltaTime * 20 * rotSpeed;
        xRot = Mathf.Clamp(xRot, -90, 90);
        FPVHolder.rotation = Quaternion.Euler(xRot, yRot, 0);//������תһ��empty
        FPV_vcam.rotation = Quaternion.Slerp(FPV_vcam.rotation, FPVHolder.rotation, camFollowingSpeed * 0.4f * Time.fixedDeltaTime);//���vcamƽ�����������ת
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, Quaternion.Euler(0, yRot, 0), camFollowingSpeed * 0.4f * Time.fixedDeltaTime);//ƽ����ת���mesh
        FPV_vcam.position += (FPVHolder.position - FPV_vcam.position) * camFollowingSpeed * Time.fixedDeltaTime;//���λ��ƽ����������壬��ʵ������Vector3.MoveTowardsҲ���Ե������ø���
    }
    void viewControllCam()
    {
        yRot = yRotOri + cameraManager.getYRot();
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, Quaternion.Euler(0, yRot, 0), camFollowingSpeed * 0.4f * Time.fixedDeltaTime);
    }
    // Update is called once per frame
    void FixedUpdate()//PC�����������Ҫ��Update��ֹ©�����ٵ��㣬��PE����������������������Կ�����FixedUpdate��Լ���ܺͼ���bug
    {
        if (mainManagerPE.playState == PlayState.basic)
        { MoveControll(); viewControll(); }

        if (mainManagerPE.playState == PlayState.camera)
            viewControllCam();
    }
}
