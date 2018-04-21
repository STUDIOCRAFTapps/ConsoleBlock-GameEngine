using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WObject : MonoBehaviour {

	public string Name = "";
	WMaterialProprieties proprieties;

	virtual public void Initialize () {
		
	}

	virtual public void Destroy () {
		
	}

	virtual public void Update () {
		
	}
}

public class WMaterialProprieties {
	public bool CanBurn = false;
	public bool HasWaterResistance = true;
}
