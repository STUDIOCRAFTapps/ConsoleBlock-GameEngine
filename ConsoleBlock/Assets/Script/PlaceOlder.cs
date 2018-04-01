using UnityEngine;
using System.Collections;

public class PlaceOlder : MonoBehaviour {

	public bool isPlaceOlderOverlap;

	void OnTriggerStay() {
		isPlaceOlderOverlap = true;
	}

	void OnTriggerExit() {
		isPlaceOlderOverlap = false;
	}

	void OnTriggerEnter() {
		isPlaceOlderOverlap = true;
	}

}
