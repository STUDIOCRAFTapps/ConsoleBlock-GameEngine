using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickScript : WInteractable {

    public Transform Handle;
    public Vector2 TargetRotation;
    public Vector2 CurrentRotation;

    bool IsSelected = false;

    private void Start () {
        GlobalVariable.Add(new Variable("AxisX", VariableType.v_float, 0f, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("AxisY", VariableType.v_float, 0f, new VariableParameters(true, VariableAccessType.v_readonly)));
    }

    override public void OnPointedAt (Player player) {
        TargetRotation = Vector2.zero;
        if(InputControl.GetInput(InputControl.InputType.MouvementFoward)) {
            TargetRotation.x -= 1;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementBackward)) {
            TargetRotation.x += 1;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementLeft)) {
            TargetRotation.y -= 1;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementRight)) {
            TargetRotation.y += 1;
        }

        player.ForcedMouvementFreeze = true;
        IsSelected = true;
    }

    private void Update () {
        GlobalVariable[0].source = TargetRotation.y;
        GlobalVariable[1].source = TargetRotation.x;

        if(!IsSelected) {
            TargetRotation = Vector2.zero;
        }
        CurrentRotation = Vector2.Lerp(CurrentRotation, TargetRotation, 0.1f);
        Handle.localEulerAngles = new Vector3(CurrentRotation.x*17,0,CurrentRotation.y*17);

        IsSelected = false;
    }
}
