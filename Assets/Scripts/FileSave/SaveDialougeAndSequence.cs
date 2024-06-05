using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
//负责管理多组剧情序列的调用与进度存读，同时亦负责DialogueSystem进度的存读
public class DialougeAndSequenceData
{
    public int SceneIndex;//场景编号
    public int liveSequenceIndex;//活动序列编号
    public int liveDialougeIndex;//活动对话编号
    public int liveAnimationHash;//Fengling的动画
    public bool allSequencesEnded;
}
public class SaveDialougeAndSequence : MonoBehaviour
{
    public int SceneIndex;
    [Header("Sequences全屏剧情&教程序列")]
    public int liveSequenceIndex;
    [SerializeField] Transform sequenceParent;//所有Sequence的父级
    public GameObject[] sequences;
    int childSequenceCount;//sequences[]的长度
    [SerializeField] bool allSequencesEnded;

    [Header("UDS DialougeSystem")]
    public int liveDialougeIndex;
    [Header("Fengling Scr剧情用角色的Animator数据也存在这里")]
    [Header("Animation动画存读")]
    [SerializeField] Animator Fengling_Scr_tmp;
    public int liveAnimationHash;//Fengling正在播放的动画状态

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
        if (!allSequencesEnded)//当allSequencesEnded为true，代表所有序列放完了。
        {
            sequences[liveSequenceIndex].SetActive(true);
        }
    }
    public void NextSequence()//按顺序的下一个剧情序列
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
    public void jumpToSequence(int targetIndex)//考虑到有选择枝可以跳过剧情，所以做一个直接跳到某个序列的方法
    {
        if (targetIndex < childSequenceCount)
        {
            sequences[liveSequenceIndex].SetActive(false);
            sequences[targetIndex].SetActive(true);
            liveSequenceIndex = targetIndex;
        }
        else return;
    }
    public void jumpToSequence(int targetIndex,int i)//因为脚本中的index是从0开始计数的，但大部分命名则是从1开始计数的，所以用这个带参数i的重载修正
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
