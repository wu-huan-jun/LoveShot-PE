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
        SaveData.OverWriteSaveData(data);//ˢ�»�浵���
        var data2 = SaveData.loadJsonFromDefaultPath<SaveFileInfo>("SaveFileInfo.json");
        GameObject.Find("LoadScene").GetComponent<LoadScene>().Load(data2.liveSceneIndex);
    }
    public void OverWriteSaveName()//���Ĵ浵����
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
        SaveData.SaveByJson(path, data2);//���˴浵����

        //ˢ�»�浵��ŵ����0��ָ��գ�
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
