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
        while (!operation.isDone)   //������û�м������
        {
            loadSlider.fillAmount = operation.progress;  //�������볡�����ؽ��ȶ�Ӧ
            loadText.text = (operation.progress * 100).ToString() + "%";
            yield return null;
        }
    }*/
    private IEnumerator LoadingScene(int index)
    {
        UIManager.FadeInAn(LoadCGroup, 1);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index); 
        asyncOperation.allowSceneActivation = false;                          //����������������
                                                                              //�첽������ allowSceneActivation== falseʱ���Ῠ��0.89999��һ��ֵ
        while (asyncOperation.progress < 0.9f)                                //���첽����С��0.9f��ʱ��
        {
            targetProgress = (int)(asyncOperation.progress * 100); //�첽������ allowSceneActivation= falseʱ���Ῠ��0.89999��һ��ֵ���������100ת����
            yield return LoadProgress();
        }
        targetProgress = 100; //ѭ���󣬵�ǰ�����Ѿ�Ϊ90�ˣ�������Ҫ����Ŀ����ȵ�100������ѭ��
        yield return LoadProgress();
        asyncOperation.allowSceneActivation = true; //������ϣ����Ｄ��� ���� ��ת�����ɹ�
    }

    private IEnumerator<WaitForEndOfFrame> LoadProgress()
    {
        while (currentProgress < targetProgress) //��ǰ���� < Ŀ�����ʱ
        {
            currentProgress += loadSpeed;                            
            loadSlider.fillAmount = (float)currentProgress / 100; //��UI��������ֵ
            loadText.text = currentProgress.ToString() + "%";
            yield return new WaitForEndOfFrame();         //��һ֡
        }
    }
}