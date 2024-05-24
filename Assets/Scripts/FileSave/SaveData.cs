using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static void SaveAtDefaultPath(string fileName,object data)//将json存在默认路径
    {
        FileSaveData fileSaveData = GetSaveData();
        var json = JsonUtility.ToJson(data);
        var path = Path.Combine(Application.persistentDataPath,"SaveLogs","SaveFile"+ fileSaveData.liveSaveIndex.ToString(),fileName);
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        Debug.Log($"保存存档文件至{path}");
#endif
    }
    public static void SaveByJson(string filePath, object data)//直接将json存在指定完整路径（含后缀）
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
#if UNITY_EDITOR
        Debug.Log($"保存存档文件至{filePath}");
#endif
    }
    public static T loadJsonFromDefaultPath<T>(string fileName)//从默认路径读取json
    {
        FileSaveData fileSaveData = GetSaveData();
        var path = Path.Combine(Application.persistentDataPath, "SaveLogs", "SaveFile" + fileSaveData.liveSaveIndex.ToString(), fileName);
        var json = File.ReadAllText(path);
        Debug.Log($"从{path}读取存档文件");
        return JsonUtility.FromJson<T>(json);
    }
    public static T loadFromJson<T>(string filePath)//从指定完整路径读取json
    {
        var json = File.ReadAllText(filePath);
        Debug.Log($"从{filePath}读取存档文件");
        return JsonUtility.FromJson<T>(json);
    }

    public static FileSaveData GetSaveData()//读取总存档文件
    {
        string filePath = Path.Combine(Application.persistentDataPath,"SaveLogs","FileSaveManager.json");
        return loadFromJson<FileSaveData>(filePath);
    }
    public static void OverWriteSaveData(FileSaveData data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "SaveLogs", "FileSaveManager.json");
        SaveByJson(filePath, data);
    }
    public static string GetSaveRootPath()//返回FileSaveManager所在的位置（根位置）
    {
        return Path.Combine(Application.persistentDataPath, "SaveLogs");
    }
    public static string GetLiveSavePath()//返回活动存档所在的位置
    {
        int liveSaveIndex = GetSaveData().liveSaveIndex;
        return Path.Combine(Application.persistentDataPath, "SaveLogs", "SaveFile" + liveSaveIndex.ToString());
    }

    public static void DeleteSaveFile(string filePath)//删除存档Json
    {
        File.Delete(File.ReadAllText(Path.Combine(Application.persistentDataPath, filePath)));
    }
    public static void CreateDirectory(string path)//在指定位置新建文件夹
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("文件夹创建成功：" + path);
        }
        else
        {
            Debug.LogWarning("文件夹已存在：" + path);
        }
    }
}
