using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveScrollInstancer : MonoBehaviour
{
    [SerializeField] GameObject SaveScrollPrefab;//prefab
    [SerializeField] Transform Content;//prefab实例化到的父级


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i <= SaveData.GetSaveData().saveCount; i++)
        {
            string path = SaveData.GetSaveRootPath();
            SaveFileInfo data = SaveData.loadFromJson<SaveFileInfo>(Path.Combine(path, "SaveFile"+i.ToString(), "SaveFileInfo.json"));
            if (!data.deleted)
            {
                SaveScroll saveScroll = SaveScrollPrefab.GetComponent<SaveScroll>();
                saveScroll.inputField.text = data.name;
                saveScroll.DateBar.text = "创建时间:" + data.createDate + " " + data.createTime;
                saveScroll.saveIndex = data.index;
                Instantiate(saveScroll, new Vector3(1132, 720 + (i - 1) * (-300), 0), Quaternion.Euler(0, 0, 0), Content);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
