using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

public class GetLuaVariables : MonoBehaviour
{
    /// <summary>
    /// The variable to increment.
    /// </summary>
    /*[Tooltip("Increment this Dialogue System variable.")]
    [VariablePopup]
    public string variable = string.Empty;
    protected string actualVariableName
    {
        get { return string.IsNullOrEmpty(variable) ? DialogueActor.GetPersistentDataName(transform) : variable; }
    }

    private IEnumerator Start()
    {
        string oldValue = DialogueLua.GetVariable(actualVariableName).asString;
        Debug.Log("��ǰ����ֵΪ: " + oldValue);
        yield return new WaitForSeconds(1f);
        DialogueLua.SetVariable(actualVariableName, "CNM");
        string newValue = DialogueLua.GetVariable(actualVariableName).asString;
        Debug.Log("�������óɹ�,��ǰ����ֵΪ: " + newValue);
    }*/
    public string variable1 = "Scene";
    public string variable2 = "Dialogue_Index";
    private void Start()
    {
        Debug.Log(DialogueLua.GetVariable("Dialogue_Index").asInt);
    }


}