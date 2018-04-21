using UnityEngine;
using System.Collections;

public class CamMenu : MonoBehaviour {

	bool FocusPlanet = false;

	// Use this for initialization
	void Update () {
		if(!FocusPlanet) {
			transform.position = new Vector3(Mathf.Lerp(transform.position.x, 0f, 0.1f), transform.position.y, transform.position.z);
		} else {
			transform.position = new Vector3(Mathf.Lerp(transform.position.x, -17f, 0.1f), transform.position.y, transform.position.z);
		}
	}

	public void ArrowLeft () {
		FocusPlanet = true;
	}

	public void ArrowRight () {
		FocusPlanet = false;
	}
}
