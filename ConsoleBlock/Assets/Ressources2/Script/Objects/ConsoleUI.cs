using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUI : DefaultUI {

    string Script = string.Empty;
    public TMPro.TMP_InputField scriptInputField;
    public InputField nameInputField;

    public RectTransform FunctionExecuterView;
    public RectTransform FunctionExecuterTemplate;

    public GameObject[] Tabs;

    override public void OpenUI () {
        scriptInputField.text = Target.GetComponent<ConsoleScript>().encscript;
        nameInputField.text = Target.Name;
        ClearFunctionExecuter();
    }

    public void ClearFunctionExecuter () {
        for(int i = 0; i < FunctionExecuterView.childCount; i++) {
            Destroy(FunctionExecuterView.GetChild(0));
        }
    }

    public void UpdateFunctionExecutionTabs () {
        ClearFunctionExecuter();
        for(int i = 0; i < Target.GetComponent<ConsoleScript>().GlobalFunctionNames.Count; i++) {
            GameObject button = (GameObject)Instantiate(FunctionExecuterTemplate.gameObject,FunctionExecuterView);
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<Text>().text = Target.GetComponent<ConsoleScript>().GlobalFunctionNames[i];
            button.GetComponent<Button>().onClick.AddListener(() => {
                int x = i;
                Target.GetComponent<ConsoleScript>().ExecuteFunction(Target.GetComponent<ConsoleScript>().GlobalFunctionNames[x], Target.GetComponent<ConsoleScript>().GetFunctionParametersTemplate(Target.GetComponent<ConsoleScript>().GlobalFunctionNames[x]));
            });
        }
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
        Target.Name = nameInputField.text;
    }

    public void ParametersNameChanged (string Name) {
        Target.Name = Name;
    }

    void Update () {
        //TODO: Autocomplete stuff here
    }
}
