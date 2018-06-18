using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalVariableUI : MonoBehaviour {

    string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";

    public string Name;
    public VariableType variableType;
    public object Value;
    public bool IsPublic;

    public InputField VariableName;
    public Dropdown VariableTypeDrop;
    public RectTransform[] Inputs;
    public Toggle IsPublicToggle;

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

        switch(VariableTypeDrop.value) {
            case 0:
            variableType = VariableType.v_int;
            Value = int.Parse(Inputs[0].GetComponent<InputField>().text);
            break;
            case 1:
            variableType = VariableType.v_float;
            Value = float.Parse(Inputs[1].GetComponent<InputField>().text, System.Globalization.CultureInfo.InvariantCulture);
            break;
            case 2:
            variableType = VariableType.v_string;
            Value = Inputs[2].GetComponent<InputField>().text;
            break;
            case 3:
            variableType = VariableType.v_char;
            Value = Inputs[3].GetComponent<InputField>().text[0];
            break;
            case 4:
            variableType = VariableType.v_bool;
            Value = Inputs[4].GetComponent<Toggle>().isOn;
            break;
        }
        IsPublic = IsPublicToggle.isOn;
    }

    public void EditVariableDisplay (Variable reference) {
        VariableName.text = reference.Id;
        switch(reference.variableType) {
            case VariableType.v_bool:
            VariableTypeDrop.value = 4;
            Inputs[4].GetComponent<Toggle>().isOn = (bool)reference.source;
            break;
            case VariableType.v_int:
            VariableTypeDrop.value = 0;
            Inputs[0].GetComponent<InputField>().text = ((int)reference.source).ToString();
            break;
            case VariableType.v_float:
            VariableTypeDrop.value = 1;
            Inputs[1].GetComponent<InputField>().text = ((float)reference.source).ToString();
            break;
            case VariableType.v_string:
            VariableTypeDrop.value = 2;
            Inputs[2].GetComponent<InputField>().text = (string)reference.source;
            break;
            case VariableType.v_char:
            VariableTypeDrop.value = 3;
            Inputs[3].GetComponent<InputField>().text = ((char)reference.source).ToString();
            break;
        }
        if(reference.variableParameters != null) {
            IsPublic = reference.variableParameters.IsPublic;
            IsPublicToggle.isOn = reference.variableParameters.IsPublic;
        } else {
            IsPublic = true;
            IsPublicToggle.isOn = true;
        }
    }

    public void DropDownCheckUp () {
        for(int i = 0; i < Inputs.Length; i++) {
            Inputs[i].gameObject.SetActive(VariableTypeDrop.value == i);
        }
    }
}
