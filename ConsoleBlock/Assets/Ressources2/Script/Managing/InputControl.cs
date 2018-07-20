using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputControl {

    public static bool GetInputDown (InputType inputType) {
        if(!IsInputTypeKeycode(inputType)) {
            if(inputType == InputType.SelectionScrollUp) {
                return Input.mouseScrollDelta.y > 0.4f;
            } else if(inputType == InputType.SelectionScrollDown) {
                return Input.mouseScrollDelta.y < 0.4f;
            } else {
                return Input.GetMouseButtonDown(InputTypeToMouseButton(inputType));
            }
        } else {
            return Input.GetKeyDown(InputTypeToKeycode(inputType));
        }
    }

    public static bool GetInput (InputType inputType) {
        if(!IsInputTypeKeycode(inputType)) {
            if(inputType == InputType.SelectionScrollUp) {
                return Input.mouseScrollDelta.y > 0f;
            } else if(inputType == InputType.SelectionScrollDown) {
                return Input.mouseScrollDelta.y < 0f;
            } else {
                return Input.GetMouseButton(InputTypeToMouseButton(inputType));
            }
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

    public static bool GetAnyInputDown (InputType[] inputType) {
        bool Result = false;
        foreach(InputType input in inputType) {
            if(!IsInputTypeKeycode(input)) {
                if(input == InputType.SelectionScrollUp) {
                    Result = Result || Input.mouseScrollDelta.y > 0.4f;
                } else if(input == InputType.SelectionScrollDown) {
                    Result = Result || Input.mouseScrollDelta.y < 0.4f;
                } else {
                    Result = Result || Input.GetMouseButtonDown(InputTypeToMouseButton(input));
                }
            } else {
                Result = Result || Input.GetKeyDown(InputTypeToKeycode(input));
            }
        }
        return Result;
    }

    public static bool GetAnyInput (InputType[] inputType) {
        bool Result = false;
        foreach(InputType input in inputType) {
            if(!IsInputTypeKeycode(input)) {
                if(input == InputType.SelectionScrollUp) {
                    Result = Result || Input.mouseScrollDelta.y > 0f;
                } else if(input == InputType.SelectionScrollDown) {
                    Result = Result || Input.mouseScrollDelta.y < 0f;
                } else {
                    Result = Result || Input.GetMouseButton(InputTypeToMouseButton(input));
                }
            } else {
                Result = Result || Input.GetKey(InputTypeToKeycode(input));
            }
        }
        return Result;
    }

    public static bool GetAnyInputUp (InputType[] inputType) {
        bool Result = false;
        foreach(InputType input in inputType) {
            if(!IsInputTypeKeycode(input)) {
                Result = Result || Input.GetMouseButtonUp(InputTypeToMouseButton(input));
            } else {
                Result = Result || Input.GetKeyUp(InputTypeToKeycode(input));
            }
        }
        return Result;
    }

    public static int InputTypeToMouseButton (InputType inputType) {
        //TODO: Prepare InputTypeToMouseButton for custom keycode

        return (int)inputType;
    }

    public static KeyCode InputTypeToKeycode (InputType inputType) {
        //TODO: Prepare InputTypeToKeycode for custom keycode
        int KeyboardType = PlayerPrefs.GetInt("KeyboardType", 0);

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
            if(KeyboardType == 0) {
                return KeyCode.W;
            } else {
                return KeyCode.Z;
            }

            case InputType.MouvementBackward:
            if(KeyboardType == 0) {
                return KeyCode.S;
            } else {
                return KeyCode.S;
            }

            case InputType.MouvementLeft:
            if(KeyboardType == 0) {
                return KeyCode.A;
            } else {
                return KeyCode.Q;
            }

            case InputType.MouvementRight:
            if(KeyboardType == 0) {
                return KeyCode.D;
            } else {
                return KeyCode.D;
            }

            case InputType.MouvementJump:
            return KeyCode.Space;

            case InputType.SelectionUp:
            return KeyCode.UpArrow;

            case InputType.SelectionDown:
            return KeyCode.DownArrow;

            case InputType.SelectionLeft:
            return KeyCode.LeftArrow;

            case InputType.SelectionRight:
            return KeyCode.RightArrow;
        }
        return 0;
    }

    public static bool IsInputTypeKeycode (InputType inputType) {
        if(inputType == InputType.MouseMainPress || inputType == InputType.MouseSecondairyPress || inputType == InputType.MouseSpecialPress || inputType == InputType.SelectionScrollDown || inputType == InputType.SelectionScrollUp) {
            return false;
        } else {
            return true;
        }
    }

    public enum InputType {
        MouseMainPress,
        MouseSecondairyPress,
        MouseSpecialPress,
        SelectionScrollUp,
        SelectionScrollDown,
        CodingInputFieldShowAutocomplete,
        Close,
        BuildingMode,
        BuildingChangeRotation,
        BuildingInventory,
        MouvementFoward,
        MouvementBackward,
        MouvementLeft,
        MouvementRight,
        MouvementJump,
        SelectionUp,
        SelectionDown,
        SelectionLeft,
        SelectionRight
    }
}
