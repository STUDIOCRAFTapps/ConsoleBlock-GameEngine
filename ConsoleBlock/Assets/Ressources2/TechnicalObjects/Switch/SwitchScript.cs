using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : WInteractable {

    public Transform Switch;

    public bool isOn = false;

    private void Start () {
        GlobalVariable.Add(new Variable("IsOn", VariableType.v_bool, false, new VariableParameters(true, VariableAccessType.v_readonly)));
    }

    public void OnSwitchInteraction () {
        isOn = !isOn;
        GlobalVariable[0].source = isOn;
        if(!isOn) {
            Switch.localEulerAngles = Vector3.right * 9;
        } else {
            Switch.localEulerAngles = Vector3.right * -9;
        }
    }
}
