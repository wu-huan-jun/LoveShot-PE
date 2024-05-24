using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ImageData //exif
{
    public int index;
    public int pixelX;
    public int pixelY;
    public Vector3 pos;
    public Quaternion rot;
    public bool isP;
    public ImageData(int index, int  pixelX, int pixelY, Vector3 pos, Quaternion rot,bool isP)
    {
        this.index = index;
        this.pixelY = pixelY;
        this.pixelX = pixelX;
        this.pos = pos;
        this.rot = rot;
        this.isP = isP;
    }
};

//渲染照片，并将照片和exif保存至系统目录下/DCIM/LoveShot/Save{存档代码}/
public class CamImageCreate : MonoBehaviour
{
    [SerializeField] CameraManagerPE CamManager;
    [SerializeField] GameObject m_RenderCam;
    public int photographSerial;
    public int pixelX;
    public int pixelY;//照片尺寸
    public bool Raw_outPut;
    Vector3 gravity;
    Transform t_RenderCam;
    Camera c_RenderCam;
    [SerializeField] Camera CamView;//屏幕相机
    [SerializeField] bool isPortrait;
    [Header("地图缩略图生成")]
    [SerializeField] Transform playerMesh;
    [SerializeField] GameObject imageOnMapPrefab;
    [SerializeField] GameObject MapUI;
    private void Start()
    {
        CamManager = GetComponent<CameraManagerPE>();
        t_RenderCam = m_RenderCam.GetComponent<Transform>();
        c_RenderCam = m_RenderCam.GetComponent<Camera>();
        c_RenderCam.enabled = false;
    }

    public void TakePhoto()//这个函数由UI上的快门按钮唤起
    {
        CamManager.photographSerial += 1;
        photographSerial = CamManager.photographSerial;
        StartCoroutine(CameraCaptureCoroutine(c_RenderCam, new Rect(0, 0, pixelX, pixelY), "P" + photographSerial.ToString()));//异步加载照片
    }
    public ImageData SaveToJson(string path)
    {
        ImageData imageData = new ImageData(photographSerial,pixelX,pixelY,t_RenderCam.position,t_RenderCam.rotation,isPortrait);
        SaveData.SaveByJson(Path.Combine(path,
                                            "P"+photographSerial.ToString()+".exif"), imageData);
        return imageData;
    }
    IEnumerator CameraCaptureCoroutine(Camera camera, Rect rect, string fileName)//照片渲染主函数
    {
        //检测手机姿态并自动转图
        gravity = Input.gyro.gravity;
        if (-1 <= gravity.y && gravity.y < -0.5)//Home在右
        {
            //默认状态，什么也不用做
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, 0);
        }
        else if (1 >= gravity.y && gravity.y > 0.5)//Home在左
        {
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, 180);//相机Z转180
        }
        else if (-1 <= gravity.x && gravity.x < -0.5)//Home在上（反拿竖）
        {
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, -90);//
            isPortrait = true;
            //交换长宽比
        }
        else if (1 >= gravity.x && gravity.x > 0.5)//Home在下（正拿竖）
        {
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, 90);//相机Z转90 
            isPortrait = true;
        }
        if (isPortrait)
        {
            float tmp = rect.width;
            rect.width = rect.height;
            rect.height = tmp;//交换长宽比
            c_RenderCam.fieldOfView = CamView.fieldOfView *4/3;
        }
        else
        {
            c_RenderCam.fieldOfView = CamView.fieldOfView;
        }
        string pathN = PictureInstacer.GetAlbumPath();
        SaveData.CreateDirectory(pathN);
        
        SaveToJson(pathN+"/");//保存exif

        yield return new WaitForSeconds(3); //协程工作
        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
        render.enableRandomWrite = true; // 启用随机写入以进行 GPU 访问
        render.Create();
        c_RenderCam.enabled = true;//打开渲染相机
        camera.targetTexture = render;//设置截图相机的targetTexture为render
        c_RenderCam.rect = new Rect(0, 0, 1, 1);//相机铺满整个RenderTexture
        camera.Render();//手动开启截图相机的渲染
        Texture2D tex;
        if (Raw_outPut)
            tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB48, false);//16bit
        else
            tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);//8bit
        RenderTexture.active = render;//激活RenderTexture
        tex.ReadPixels(rect, 0, 0);//读取像素
        tex.Apply();//保存像素信息
        RenderTexture.active = null;//关闭RenderTexture的激活状态
        t_RenderCam.localRotation = Quaternion.Euler(180, 180, 0);//恢复相机旋转
        
        render.Release();//释放RenderTex
        c_RenderCam.enabled = false;//关闭渲染相机


        //生成图片并保存到本地
        byte[] bytes = tex.EncodeToJPG();
        File.WriteAllBytes(pathN + "/" + fileName + ".jpg", bytes);//保存图片
        Debug.Log("保存图片于" + pathN + "/"+fileName+".jpg");
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//刷新Unity的资产目录
#elif UNITY_ANDROID
        string[] paths = new string[1];
        paths[0] = pathN;
        ScanFile(paths);//刷新安卓相册
#endif
        //实例化一个imageOnMapPrefab,并将其内的图片设置为刚刚拍的照片
        imageOnMapPrefab.GetComponent<PictureOnMap>().useFilePath = true;
        imageOnMapPrefab.GetComponent<PictureOnMap>().index = photographSerial;
        imageOnMapPrefab.GetComponent<PictureOnMap>().aspectRatio = rect.width / rect.height;
        Instantiate(imageOnMapPrefab, new Vector3(m_RenderCam.transform.position.x, -80, m_RenderCam.transform.position.z),
            Quaternion.Euler(90,0,0),MapUI.transform);

        if (isPortrait)
        {
            float tmp = rect.width;
            rect.width = rect.height;
            rect.height = tmp;
            isPortrait = false;
        }
    }
    void ScanFile(string[] path)//在安卓系统中刷新相册
    {
        using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
            {
                Conn.CallStatic("scanFile", playerActivity, path, null, null);
            }
        }
    }
    //https://blog.csdn.net/qq_36848370/article/details/105371091

}
