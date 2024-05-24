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

    [SerializeField] MainManagerPE mainManagerPE;
    [SerializeField] GameObject playerMesh;//Xiyue x.x.x
    Transform playerTransform;
    Rigidbody rb;
    [SerializeField] float playerDefRotY;
    [SerializeField] Transform FPVHolder;//FPV在骨骼上跟随的位置
    [SerializeField] GameObject m_vcam;
    Transform FPV_vcam;
    [SerializeField] Animator m_animator;//mesh的animator 
    [SerializeField] Transform target;
    [SerializeField] Transform targetHorizonal;
    [SerializeField] Camera miniMapCam;//小地图相机
    public VariableJoystick moveController;//摇杆输入
    public VariableJoystick viewController;
    public float rotSpeed = 5;

    [Header("Sens")]
    [SerializeField] float camFollowingSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = playerMesh.transform;
        yRot = playerDefRotY;
        rb = playerMesh.GetComponent<Rigidbody>();
        FPV_vcam = m_vcam.GetComponent<Transform>();
    }
    public void SaveToJson()
    {
        PlayerMaleData playerMaleData = new PlayerMaleData(xRot, yRot, playerMesh.transform.position);
        SaveData.SaveAtDefaultPath("PlayerMaleData.json", playerMaleData);
    }
    public void LoadFromJson()
    {
        PlayerMaleData playerMaleData = SaveData.loadJsonFromDefaultPath<PlayerMaleData>("PlayerMaleData.json");
        xRot = playerMaleData.xRot;
        yRot = playerMaleData.yRot;
        playerMesh.transform.position = playerMaleData.Pos;
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
        m_animator.SetFloat("SpeedX", SpeedZ);
        m_animator.SetFloat("SpeedZ", SpeedX);
    }
    void MoveControll()
    {
        SpeedX = Mathf.Lerp(SpeedX, moveController.Vertical *2.7f, Time.fixedDeltaTime * 10);
        SpeedZ = Mathf.Lerp(SpeedZ, moveController.Horizontal * 2f, Time.fixedDeltaTime * 10);
        m_animator.SetFloat("SpeedX", SpeedZ);
        m_animator.SetFloat("SpeedZ", SpeedX);
        //rb.MovePosition(rb.position + m_animator.deltaPosition);//把刚体移动到玩家网格处
    }
    void viewControll()
    {
        xRot -= viewController.Vertical * Time.fixedDeltaTime * 20 * rotSpeed;
        yRot += viewController.Horizontal* Time.fixedDeltaTime * 20 * rotSpeed;
        xRot = Mathf.Clamp(xRot, -90, 90);
        FPVHolder.rotation = Quaternion.Euler(xRot, yRot, 0);//这行是转一个empty
        FPV_vcam.rotation = Quaternion.Slerp(FPV_vcam.rotation, FPVHolder.rotation, camFollowingSpeed * 0.4f * Time.fixedDeltaTime);//真的vcam平滑跟随空物体转
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, Quaternion.Euler(0, yRot, 0), camFollowingSpeed * 0.4f * Time.fixedDeltaTime);//平滑地转玩家mesh
        FPV_vcam.position += (FPVHolder.position - FPV_vcam.position) * camFollowingSpeed * Time.fixedDeltaTime;//相机位置平滑跟随空物体，其实这里用Vector3.MoveTowards也可以但是懒得改了
    }
    void viewControllCam()//shit的一部分 具体内容看CamerManager
    {
        targetHorizonal.position = new Vector3(target.position.x, playerTransform.position.y, target.position.z);
        playerTransform.LookAt(targetHorizonal);
    }
    // Update is called once per frame
    void FixedUpdate()//PC版的输入检测主要用Update防止漏掉快速单点，但PE版的输入大多数是连续的所以可以用FixedUpdate节约性能和减少bug
    {
        MoveControll();
        if(mainManagerPE.playState==PlayState.basic)
            viewControll();
        if (mainManagerPE.playState == PlayState.camera)
            viewControllCam();//相机启用时只转
    }
}
