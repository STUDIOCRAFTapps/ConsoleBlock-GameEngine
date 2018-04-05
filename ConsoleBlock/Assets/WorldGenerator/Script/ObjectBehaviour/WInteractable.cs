using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WInteractable : WObject {

	public List<Variable> GlobalVariable = new List<Variable>();
	public WObjectInput inputSource;
	public WObjectOutput outputSources;

	virtual public void OnInteraction () {
		
	}
}
