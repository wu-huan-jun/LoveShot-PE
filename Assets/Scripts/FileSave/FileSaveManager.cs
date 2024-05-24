using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]//��ΪloadFromJson��֧�ַ����л���MonoBehaviour�����࣬����������Ҫ�������ݶ�Ҫ�¿�һ����
public class FileSaveData
{
    public int saveCount;
    public int liveSaveIndex;
}
public class SaveFileInfo
{
    public int index;
    public string name;
    public string createDate;
    public string createTime;
    public bool initialized;//�ڵ�һ�ν���������ʱ�����������false����½�������Ĵ浵json
    public int liveSceneIndex;//���������scene
    public bool deleted;//ɾ��һ���浵���ļ��в����ᱻɾ�������Ǳ����أ�������Ϊtrueʱ��SaveScrollInstancer������ʵ����ָ������浵��prefab
    public SaveFileInfo(int index, string name,string date, string Time)//��StartScene�еĳ�ʼ��
    {
        this.index = index;
        this.name = name;
        createDate = date;
        createTime = Time;
        initialized = false;
        liveSceneIndex = 2;
        deleted = false;
    }
    public SaveFileInfo(int index, string name, string date, string Time,int liveSceneIndex)//��MainScene�еĳ�ʼ��
    {
        this.index = index;
        this.name = name;
        createDate = date;
        createTime = Time;
        initialized = true;
        this.liveSceneIndex = liveSceneIndex;
        deleted = false;
    }
}
public class FileSaveManager : MonoBehaviour
{
    public int saveCount = 0;//�浵���ܼ���
    public int liveSaveIndex = 0;//������������Ĵ浵���
    string path = "SaveLogs";//"Ĭ��·��"
    string persistentDataPath;//���ⷴ������application.persistentDataPath
    StartSceneUI startSceneUI;

    [Header("GameObject")]
    [SerializeField] bool initialized;//��������Ĵ浵���ޱ���ʼ��
    [SerializeField] MainManagerPE mainManagerPE;
    [SerializeField] PlayerCharacterPE playerMale;
    [SerializeField] PlayerFemale playerFemale;
    [SerializeField] CameraManagerPE CamViewManager;
    [SerializeField] LoadScene loadScene;

    void Start()
    {
        persistentDataPath = Application.persistentDataPath;
        string pathN = Path.Combine(persistentDataPath, path);//��Ϸ���ݸ�Ŀ¼+�浵�ļ�����
        if (!Directory.Exists(pathN))//���û���ҵ� �ܵ� �浵�ļ���
        {
            SaveData.CreateDirectory(pathN);//�ͽ���һ���浵�ļ���
            SaveData.SaveByJson(Path.Combine(pathN,"FileSaveManager.json"), this);// ���ҽ��� �ܵ� �浵�ļ�����¼�浵��������live�Ĵ浵
        }
        else
        {
            Debug.Log(Path.Combine(pathN, "FileSaveManager.json"));
            var loadData = SaveData.loadFromJson<FileSaveData>(Path.Combine(pathN, "FileSaveManager.json"));//�����Լ��Ĵ浵�ļ�
            saveCount = loadData.saveCount;
            liveSaveIndex = loadData.liveSaveIndex;
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            var data = SaveData.loadJsonFromDefaultPath<SaveFileInfo>("SaveFileInfo.json");
            if (!data.initialized)//���һ���´浵��û�д浵��δ��������ʼ��
            {
                Save();
                SaveFileInfo saveFileInfo = new SaveFileInfo(data.index,data.name, data.createDate, data.createTime, 1);//ˢ�´浵״̬
                SaveData.SaveAtDefaultPath("SaveFileInfo.Json", saveFileInfo);
            }
            else
            {
                Load();
            }
        }
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            startSceneUI = GetComponent<StartSceneUI>();
            startSceneUI.enabled = true;
        }
    }
    public void Load()
    {
        mainManagerPE.LoadFromJson();
        playerMale.LoadFromJson();
        playerFemale.LoadFromJson();
        CamViewManager.LoadFromJson();
    }
    public void Save()
    {
        playerMale.SaveToJson();
        playerFemale.SaveToJson();
        mainManagerPE.SaveToJson();
        CamViewManager.SaveToJson();
    }
    public void CreateSaveFile()//���ڡ�����Ϸ���Ĵ����浵
    {
        saveCount++;
        liveSaveIndex = saveCount;
        string pathN = Path.Combine(persistentDataPath, path, "SaveFile" + liveSaveIndex);
        SaveData.CreateDirectory(pathN);
        SaveData.SaveByJson(Path.Combine(persistentDataPath, path, "FileSaveManager.json"), this);//ˢ�´浵��¼

        //��������浵�Ļ�����Ϣ
        string date = System.DateTime.Now.Date.ToString();
        string time = System.DateTime.Now.ToLongTimeString().ToString();
        SaveFileInfo saveFileInfo = new SaveFileInfo(liveSaveIndex, "�浵"+liveSaveIndex,date,time);
        SaveData.SaveAtDefaultPath("SaveFileInfo.json", saveFileInfo);
    }
    public void LoadSaveFile()//��������Ϸ��
    {
        int liveSceneIndex = SaveData.loadJsonFromDefaultPath<SaveFileInfo>("SaveFileInfo.Json").liveSceneIndex;
        loadScene.Load(liveSceneIndex);
    }
    
}
