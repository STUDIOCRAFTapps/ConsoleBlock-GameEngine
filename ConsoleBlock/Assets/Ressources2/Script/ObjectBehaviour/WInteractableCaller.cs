using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WInteractableCaller : MonoBehaviour {

    public WInteractable interactable;
    public CallType callType;

    public WInteractable Call () {
        if(callType == CallType.TactileInteraction) {
            return null;
        } else if(callType == CallType.Transmition || callType == CallType.PowerOutput || callType == CallType.PowerInput) {
            return interactable;
        } else {
            return null;
        }
    }

    public WInteractable Call (Player player) {
        if(callType == CallType.Interaction) {
            interactable.OnInteraction(player);
            return null;
        } else {
            return interactable;
        }
    }

    public void PointedAtCall (Player player) {
        if(callType == CallType.PointedAt) {
            interactable.OnPointedAt(player);
        }
    }

    public WInteractable TactileCall (Vector2 UVs, bool MouseDown, bool MousePress, bool MouseUp) {
        if(MouseDown) {
            if(interactable.GetComponent<SwitchScript>() != null) {
                interactable.GetComponent<SwitchScript>().OnSwitchInteraction();
            }
        }
        if(callType == CallType.TactileInteraction) {
            if(interactable.GetComponent<ScreenScript>() != null) {
                interactable.GetComponent<ScreenScript>().TouchUVs = UVs;
                interactable.GetComponent<ScreenScript>().MouseDown = MouseDown;
                interactable.GetComponent<ScreenScript>().MousePress = MousePress;
                interactable.GetComponent<ScreenScript>().MouseUp = MouseUp;
            }
            if(interactable.GetComponent<ButtonScript>() != null) {
                interactable.GetComponent<ButtonScript>().OnButtonInteraction();
            }
        }
        return null;
    }
}

public enum CallType {
    Interaction,
    TactileInteraction,
    PointedAt,
    Transmition,
    PowerInput,
    PowerOutput
}
