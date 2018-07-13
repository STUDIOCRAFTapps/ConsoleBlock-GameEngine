using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class BatteryUI : DefaultUI {

    public InputField Name;
    public Text[] Values;
    public Image Filling;
    public InputField[] fields;

    public void Save () {
        Target.Name = Name.text;
        Target.GlobalVariable[2].source = Mathf.Clamp(float.Parse(fields[0].text, CultureInfo.InvariantCulture), 0.0f, Target.GetComponent<BatteryScript>().MaxCap);
        Target.GlobalVariable[3].source = Mathf.Clamp(float.Parse(fields[1].text, CultureInfo.InvariantCulture), 0.0f, Target.GetComponent<BatteryScript>().MaxCap);
        Target.GlobalVariable[6].source = Mathf.Clamp(float.Parse(fields[2].text, CultureInfo.InvariantCulture), 0.0f, 100.0f)/100f;
    }

    override public void OpenUI () {
        fields[0].text = Target.GlobalVariable[2].source.ToString();
        fields[1].text = Target.GlobalVariable[3].source.ToString();
        fields[2].text = ((float)Target.GlobalVariable[6].source * 100f).ToString();
    }

        // Update is called once per frame
    void Update () {
        Values[0].text = Target.PowerSource.PPSExpulsion.ToString("F1");
        Values[1].text = Mathf.Clamp((float)Target.GlobalVariable[2].source,0.0f,Target.GetComponent<BatteryScript>().Capacity).ToString("F1");
        if(Target.PowerSource != null) {
            Values[2].text = Target.PowerSource.Name;
        } else {
            Values[2].text = "Null";
        }
        Values[3].text = "Output Sources (?):";
        Values[4].text = "ERROR";
        Values[5].text = "ERROR";
        Values[6].text = "ERROR";
        Values[7].text = (Mathf.Round((float)Target.GlobalVariable[1].source * 100f).ToString() + "%");
        Values[8].text = floatToK(Target.GetComponent<BatteryScript>().Capacity) + "/" + floatToK(Target.GetComponent<BatteryScript>().MaxCap);
        Filling.fillAmount = (float)Target.GlobalVariable[1].source;
    }

    string floatToK (float v) {
        if(v < 1000) {
            return Mathf.FloorToInt(v).ToString();
        } else {
            return (Mathf.Floor(v/100f)/10f).ToString("F1") + "k";
        }
    }
}
