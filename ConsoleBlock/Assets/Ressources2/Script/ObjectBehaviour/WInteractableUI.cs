using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WInteractableUI : DefaultUI {

    public InputField nameInputField;

    public Text[] PPS_Values;

    override public void OpenUI () {
        nameInputField.text = Target.Name;

        PPS_Values[0].text = Target.ComsumePPS.ToString();
        PPS_Values[1].text = Target.GeneratePPS.ToString();
        PPS_Values[2].text = Target.PPSComsumption.ToString("F1");
        PPS_Values[3].text = Target.PPSExpulsion.ToString("F1");
        if(Target.PowerSource != null) {
            PPS_Values[4].text = Target.PowerSource.Name;
        } else {
            PPS_Values[4].text = "Null";
        }
    }

    public void Save () {
        Target.Name = nameInputField.text;
    }
}
