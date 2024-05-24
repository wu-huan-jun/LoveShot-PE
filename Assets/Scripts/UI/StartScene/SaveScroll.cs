using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveScroll : MonoBehaviour
{
    public InputField inputField;
    public Text DateBar;
    public int saveIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void LoadSave()
    {
        var data = SaveData.GetSaveData();
        data.liveSaveIndex = saveIndex;
        SaveData.OverWriteSaveData(data);//刷新活动存档编号
        var data2 = SaveData.loadJsonFromDefaultPath<SaveFileInfo>("SaveFileInfo.json");
        GameObject.Find("LoadScene").GetComponent<LoadScene>().Load(data2.liveSceneIndex);
    }
    public void OverWriteSaveName()//更改存档命名
    {
        string path = SaveData.GetSaveRootPath();
        path = Path.Combine(path, "SaveFile" + saveIndex, "SaveFileInfo.json");
        var data2 = SaveData.loadFromJson<SaveFileInfo>(path);
        data2.name = inputField.text;
        SaveData.SaveByJson(path, data2);
    }
    public void DeleteSave()
    {
        string path = SaveData.GetSaveRootPath();
        path = Path.Combine(path, "SaveFile" + saveIndex, "SaveFileInfo.json");
        var data2 = SaveData.loadFromJson<SaveFileInfo>(path);
        data2.deleted = true;
        SaveData.SaveByJson(path, data2);//将此存档隐藏

        //刷新活动存档编号到编号0（指向空）
        var data = SaveData.GetSaveData();
        data.liveSaveIndex = 0;
        SaveData.OverWriteSaveData(data);

        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
