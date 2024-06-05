using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureFullScreen : UIManager
{
    public Vector3 photoPos;
    public Quaternion photoRot;
    [SerializeField] Image image_object;//prefab下挂的图片
    public float aspectRatio;//照片长宽比
    public Texture2D imageSource;
    [SerializeField] GameObject FullScreenPhotoPannel;
    [Header("Tp")]
    [SerializeField] string playerMeshName = "Xiyue_mesh";
    [SerializeField] string MainManager = "MainGameManager";
    Transform player, playerMesh;
    // Start is called before the first frame update
    void Start()
    {
        FullScreenPhotoPannel = GameObject.Find("FullScreenPhotos");
        playerMesh = GameObject.Find(playerMeshName).transform;
        player = playerMesh.parent;
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
        playerMesh.position = photoPos;
        playerMesh.rotation = photoRot;
        player.gameObject.GetComponent<PlayerCharacterPE>().yRot = 0;
        GameObject.Find(MainManager).GetComponent<MainManagerPE>().SwitchState(PlayState.basic);
        this.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
