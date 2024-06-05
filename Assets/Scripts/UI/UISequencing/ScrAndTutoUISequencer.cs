using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
//����һ��ȫ������/�̵̳����β��ţ��������ϸĸ�Ҳ����Dialogue��
public class ScrAndTutoUISequencer : UIManager
{
    public int SceneIndex;//�������
    public int SequnceIndex;//�������
    public GameObject[] childObjects;//����������object
    CanvasGroup group;
    CanvasGroup groupNext;
    [SerializeField] UnityEvent OnSequenceEnd;
    public int liveObjectIndex = 0;
    public int childCount;
    public float fadeTime = 0.6f;
    bool faded;
    bool isLast;
    // Start is called before the first frame update
    void Start()
    {
        faded = true;
        Transform t = GetComponent<Transform>();
        childCount = t.childCount;
        childObjects = new GameObject[childCount];
        for(int i = 0; i < childCount; i++)
        {
            childObjects[i] = t.GetChild(i).gameObject;
            childObjects[i].GetComponent<ScrAndTutoUI>().enabled = false;
        }
        childObjects[0].GetComponent<CanvasGroup>().alpha = 1;
        childObjects[0].GetComponent<ScrAndTutoUI>().enabled = true;

    }

    public void NextObject()//��һ��
    {
        group = childObjects[liveObjectIndex].GetComponent<CanvasGroup>();
        if (liveObjectIndex < childCount-1)
        {
            groupNext = childObjects[liveObjectIndex + 1].GetComponent<CanvasGroup>();
            isLast = false;
        }
        else isLast = true;
        liveObjectIndex++;
        faded = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //����������ʾ�ģ�������һ��
        if (!faded && !FadeOut(group, fadeTime))//faded == false && FadeOut()==false
        {
            if (!isLast) FadeIn(groupNext, fadeTime*0.99f);
        }
        else if (!faded)
        {
            faded = true;
            group.GetComponent<ScrAndTutoUI>().enabled = false;
            if (!isLast) groupNext.GetComponent<ScrAndTutoUI>().enabled = true;
            else OnSequenceEnd.Invoke();
        }
    }
}
