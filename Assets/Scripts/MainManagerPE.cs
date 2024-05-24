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
    [SerializeField] Camera mainCamera;     //����fpv/tpv�����
    [SerializeField] Camera CamView;        //�ֳ����
    [SerializeField] Camera SequenceCam;    //�Ի����
    [SerializeField] Canvas BasicPlayCanvas;//������UI
    [SerializeField] Canvas UDS_UI;         //�Ի�ϵͳUI
    [SerializeField] GameObject map;        //���ͼUI���������
    [SerializeField] GameObject MapPictures;//�����ڴ��ͼ�ϵ���Ƭ
    [SerializeField] GameObject CamViewUI;  //���UI
    [SerializeField] GameObject PostureButton;//Pose��ť
    [SerializeField] Rig TwoHandsIK;        //˫��IK�����λ��
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
            TwoHandsIK.weight = 0;//�����ȫ��������
            map.SetActive(false);//�ص����ͼUI���������
            MapPictures.SetActive(false);
            PostureButton.SetActive(true);
        }
        if (playState == PlayState.camera)
        {
            cameraManager.enabled = true;//�����������
            mainCamera.enabled = false;//�ص��������
            CamView.enabled = true;//���ֳ����
            BasicPlayCanvas.enabled = false;//�ص�����UI
            CamViewUI.SetActive(true);//�����UI
            TwoHandsIK.weight = 1;//��IK׷��
        }
        if(playState == PlayState.map)
        {
            mainCamera.enabled = false;//�ص��������
            BasicPlayCanvas.enabled = false;//�ص�����UI
            MapPictures.SetActive(true);
            map.SetActive(true);//�����UI
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
        if(m_playerState == PlayerState.NULL)//���û�����룬���л���ǰ״̬
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


    static public float ClampNew(float ori, float min, float max, float rate)//ƽ������
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
