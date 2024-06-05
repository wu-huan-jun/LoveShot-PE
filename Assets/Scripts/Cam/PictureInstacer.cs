using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PictureInstacer : MonoBehaviour
{
    //��.exifʵ������Ƭprefab
    [SerializeField] GameObject PictureOnMapPrefab;
    [SerializeField] string path;
    [SerializeField] Transform MapUI;
    
    void Start()
    {
        path = GetAlbumPath();
        Main();
    }
    static public string GetAlbumPath()
    {
#if UNITY_EDITOR
        string path = "Assets/";
#elif UNITY_ANDROID
        string path = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android"))+"/";
#endif
        path += "DCIM/LoveShot/" + "Save" + SaveData.loadJsonFromDefaultPath<SaveFileInfo>("SaveFileInfo.json").index.ToString();//"/DCIM/LoveShot/SaveN"
        return path;
    }
    
    static public void InstanceFromExif(string jsonContent,GameObject PictureOnMapPrefab,Transform MapUI)
    {
        ImageData data = JsonUtility.FromJson<ImageData>(jsonContent);
        Vector3 pos = new Vector3(data.pos.x, -80, data.pos.z);
        PictureOnMap pictureOnMap = PictureOnMapPrefab.GetComponent<PictureOnMap>();
        pictureOnMap.useFilePath = true;
        pictureOnMap.index = data.index;
        //pictureOnMap.aspectRatio = data.pixelX / data.pixelY;

        Instantiate(PictureOnMapPrefab, pos, Quaternion.Euler(90, 0, 0), MapUI);
    }
    private void Main()
    {
        string[] jsonFiles = Directory.GetFiles(path, "*.exif");//��ȡexif
        foreach (string jsonFile in jsonFiles)
        {
            // ��ȡJSON�ļ�����
            string jsonContent = File.ReadAllText(jsonFile);

            // �����л�JSON����
            if(jsonContent != Path.Combine(GetAlbumPath(),"P0.exif"))
            {
                InstanceFromExif(jsonContent, PictureOnMapPrefab,MapUI);
            }
        }
    }
}
