using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Animations.Rigging;//IK

public enum PlayState {basic,camera,cutscene,dialogue,map};
public enum PlayerState {NULL,male,female};
public enum GameState {play,enterScene,hold};
public class MainManagerData
{
    public PlayState playState;
    public GameState gameState;
    public PlayerState playerState;
}
public class MainManagerPE : MonoBehaviour
{
    public PlayState playState;
    public GameState gameState;
    public PlayerState playerState;

    [SerializeField] CameraManagerPE cameraManager;
    [SerializeField] Camera mainCamera;     //场景fpv/tpv主相机
    [SerializeField] Camera CamView;        //手持相机
    [SerializeField] Camera SequenceCam;    //对话相机
    [SerializeField] Canvas BasicPlayCanvas;//主界面UI
    [SerializeField] Canvas UDS_UI;         //对话系统UI
    [SerializeField] GameObject map;        //大地图UI及其控制器
    [SerializeField] GameObject MapPictures;//浮动在大地图上的照片
    [SerializeField] GameObject CamViewUI;  //相机UI
    [SerializeField] GameObject PostureButton;//Pose按钮
    [SerializeField] Rig TwoHandsIK;        //双手IK到相机位置
    public PlayerCharacterPE playerMale;
    public PlayerFemale playerFemale;


    // Start is called before the first frame update
    void Start()
    {
        SwitchState(PlayState.basic);
        SwtichPlayer(PlayerState.male);
    }
    public void SaveToJson()
    {
        SaveData.SaveAtDefaultPath("MainManagerData.json", this);
    }
    public void LoadFromJson()
    {
        MainManagerData data = SaveData.loadJsonFromDefaultPath<MainManagerData>("MainManagerData.json");
        playerState = data.playerState;
        gameState = data.gameState;
        playState = data.playState;
        SwitchState(playState);
        SwtichPlayer(playerState);
    }
    public void BasicPlayToCamButton()
    {
        SwitchState(PlayState.camera);
    }
    public void ToBasicPlayButton()
    {
        SwitchState(PlayState.basic);
    }
    public void BasicToMapButton()
    {
        SwitchState(PlayState.map);
    }
    public PlayState SwitchState(PlayState m_playState)
    {
        playState = m_playState;
        if (playState == PlayState.basic)
        {
            cameraManager.enabled=false;
            mainCamera.enabled = transform;
            CamView.enabled = false;
            BasicPlayCanvas.enabled = true;
            CamViewUI.SetActive(false);
            TwoHandsIK.weight = 0;//和相机全部反操作
            map.SetActive(false);//关掉大地图UI及其控制器
            MapPictures.SetActive(false);
            PostureButton.SetActive(true);
        }
        if (playState == PlayState.camera)
        {
            cameraManager.enabled = true;//启用相机管理
            mainCamera.enabled = false;//关掉场景相机
            CamView.enabled = true;//打开手持相机
            BasicPlayCanvas.enabled = false;//关掉场景UI
            CamViewUI.SetActive(true);//打开相机UI
            TwoHandsIK.weight = 1;//打开IK追踪
        }
        if(playState == PlayState.map)
        {
            mainCamera.enabled = false;//关掉场景相机
            BasicPlayCanvas.enabled = false;//关掉场景UI
            MapPictures.SetActive(true);
            map.SetActive(true);//打开相机UI
            PostureButton.SetActive(false);
        }
        return playState;
    }
    public void SwitchPlayerButton()
    {
        SwtichPlayer(PlayerState.NULL);
    }
    public void SwtichPlayer(PlayerState m_playerState = PlayerState.NULL)
    {
        if(m_playerState == PlayerState.NULL)//如果没有输入，就切换当前状态
        {
            if (playerState == PlayerState.male)
                playerState = PlayerState.female;
            else
                playerState = PlayerState.male;
        }
        else
            playerState = m_playerState;
        playerMale.enabled = (playerState == PlayerState.male);
        playerFemale.enabled = (playerState == PlayerState.female);
    }


    static public float ClampNew(float ori, float min, float max, float rate)//平滑限制
    {
        if (ori > max)
            return ori + (max - ori) * rate;
        else if (ori < min)
            return ori + (min - ori) * rate;
        else
            return ori;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
