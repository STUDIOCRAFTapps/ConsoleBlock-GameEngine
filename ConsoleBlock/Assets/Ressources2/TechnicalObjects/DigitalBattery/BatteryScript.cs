using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryScript : WInteractable {

    public Material[] mats;
    public MeshRenderer[] renderers;

    [HideInInspector]
    public int MaxCap = 2000;
    public float Capacity = 0f;

    override public void OnInteraction (Player player) {
        IsInteracting = true;
        player.OpenUI();
        player.uiManager.OpenUI("Battery Editor", this);
    }

    // Use this for initialization
    void Start () {
        GlobalVariable.Add(new Variable("FillingValue", VariableType.v_float, 0.0f, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("FillingDecimal", VariableType.v_float, 0.0f, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("MaxPPSOutputing", VariableType.v_float, 2000.0f, new VariableParameters(true, VariableAccessType.v_readwrite)));
        GlobalVariable.Add(new Variable("MaxPPSInputing", VariableType.v_float, (float)MaxCap, new VariableParameters(true, VariableAccessType.v_readwrite)));
        GlobalVariable.Add(new Variable("IsFull", VariableType.v_bool, false, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("IsEmpty", VariableType.v_bool, true, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("MaxFillingCapacityDecimal", VariableType.v_float, 1.0f, new VariableParameters(true, VariableAccessType.v_readonly)));

        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("SetmaxPPSFlowValues", new List<VariableTemplate>() {
            new VariableTemplate("PPSMinOutputing", VariableType.v_float),
            new VariableTemplate("PPSMaxInputing", VariableType.v_float)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("SetMaxFillingCapacity", new List<VariableTemplate>() {
            new VariableTemplate("MaxFillingCapacityDecimal", VariableType.v_float)
        }));
    }

    // Update is called once per frame
    override public void Update () {
        if(PowerSource != null) {
            Capacity += Mathf.Clamp(PowerSource.PPSPool, 0.0f, (float)GlobalVariable[3].source*Time.deltaTime);
            Capacity = Mathf.Min(MaxCap * (float)GlobalVariable[6].source, Capacity);

            if(Capacity / MaxCap >= 1f) {
                renderers[0].material = mats[1];
                renderers[1].material = mats[1];
                renderers[2].material = mats[1];
                renderers[3].material = mats[1];
            } else if(Capacity / MaxCap > 0.75f) {
                renderers[0].material = mats[0];
                renderers[1].material = mats[1];
                renderers[2].material = mats[1];
                renderers[3].material = mats[1];
            } else if(Capacity / MaxCap > 0.5f) {
                renderers[0].material = mats[0];
                renderers[1].material = mats[0];
                renderers[2].material = mats[1];
                renderers[3].material = mats[1];
            } else if(Capacity / MaxCap > 0f) {
                renderers[0].material = mats[0];
                renderers[1].material = mats[0];
                renderers[2].material = mats[0];
                renderers[3].material = mats[1];
            } else if(Capacity / MaxCap <= 0f) {
                renderers[0].material = mats[0];
                renderers[1].material = mats[0];
                renderers[2].material = mats[0];
                renderers[3].material = mats[0];
            }
        }
        GlobalVariable[0].source = Capacity;
        GlobalVariable[1].source = Capacity / MaxCap;

        GlobalVariable[4].source = Capacity == MaxCap;
        GlobalVariable[5].source = Capacity == 0f;

        PPSPool = Mathf.Clamp(Capacity, 0.0f, (float)GlobalVariable[1].source);

        for(int i = 0; i < FunctionCall.Count; i++) {
            FunctionCaller fc = FunctionCall[FunctionCall.Count - 1];
            FunctionCall.RemoveAt(0);
            int pr = i;
            i = 0;
            if(fc.Name == "SetMaxPPSFlowValues") {
                GlobalVariable[2].source = Mathf.Clamp((float)fc.parameters[0].source, 0.0f, 2000f);
                GlobalVariable[3].source = Mathf.Clamp((float)fc.parameters[1].source, 0.0f, 2000f);
            }
            if(fc.Name == "SetMaxFillingCapacity") {
                GlobalVariable[6].source = Mathf.Clamp((float)fc.parameters[0].source, 0.0f, 1.0f);
            }
        }
    }
}
