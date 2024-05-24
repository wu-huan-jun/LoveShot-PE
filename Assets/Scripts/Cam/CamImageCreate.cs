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

//��Ⱦ��Ƭ��������Ƭ��exif������ϵͳĿ¼��/DCIM/LoveShot/Save{�浵����}/
public class CamImageCreate : MonoBehaviour
{
    [SerializeField] CameraManagerPE CamManager;
    [SerializeField] GameObject m_RenderCam;
    public int photographSerial;
    public int pixelX;
    public int pixelY;//��Ƭ�ߴ�
    public bool Raw_outPut;
    Vector3 gravity;
    Transform t_RenderCam;
    Camera c_RenderCam;
    [SerializeField] Camera CamView;//��Ļ���
    [SerializeField] bool isPortrait;
    [Header("��ͼ����ͼ����")]
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

    public void TakePhoto()//���������UI�ϵĿ��Ű�ť����
    {
        CamManager.photographSerial += 1;
        photographSerial = CamManager.photographSerial;
        StartCoroutine(CameraCaptureCoroutine(c_RenderCam, new Rect(0, 0, pixelX, pixelY), "P" + photographSerial.ToString()));//�첽������Ƭ
    }
    public ImageData SaveToJson(string path)
    {
        ImageData imageData = new ImageData(photographSerial,pixelX,pixelY,t_RenderCam.position,t_RenderCam.rotation,isPortrait);
        SaveData.SaveByJson(Path.Combine(path,
                                            "P"+photographSerial.ToString()+".exif"), imageData);
        return imageData;
    }
    IEnumerator CameraCaptureCoroutine(Camera camera, Rect rect, string fileName)//��Ƭ��Ⱦ������
    {
        //����ֻ���̬���Զ�תͼ
        gravity = Input.gyro.gravity;
        if (-1 <= gravity.y && gravity.y < -0.5)//Home����
        {
            //Ĭ��״̬��ʲôҲ������
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, 0);
        }
        else if (1 >= gravity.y && gravity.y > 0.5)//Home����
        {
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, 180);//���Zת180
        }
        else if (-1 <= gravity.x && gravity.x < -0.5)//Home���ϣ���������
        {
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, -90);//
            isPortrait = true;
            //���������
        }
        else if (1 >= gravity.x && gravity.x > 0.5)//Home���£���������
        {
            t_RenderCam.localRotation = Quaternion.Euler(180, 180, 90);//���Zת90 
            isPortrait = true;
        }
        if (isPortrait)
        {
            float tmp = rect.width;
            rect.width = rect.height;
            rect.height = tmp;//���������
            c_RenderCam.fieldOfView = CamView.fieldOfView *4/3;
        }
        else
        {
            c_RenderCam.fieldOfView = CamView.fieldOfView;
        }
        string pathN = PictureInstacer.GetAlbumPath();
        SaveData.CreateDirectory(pathN);
        
        SaveToJson(pathN+"/");//����exif

        yield return new WaitForSeconds(3); //Э�̹���
        RenderTexture render = new RenderTexture((int)rect.width, (int)rect.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
        render.enableRandomWrite = true; // �������д���Խ��� GPU ����
        render.Create();
        c_RenderCam.enabled = true;//����Ⱦ���
        camera.targetTexture = render;//���ý�ͼ�����targetTextureΪrender
        c_RenderCam.rect = new Rect(0, 0, 1, 1);//�����������RenderTexture
        camera.Render();//�ֶ�������ͼ�������Ⱦ
        Texture2D tex;
        if (Raw_outPut)
            tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB48, false);//16bit
        else
            tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);//8bit
        RenderTexture.active = render;//����RenderTexture
        tex.ReadPixels(rect, 0, 0);//��ȡ����
        tex.Apply();//����������Ϣ
        RenderTexture.active = null;//�ر�RenderTexture�ļ���״̬
        t_RenderCam.localRotation = Quaternion.Euler(180, 180, 0);//�ָ������ת
        
        render.Release();//�ͷ�RenderTex
        c_RenderCam.enabled = false;//�ر���Ⱦ���


        //����ͼƬ�����浽����
        byte[] bytes = tex.EncodeToJPG();
        File.WriteAllBytes(pathN + "/" + fileName + ".jpg", bytes);//����ͼƬ
        Debug.Log("����ͼƬ��" + pathN + "/"+fileName+".jpg");
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();//ˢ��Unity���ʲ�Ŀ¼
#elif UNITY_ANDROID
        string[] paths = new string[1];
        paths[0] = pathN;
        ScanFile(paths);//ˢ�°�׿���
#endif
        //ʵ����һ��imageOnMapPrefab,�������ڵ�ͼƬ����Ϊ�ո��ĵ���Ƭ
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
    void ScanFile(string[] path)//�ڰ�׿ϵͳ��ˢ�����
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
