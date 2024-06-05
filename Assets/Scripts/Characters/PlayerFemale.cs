using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeData
{
    public float xRot;
    public float yRot;
    public Vector3 Pos;
    //只负责存取位置信息，不负责posture
    public PlayerFeData(float xRot, float yRot, Vector3 Pos)
    {
        this.xRot = xRot;
        this.yRot = yRot;
        this.Pos = Pos;
    }
}
public class PlayerFemale : MonoBehaviour//第一和第三人称控制差的不少 就不共用父类了..
{

    public GameObject mesh;//animator+transform
    public Animator animator;

    [Header("Move")]
    public float xRot;
    public float yRot;
    float SpeedX;
    float SpeedZ;
    float Speed;
    [SerializeField] Transform orientation;//TPP视角的“前”
    [SerializeField] Transform orientationX;//TPP视角父级，脚本控制XY旋转
    [SerializeField] Transform mesh_transform;//mesh的transform
    [SerializeField] Rigidbody rb;//mesh的rb
                                  //，因为blender单位长度和Unity不一样，所以要额外给rb加一个正确的重力
    [SerializeField] GameObject vcam;//TPP vcam
    [SerializeField] Transform vcamFollowingTar;//Orientation跟随的目标，一般是spine.003
    [SerializeField] Camera miniMapCam;

    [Header("Input")]
    public VariableJoystick moveController;//摇杆输入
    public VariableJoystick viewController;
    public float rotSpeed = 5;

    [Header("Sens")]
    [SerializeField] float camRotFollowingSpeed = 20;
    [SerializeField] float camPosFollowingSpeed = 20;
    [SerializeField] float speedLerp;

    // Start is called before the first frame update
    void Start()
    {
        //mesh_transform = GetComponent<Transform>();
        //rb = GetComponent<Rigidbody>();
        xRot = 15;
    }
    public void SaveToJson()
    {
        PlayerFeData playerFeData = new PlayerFeData(xRot, yRot, mesh.transform.position);
        SaveData.SaveAtDefaultPath("PlayerFeData.json", playerFeData);
    }
    public void LoadFromJson()
    {
        PlayerFeData playerFeData = SaveData.loadJsonFromDefaultPath<PlayerFeData>("PlayerFeData.json");
        xRot = playerFeData.xRot;
        yRot = playerFeData.yRot;
        mesh.transform.position = playerFeData.Pos;
    }
    private void OnEnable()
    {
        vcam.SetActive(true);
        miniMapCam.enabled = true;
    }
    private void OnDisable()
    {
        vcam.SetActive(false);
        miniMapCam.enabled = false;
        Speed = 0;
        animator.SetFloat("Speed", Speed);
    }
    void MoveControll()
    {
        SpeedX = moveController.Vertical;
        SpeedZ = moveController.Horizontal;

        if (!(Mathf.Abs(moveController.Vertical) < 0.05 && Mathf.Abs(moveController.Horizontal) < 0.05))//无输入时不转mesh
        {
            mesh_transform.forward += ((orientation.forward * SpeedX + orientation.right * SpeedZ)- mesh_transform.forward)*Time.fixedDeltaTime*20;
            animator.SetBool("IdleMode", false);
            animator.SetFloat("IdleIndex", 0);
        }
        if(Speed<0.1&& new Vector2(SpeedX, SpeedZ).sqrMagnitude > 0.5f)//起步
        {
            Speed = new Vector2(SpeedX, SpeedZ).sqrMagnitude;
        }
        else if ( Speed - new Vector2(SpeedX, SpeedZ).sqrMagnitude > 0)//减速时
            Speed = Mathf.Lerp(Speed, new Vector2(SpeedX, SpeedZ).sqrMagnitude,0.1f/speedLerp);
        else//加速时
            Speed = Mathf.Lerp(Speed, new Vector2(SpeedX, SpeedZ).sqrMagnitude,2/speedLerp);
        animator.SetFloat("Speed",Speed);

        //rb.AddForce(new Vector3(0, -10, 0),ForceMode.Acceleration);

    }
    void viewControll()
    {
        xRot -= viewController.Vertical * Time.fixedDeltaTime * 20 * rotSpeed;
        yRot += viewController.Horizontal * Time.fixedDeltaTime * 20 * rotSpeed;
        xRot = Mathf.Clamp(xRot, -5, 90);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);

        //相机跟随
        orientationX.position += (vcamFollowingTar.position - orientationX.position) * Time.deltaTime * camPosFollowingSpeed;
        orientationX.rotation = Quaternion.Lerp(orientationX.rotation, Quaternion.Euler(xRot, yRot, 0),5/camRotFollowingSpeed);
    }
    void FixedUpdate()
    {
        viewControll();
        MoveControll();
    }
}
