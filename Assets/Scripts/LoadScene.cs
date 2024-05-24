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
    public Image loadSlider;
    public TMP_Text loadText;
    public TMP_Text tips;

    // Start is called before the first frame update

    // Update is called once per frame
    public void Load(int index)
    {
        StartCoroutine(LoadS(index));
    }
    IEnumerator LoadS(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index); 
        while (!operation.isDone)   //当场景没有加载完毕
        {
            loadSlider.fillAmount = operation.progress;  //进度条与场景加载进度对应
            loadText.text = (operation.progress * 100).ToString() + "%";
            yield return null;
        }
    }
}
