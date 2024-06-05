using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
//����������������еĵ�������ȴ����ͬʱ�ฺ��DialogueSystem���ȵĴ��
public class DialougeAndSequenceData
{
    public int SceneIndex;//�������
    public int liveSequenceIndex;//����б��
    public int liveDialougeIndex;//��Ի����
    public int liveAnimationHash;//Fengling�Ķ���
    public bool allSequencesEnded;
}
public class SaveDialougeAndSequence : MonoBehaviour
{
    public int SceneIndex;
    [Header("Sequencesȫ������&�̳�����")]
    public int liveSequenceIndex;
    [SerializeField] Transform sequenceParent;//����Sequence�ĸ���
    public GameObject[] sequences;
    int childSequenceCount;//sequences[]�ĳ���
    [SerializeField] bool allSequencesEnded;

    [Header("UDS DialougeSystem")]
    public int liveDialougeIndex;
    [Header("Fengling Scr�����ý�ɫ��Animator����Ҳ��������")]
    [Header("Animation�������")]
    [SerializeField] Animator Fengling_Scr_tmp;
    public int liveAnimationHash;//Fengling���ڲ��ŵĶ���״̬

    // Start is called before the first frame update
    void Start()
    {
        SceneIndex = SceneManager.GetActiveScene().buildIndex;
        childSequenceCount = sequenceParent.childCount;
        sequences = new GameObject[childSequenceCount];
        for (int i = 0; i < childSequenceCount; i++)
        {
            sequences[i] = sequenceParent.GetChild(i).gameObject;
            sequences[i].SetActive(false);
        }
        if (!allSequencesEnded)//��allSequencesEndedΪtrue�������������з����ˡ�
        {
            sequences[liveSequenceIndex].SetActive(true);
        }
    }
    public void NextSequence()//��˳�����һ����������
    {
        if (liveSequenceIndex < childSequenceCount - 1)
        {
            allSequencesEnded = false;
            sequences[liveSequenceIndex + 1].SetActive(true);
        }
        else allSequencesEnded = true;
        sequences[liveSequenceIndex].SetActive(false);
        liveSequenceIndex++;
    }
    public void jumpToSequence(int targetIndex)//���ǵ���ѡ��֦�����������飬������һ��ֱ������ĳ�����еķ���
    {
        if (targetIndex < childSequenceCount)
        {
            sequences[liveSequenceIndex].SetActive(false);
            sequences[targetIndex].SetActive(true);
            liveSequenceIndex = targetIndex;
        }
        else return;
    }
    public void jumpToSequence(int targetIndex,int i)//��Ϊ�ű��е�index�Ǵ�0��ʼ�����ģ����󲿷��������Ǵ�1��ʼ�����ģ����������������i����������
    {
        targetIndex -= 1;
        jumpToSequence(targetIndex);
    }
    public void SaveToJson()
    {
        liveDialougeIndex = DialogueLua.GetVariable("Dialogue_index").asInt;
        liveAnimationHash = Fengling_Scr_tmp.GetCurrentAnimatorStateInfo(0).fullPathHash;
        SaveData.SaveAtDefaultPath("DialougeAndSequenceData.json", this);

    }
    public void LoadFromJson()
    {
        var data = SaveData.loadJsonFromDefaultPath<DialougeAndSequenceData>("DialougeAndSequenceData.json");
        liveSequenceIndex = data.liveSequenceIndex;
        liveDialougeIndex = data.liveDialougeIndex;
        allSequencesEnded = data.allSequencesEnded;
        liveAnimationHash = data.liveAnimationHash;
        Fengling_Scr_tmp.Play(liveAnimationHash, 0);
        DialogueLua.SetVariable("Dialogue_index", liveDialougeIndex);
        Debug.Log(liveSequenceIndex);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
