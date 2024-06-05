using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadScene : MonoBehaviour
{
    public AsyncOperation async;//场景异步加载类
    public float loadProgess;
    public CanvasGroup LoadCGroup;
    public Image loadSlider;
    public TMP_Text loadText;
    public TMP_Text tips;
    int targetProgress;
    int currentProgress;
    int loadSpeed;

    private void Start()
    {
        LoadCGroup = GetComponent<CanvasGroup>();
        LoadCGroup.alpha = 1;
        UIManager.FadeOutAn(LoadCGroup, 1);
        loadSpeed = 1;
    }
    public void Load(int index, int loadSpeed)
    {
        this.loadSpeed = loadSpeed;
        targetProgress = 0;
        currentProgress = 0;
        StartCoroutine(LoadingScene(index));
    }
    public void Load(int index)
    {
        targetProgress = 0;
        currentProgress = 0;
        StartCoroutine(LoadingScene(index));
    }
    /*
    IEnumerator LoadS(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index); 
        while (!operation.isDone)   //当场景没有加载完毕
        {
            loadSlider.fillAmount = operation.progress;  //进度条与场景加载进度对应
            loadText.text = (operation.progress * 100).ToString() + "%";
            yield return null;
        }
    }*/
    private IEnumerator LoadingScene(int index)
    {
        UIManager.FadeInAn(LoadCGroup, 1);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index); 
        asyncOperation.allowSceneActivation = false;                          //不允许场景立即激活
                                                                              //异步进度在 allowSceneActivation== false时，会卡在0.89999的一个值
        while (asyncOperation.progress < 0.9f)                                //当异步加载小于0.9f的时候
        {
            targetProgress = (int)(asyncOperation.progress * 100); //异步进度在 allowSceneActivation= false时，会卡在0.89999的一个值，这里乘以100转整形
            yield return LoadProgress();
        }
        targetProgress = 100; //循环后，当前进度已经为90了，所以需要设置目标进度到100；继续循环
        yield return LoadProgress();
        asyncOperation.allowSceneActivation = true; //加载完毕，这里激活场景 —— 跳转场景成功
    }

    private IEnumerator<WaitForEndOfFrame> LoadProgress()
    {
        while (currentProgress < targetProgress) //当前进度 < 目标进度时
        {
            currentProgress += loadSpeed;                            
            loadSlider.fillAmount = (float)currentProgress / 100; //给UI进度条赋值
            loadText.text = currentProgress.ToString() + "%";
            yield return new WaitForEndOfFrame();         //等一帧
        }
    }
}