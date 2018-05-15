using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUI : DefaultUI {

    string Script = string.Empty;
    public TMPro.TMP_InputField scriptInputField;

    public GameObject[] Tabs;

    override public void OpenUI () {
        scriptInputField.text = Target.GetComponent<ConsoleScript>().encscript;
    }
    public void UpdateFunctionExecutionTabs () {
        
    }

    public void Save () {
        if(!Tabs[1].activeSelf) {
            //Manual
            Script = scriptInputField.text;
            Target.GetComponent<ConsoleScript>().encscript = Script;
            StartCoroutine(Target.GetComponent<ConsoleScript>().OnCompilation());
            UpdateFunctionExecutionTabs();
        } else {
            //TODO: Automatic (Visual) Coding: Compile visual script
            //Automatic
        }
    }

    public void ParametersNameChanged (string Name) {
        Target.Name = Name;
    }

    void Update () {
        //TODO: Autocomplete stuff here
    }
}
