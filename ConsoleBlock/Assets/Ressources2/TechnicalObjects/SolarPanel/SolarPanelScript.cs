using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanelScript : WInteractable {

	// Use this for initialization
	void Start () {
        GeneratePPS = !InfinitePPSFilling;
        PPSExpulsion = 15.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if(GeneratePPS) {
            PPSPool = Time.deltaTime * PPSExpulsion;
        }
	}
}
