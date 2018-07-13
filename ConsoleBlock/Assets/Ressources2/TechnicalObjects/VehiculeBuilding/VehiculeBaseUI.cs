using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehiculeBaseUI : WInteractableUI {

    public Text Angle;

    override public void OpenUI () {
        Angle.text = ((int)Target.GetComponent<VehiculeBaseScript>().GlobalVariable[0].source * 90).ToString() + 'º';
    }

    private void Update () {
        
    }

    public void RotationPlus () {
        Target.GetComponent<VehiculeBaseScript>().SetRotation((int)Target.GetComponent<VehiculeBaseScript>().GlobalVariable[0].source + 1);
        Angle.text = ((int)Target.GetComponent<VehiculeBaseScript>().GlobalVariable[0].source * 90).ToString() + 'º';
    }

    public void RotationMinus () {
        Target.GetComponent<VehiculeBaseScript>().SetRotation((int)Target.GetComponent<VehiculeBaseScript>().GlobalVariable[0].source - 1);
        Angle.text = ((int)Target.GetComponent<VehiculeBaseScript>().GlobalVariable[0].source * 90).ToString() + 'º';
    }

    public void DockingToggle () {
        if(Target.GetComponent<VehiculeBaseScript>().CurrentAnchor == null && Target.GetComponent<VehiculeBaseScript>().DetectedAnchor != null) {
            Target.GetComponent<VehiculeBaseScript>().Dock(Target.GetComponent<VehiculeBaseScript>().DetectedAnchor);
        } else {
            Target.GetComponent<VehiculeBaseScript>().Undock();
        }
    }
}
