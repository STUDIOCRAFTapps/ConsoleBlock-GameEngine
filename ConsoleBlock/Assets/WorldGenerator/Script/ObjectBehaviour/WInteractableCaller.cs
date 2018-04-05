using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WInteractableCaller : MonoBehaviour {

	public WInteractable interactable;

	public void Call () {
		interactable.OnInteraction();
	}
}
