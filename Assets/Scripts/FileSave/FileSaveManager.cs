using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]//因为loadFromJson不支持反序列化到MonoBehaviour的子类，所以所有需要储存数据都要新开一个类
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
    public bool initialized;//在第一次进入主场景时，如果这里是false则会新建各个类的存档json
    public int liveSceneIndex;//正在游玩的scene
    public bool deleted;//删除一个存档后，文件夹并不会被删除，而是被隐藏，当这里为true时，SaveScrollInstancer将不会实例化指向这个存档的prefab
    public SaveFileInfo(int index, string name,string date, string Time)//在StartScene中的初始化
    {
        this.index = index;
        this.name = name;
        createDate = date;
        createTime = Time;
        initialized = false;
        liveSceneIndex = 2;
        deleted = false;
    }
    public SaveFileInfo(int index, string name, string date, string Time,int liveSceneIndex)//在MainScene中的初始化
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
    public int saveCount = 0;//存档的总计数
    public int liveSaveIndex = 0;//现在正在游玩的存档序号
    string path = "SaveLogs";//"默认路径"
    string persistentDataPath;//避免反复调用application.persistentDataPath
    StartSceneUI startSceneUI;

    [Header("GameObject")]
    [SerializeField] bool initialized;//这个场景的存档有无被初始化
    [SerializeField] MainManagerPE mainManagerPE;
    [SerializeField] PlayerCharacterPE playerMale;
    [SerializeField] PlayerFemale playerFemale;
    [SerializeField] CameraManagerPE CamViewManager;
    [SerializeField] LoadScene loadScene;

    void Start()
    {
        persistentDataPath = Application.persistentDataPath;
        string pathN = Path.Combine(persistentDataPath, path);//游戏数据根目录+存档文件夹名
        if (!Directory.Exists(pathN))//如果没有找到 总的 存档文件夹
        {
            SaveData.CreateDirectory(pathN);//就建立一个存档文件夹
            SaveData.SaveByJson(Path.Combine(pathN,"FileSaveManager.json"), this);// 并且建立 总的 存档文件，记录存档数和现在live的存档
        }
        else
        {
            Debug.Log(Path.Combine(pathN, "FileSaveManager.json"));
            var loadData = SaveData.loadFromJson<FileSaveData>(Path.Combine(pathN, "FileSaveManager.json"));//加载自己的存档文件
            saveCount = loadData.saveCount;
            liveSaveIndex = loadData.liveSaveIndex;
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            var data = SaveData.loadJsonFromDefaultPath<SaveFileInfo>("SaveFileInfo.json");
            if (!data.initialized)//如果一个新存档还没有存档还未被场景初始化
            {
                Save();
                SaveFileInfo saveFileInfo = new SaveFileInfo(data.index,data.name, data.createDate, data.createTime, 1);//刷新存档状态
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
    public void CreateSaveFile()//用于“新游戏”的创建存档
    {
        saveCount++;
        liveSaveIndex = saveCount;
        string pathN = Path.Combine(persistentDataPath, path, "SaveFile" + liveSaveIndex);
        SaveData.CreateDirectory(pathN);
        SaveData.SaveByJson(Path.Combine(persistentDataPath, path, "FileSaveManager.json"), this);//刷新存档记录

        //保存这个存档的基本信息
        string date = System.DateTime.Now.Date.ToString();
        string time = System.DateTime.Now.ToLongTimeString().ToString();
        SaveFileInfo saveFileInfo = new SaveFileInfo(liveSaveIndex, "存档"+liveSaveIndex,date,time);
        SaveData.SaveAtDefaultPath("SaveFileInfo.json", saveFileInfo);
    }
    public void LoadSaveFile()//“继续游戏”
    {
        int liveSceneIndex = SaveData.loadJsonFromDefaultPath<SaveFileInfo>("SaveFileInfo.Json").liveSceneIndex;
        loadScene.Load(liveSceneIndex);
    }
    
}
