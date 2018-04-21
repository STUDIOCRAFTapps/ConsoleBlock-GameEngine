using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyState : MonoBehaviour {

	public GameObject Copy;
	bool Last = false;

	// Update is called once per frame
	void Update () {
		if(Last != Copy.activeInHierarchy) {
			GetComponent<Image>().enabled = Copy.activeInHierarchy;
		}
		Last = Copy.activeInHierarchy;
	}
}
