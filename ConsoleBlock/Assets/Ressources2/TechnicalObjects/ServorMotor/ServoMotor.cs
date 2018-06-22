using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoMotor : WInteractable {

    public Transform ParentRotor;

    float Goto = 0f;
    float AtSpeed = 0f;

    Vector3 StartRot;
    float StartTime = 0f;

    private void Start () {
        GlobalVariable.Add(new Variable("CurrentRotaion", VariableType.v_float, 0f, new VariableParameters(true, VariableAccessType.v_readonly)));

        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("SetSpecificRotation", new List<VariableTemplate>() {
            new VariableTemplate("Decimal_Rotation", VariableType.v_float),
            new VariableTemplate("Decimal_Speed", VariableType.v_float)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("SetConstantSpeed", new List<VariableTemplate>() {
            new VariableTemplate("Decimal_Speed", VariableType.v_float)
        }));
        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("ExecuteRotation", new List<VariableTemplate>() {
            new VariableTemplate("Turns", VariableType.v_float),
            new VariableTemplate("Decimal_Speed", VariableType.v_float)
        }));
    }

    override public void Update () {
        for(int i = 0; i < FunctionCall.Count; i++) {
            FunctionCaller fc = FunctionCall[FunctionCall.Count - 1];
            FunctionCall.RemoveAt(0);
            int pr = i;
            i = 0;
            if(fc.Name == "SetSpecificRotation") {
                Goto = Mathf.Clamp((float)fc.parameters[0].source, 0f, 1f);
                AtSpeed = Mathf.Clamp((float)fc.parameters[1].source,-1f,1f);

                if(AtSpeed < 0) {
                    Goto = -Goto;
                }
                StartRot = ParentRotor.localEulerAngles;
                StartTime = Time.time;
            } else if(fc.Name == "ExecuteRotation") {
                Goto = (float)fc.parameters[0].source;
                AtSpeed = Mathf.Clamp((float)fc.parameters[1].source, -1f, 1f);

                if(AtSpeed < 0) {
                    Goto = -Goto;
                }
                StartRot = ParentRotor.localEulerAngles;
                StartTime = Time.time;
            } else if(fc.Name == "SetConstantSpeed") {
                Goto = Mathf.Infinity;
                AtSpeed = Mathf.Clamp((float)fc.parameters[0].source, -1f, 1f);
                StartRot = ParentRotor.localEulerAngles;
                StartTime = Time.time;
            }
            i = pr;
        }

        if(AtSpeed != 0f && Goto != Mathf.Infinity && Goto != Mathf.NegativeInfinity) {
            ParentRotor.localEulerAngles = Vector3.Slerp(StartRot, new Vector3(0f, Goto * 360f, 0f), ParentRotor.localEulerAngles.y+Time.deltaTime*AtSpeed);
        } else if(Goto == Mathf.Infinity || Goto == Mathf.NegativeInfinity) {
            ParentRotor.localEulerAngles += Vector3.up * AtSpeed;
        }
    }
}
