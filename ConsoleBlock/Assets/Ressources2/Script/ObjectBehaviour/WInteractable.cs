using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: GlobalFunctions

public class WInteractable : WObject {

	public List<Variable> GlobalVariable = new List<Variable>();
    public WObjectTransmitter transmitter = new WObjectTransmitter();

	virtual public void OnInteraction () {
		
	}
}
