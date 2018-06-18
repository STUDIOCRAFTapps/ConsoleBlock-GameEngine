using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputControl {

    public static bool GetInputDown (InputType inputType) {
        if(!IsInputTypeKeycode(inputType)) {
            return Input.GetMouseButtonDown(InputTypeToMouseButton(inputType));
        } else {
            return Input.GetKeyDown(InputTypeToKeycode(inputType));
        }
    }

    public static bool GetInput (InputType inputType) {
        if(!IsInputTypeKeycode(inputType)) {
            return Input.GetMouseButton(InputTypeToMouseButton(inputType));
        } else {
            return Input.GetKey(InputTypeToKeycode(inputType));
        }
    }

    public static bool GetInputUp (InputType inputType) {
        if(!IsInputTypeKeycode(inputType)) {
            return Input.GetMouseButtonUp(InputTypeToMouseButton(inputType));
        } else {
            return Input.GetKeyUp(InputTypeToKeycode(inputType));
        }
    }

    public static int InputTypeToMouseButton (InputType inputType) {
        //TODO: Prepare InputTypeToMouseButton for custom keycode

        return (int)inputType;
    }

    public static KeyCode InputTypeToKeycode (InputType inputType) {
        //TODO: Prepare InputTypeToKeycode for custom keycode

        switch(inputType) {
            case InputType.Close:
            return KeyCode.Escape;

            case InputType.BuildingMode:
            return KeyCode.Tab;

            case InputType.CodingInputFieldShowAutocomplete:
            return KeyCode.LeftAlt;

            case InputType.BuildingChangeRotation:
            return KeyCode.R;

            case InputType.BuildingInventory:
            return KeyCode.E;

            case InputType.MouvementFoward:
            return KeyCode.W;

            case InputType.MouvementBackward:
            return KeyCode.S;

            case InputType.MouvementLeft:
            return KeyCode.A;

            case InputType.MouvementRight:
            return KeyCode.D;

            case InputType.MouvementJump:
            return KeyCode.Space;
        }
        return 0;
    }

    public static bool IsInputTypeKeycode (InputType inputType) {
        if(inputType == InputType.MouseMainPress || inputType == InputType.MouseSecondairyPress || inputType == InputType.MouseSpecialPress) {
            return false;
        } else {
            return true;
        }
    }

    public enum InputType {
        MouseMainPress,
        MouseSecondairyPress,
        MouseSpecialPress,
        CodingInputFieldShowAutocomplete,
        Close,
        BuildingMode,
        BuildingChangeRotation,
        BuildingInventory,
        MouvementFoward,
        MouvementBackward,
        MouvementLeft,
        MouvementRight,
        MouvementJump
    }
}
