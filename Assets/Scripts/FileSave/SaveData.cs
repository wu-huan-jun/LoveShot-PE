using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static void SaveAtDefaultPath(string fileName,object data)//��json����Ĭ��·��
    {
        FileSaveData fileSaveData = GetSaveData();
        var json = JsonUtility.ToJson(data);
        var path = Path.Combine(Application.persistentDataPath,"SaveLogs","SaveFile"+ fileSaveData.liveSaveIndex.ToString(),fileName);
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        Debug.Log($"����浵�ļ���{path}");
#endif
    }
    public static void SaveByJson(string filePath, object data)//ֱ�ӽ�json����ָ������·��������׺��
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
#if UNITY_EDITOR
        Debug.Log($"����浵�ļ���{filePath}");
#endif
    }
    public static T loadJsonFromDefaultPath<T>(string fileName)//��Ĭ��·����ȡjson
    {
        FileSaveData fileSaveData = GetSaveData();
        var path = Path.Combine(Application.persistentDataPath, "SaveLogs", "SaveFile" + fileSaveData.liveSaveIndex.ToString(), fileName);
        var json = File.ReadAllText(path);
        Debug.Log($"��{path}��ȡ�浵�ļ�");
        return JsonUtility.FromJson<T>(json);
    }
    public static T loadFromJson<T>(string filePath)//��ָ������·����ȡjson
    {
        var json = File.ReadAllText(filePath);
        Debug.Log($"��{filePath}��ȡ�浵�ļ�");
        return JsonUtility.FromJson<T>(json);
    }

    public static FileSaveData GetSaveData()//��ȡ�ܴ浵�ļ�
    {
        string filePath = Path.Combine(Application.persistentDataPath,"SaveLogs","FileSaveManager.json");
        return loadFromJson<FileSaveData>(filePath);
    }
    public static void OverWriteSaveData(FileSaveData data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "SaveLogs", "FileSaveManager.json");
        SaveByJson(filePath, data);
    }
    public static string GetSaveRootPath()//����FileSaveManager���ڵ�λ�ã���λ�ã�
    {
        return Path.Combine(Application.persistentDataPath, "SaveLogs");
    }
    public static string GetLiveSavePath()//���ػ�浵���ڵ�λ��
    {
        int liveSaveIndex = GetSaveData().liveSaveIndex;
        return Path.Combine(Application.persistentDataPath, "SaveLogs", "SaveFile" + liveSaveIndex.ToString());
    }

    public static void DeleteSaveFile(string filePath)//ɾ���浵Json
    {
        File.Delete(File.ReadAllText(Path.Combine(Application.persistentDataPath, filePath)));
    }
    public static void CreateDirectory(string path)//��ָ��λ���½��ļ���
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("�ļ��д����ɹ���" + path);
        }
        else
        {
            Debug.LogWarning("�ļ����Ѵ��ڣ�" + path);
        }
    }
}
