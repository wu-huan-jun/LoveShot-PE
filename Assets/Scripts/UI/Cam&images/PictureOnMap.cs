using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PictureOnMap : UIManager//地图上的照片缩略图
{
    public Vector3 photoPos;
    public Quaternion photoRot;
    [SerializeField] Image image_object;//本prefab下挂的图片
    [SerializeField] RectTransform image_icon_t;//本prefab本体（带小三角的正方形）

    [Header("Scales缩放")]
    public static float scale;//缩略图的缩放

    [Header("通过文件地址自动实例化小地图缩略图")]
    public int index;//照片序号，和这张照片的文件名、exif的文件名一致
    public bool useFilePath;//是否使用文件地址刷新这张照片，玩家拍摄照片时自动实例化这个prefab时会启用此项
    //[SerializeField] string imageSourcePath;//文件地址
    [SerializeField] Texture2D imageSource;//地址指向的照片

    [Header("实例化全屏照片")]
    [SerializeField] GameObject FullScreenPhotoPannel;//UI下的面板
    [SerializeField] PictureFullScreen pictureFullScreenPrefab;//全屏照片prefab
    public float aspectRatio;//照片长宽比

    // Start is called before the first frame update
    void Start()
    {
        if (useFilePath)
        {
            string path = PictureInstacer.GetAlbumPath();
            path = Path.Combine(path, "P"); //"/DCIM/LoveShot/SaveN/P"
            path += index.ToString(); //"/DCIM/LoveShot/SaveN/Pn"
            var imageData = SaveData.loadFromJson<ImageData>(path + ".exif");
            imageSource = new Texture2D(imageData.pixelX, imageData.pixelY);
            imageSource.LoadImage(getImageByte(path+".jpg"));
            photoPos = imageData.pos;
            photoRot = imageData.rot;
            aspectRatio = 1.0f*imageData.pixelX / imageData.pixelY;
            Debug.Log("长宽比：" + imageData.pixelX.ToString()+"/"+ imageData.pixelY.ToString()+"="+ aspectRatio.ToString());
            image_object.GetComponent<RectTransform>().localScale = new Vector3(.007f, .002275f, .00175f);
            image_object.GetComponent<RectTransform>().localPosition = new Vector3(0,.75f, 0);
        }
        FullScreenPhotoPannel = GameObject.Find("FullScreenPhotos");
        image_icon_t = GetComponent<RectTransform>();
        image_object.sprite = Sprite.Create(imageSource, new Rect(0, 0, imageSource.width, imageSource.height), new Vector2(.5f, .5f));
    }
    public void OnSelect()
    {
        pictureFullScreenPrefab.aspectRatio = aspectRatio;
        pictureFullScreenPrefab.photoRot = photoRot;
        pictureFullScreenPrefab.photoPos = photoPos;
        pictureFullScreenPrefab.imageSource = imageSource;
        Instantiate(pictureFullScreenPrefab.gameObject, FullScreenPhotoPannel.transform);
    }
    private static byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }
    private void Scales()
    {
        image_icon_t.localScale = new Vector3(0.033f / scale, 0.033f / scale, 0.033f / scale);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Scales();
    }
}
