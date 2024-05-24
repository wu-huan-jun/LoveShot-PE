using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureFullScreen : UIManager
{
    public Transform photoTransform;//照片在地图上的位置
    [SerializeField] Image image_object;//prefab下挂的图片
    public float aspectRatio;//照片长宽比
    public Texture2D imageSource;
    [SerializeField] GameObject FullScreenPhotoPannel;
    [Header("Tp")]
    [SerializeField] string playerName     = "Xiyue";
    [SerializeField] string playerMeshName = "Xiyue_mesh";//PlayerMale的移动在prefab上，旋转在mesh上，所以分开注册
    [SerializeField] string MainManager = "MainGameManager";
    Transform player, playerMesh;
    // Start is called before the first frame update
    void Start()
    {
        FullScreenPhotoPannel = GameObject.Find("FullScreenPhotos");
        player = GameObject.Find(playerName).transform;
        playerMesh = GameObject.Find(playerMeshName).transform;
        enableCanavasGroup(FullScreenPhotoPannel.GetComponent<CanvasGroup>(), true);
    }
    private void OnEnable()
    {
        image_object.sprite = Sprite.Create(imageSource, new Rect(0, 0, imageSource.width, imageSource.height), new Vector2(.5f, .5f));
        image_object.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
    }
    private void OnDisable()
    {
        disableCanavasGroup(FullScreenPhotoPannel.GetComponent<CanvasGroup>(), true);
        DestroyImmediate(this.gameObject);
    }
    public void Tp()
    {
        playerMesh.position = photoTransform.position;
        playerMesh.rotation = photoTransform.rotation;
        player.gameObject.GetComponent<PlayerCharacterPE>().yRot = 0;
        GameObject.Find(MainManager).GetComponent<MainManagerPE>().SwitchState(PlayState.basic);
        this.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
