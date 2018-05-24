using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariableUI : MonoBehaviour {

    string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";

    public string Name;
    public InputField VariableName;
    public Dropdown VariableTypeDrop;
    public RectTransform[] Inputs;

    void Update () {
        string N = VariableName.text;
        N = N.Replace(" ", "_");
        int c = 0;
        for(int i = 0; i < N.Length; i++) {
            if(!AllowedChars.Contains(N[c].ToString())) {
                N = N.Remove(c);
            } else {
                c++;
            }
        }
        if(N.Length > 0) {
            if(char.IsDigit(N[0])) {
                N = string.Empty;
            }
        }
        Name = N;
        VariableName.text = N;
    }

    public void DropDownCheckUp () {
        for(int i = 0; i < Inputs.Length; i++) {
            Inputs[i].gameObject.SetActive(VariableTypeDrop.value == i);
        }
    }
}
