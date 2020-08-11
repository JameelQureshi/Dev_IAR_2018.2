using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputFieldDetector : MonoBehaviour
{
    public InputField inputField;

    protected void Start()
    {
        Debug.Log("<color=red> Input Start method</color>");
    }

    public void OnSelect()
    {
        Debug.Log("<color=red> Input OnSelect method</color>");
        //Register InputField Events
        inputField.onEndEdit.AddListener(delegate { inputEndEdit(); });
        inputField.onValueChanged.AddListener(delegate { inputValueChanged(); });
    }

    public void clicked()
    {
        //Register InputField Events
        inputField.onEndEdit.AddListener(delegate { inputEndEdit(); });
        inputField.onValueChanged.AddListener(delegate { inputValueChanged(); });
    }

    //Called when Input is submitted
    private void inputEndEdit()
    {
        Debug.Log("<color=red> Input Submitted</color>");
    }

    //Called when Input changes
    private void inputValueChanged()
    {
        Debug.Log("<color=red> Input Changed</color>");
    }

    void OnDisable()
    {
        Debug.Log("<color=red> Input OnDisable method</color>");
        //Un-Register InputField Events
        inputField.onEndEdit.RemoveAllListeners();
        inputField.onValueChanged.RemoveAllListeners();
    }
}
