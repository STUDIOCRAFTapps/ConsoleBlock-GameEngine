using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WObject : MonoBehaviour {

	public string Name = "";
	public WMaterialProprieties proprieties;
    public BuildingBlock Parent;

    virtual public void Initialize () {
		
	}

	virtual public void Destroy () {
		
	}

	virtual public void Update () {
		
	}
}

public class WMaterialProprieties {
}
