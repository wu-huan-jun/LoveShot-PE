using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PictureOnMap : UIManager//��ͼ�ϵ���Ƭ����ͼ
{
    public Vector3 photoPos;
    public Quaternion photoRot;
    [SerializeField] Image image_object;//��prefab�¹ҵ�ͼƬ
    [SerializeField] RectTransform image_icon_t;//��prefab���壨��С���ǵ������Σ�

    [Header("Scales����")]
    public static float scale;//����ͼ������

    [Header("ͨ���ļ���ַ�Զ�ʵ����С��ͼ����ͼ")]
    public int index;//��Ƭ��ţ���������Ƭ���ļ�����exif���ļ���һ��
    public bool useFilePath;//�Ƿ�ʹ���ļ���ַˢ��������Ƭ�����������Ƭʱ�Զ�ʵ�������prefabʱ�����ô���
    //[SerializeField] string imageSourcePath;//�ļ���ַ
    [SerializeField] Texture2D imageSource;//��ַָ�����Ƭ

    [Header("ʵ����ȫ����Ƭ")]
    [SerializeField] GameObject FullScreenPhotoPannel;//UI�µ����
    [SerializeField] PictureFullScreen pictureFullScreenPrefab;//ȫ����Ƭprefab
    public float aspectRatio;//��Ƭ�����

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
            Debug.Log("����ȣ�" + imageData.pixelX.ToString()+"/"+ imageData.pixelY.ToString()+"="+ aspectRatio.ToString());
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
