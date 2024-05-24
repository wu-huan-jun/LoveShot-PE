using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadScene : MonoBehaviour
{
    public AsyncOperation async;//�����첽������
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
        while (!operation.isDone)   //������û�м������
        {
            loadSlider.fillAmount = operation.progress;  //�������볡�����ؽ��ȶ�Ӧ
            loadText.text = (operation.progress * 100).ToString() + "%";
            yield return null;
        }
    }
}
