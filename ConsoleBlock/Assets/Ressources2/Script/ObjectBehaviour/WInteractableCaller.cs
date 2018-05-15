using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WInteractableCaller : MonoBehaviour {

    public WInteractable interactable;
    public CallType callType;

    public WInteractable Call () {
        if(callType == CallType.TactileInteraction) {
            return null;
        } else if(callType == CallType.Transmition || callType == CallType.PowerOuput || callType == CallType.PowerInput) {
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

    public WInteractable TactileCall (Vector2 UVs, bool MouseDown, bool MousePress, bool MouseUp) {
        if(callType == CallType.TactileInteraction) {
            if(interactable.GetComponent<ScreenScript>() != null) {
                interactable.GetComponent<ScreenScript>().TouchUVs = UVs;
                interactable.GetComponent<ScreenScript>().MouseDown = MouseDown;
                interactable.GetComponent<ScreenScript>().MousePress = MousePress;
                interactable.GetComponent<ScreenScript>().MouseUp = MouseUp;
            }
        }
        return null;
    }
}

public enum CallType {
    Interaction,
    TactileInteraction,
    Transmition,
    PowerInput,
    PowerOuput
}
