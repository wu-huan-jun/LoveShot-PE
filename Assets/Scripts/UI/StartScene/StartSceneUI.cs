using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneUI : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    private void OnEnable()
    {
        var data = SaveData.GetSaveData();
        if (data.liveSaveIndex == 0)
        {
            continueButton.SetActive(false);
        }
        else
        {
            continueButton.SetActive(true);
        }
    }
}
