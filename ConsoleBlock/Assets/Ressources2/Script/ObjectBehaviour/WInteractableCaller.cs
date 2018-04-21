using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WInteractableCaller : MonoBehaviour {

	public WInteractable interactable;
    public CallType callType;

	public WInteractable Call () {
        if(callType == CallType.Interaction) {
            interactable.OnInteraction();
            return null;
        } else {
            return interactable;
        }
	}
}

public enum CallType {
    Interaction,
    Transmition,
    PowerInput,
    PowerOuput
}
