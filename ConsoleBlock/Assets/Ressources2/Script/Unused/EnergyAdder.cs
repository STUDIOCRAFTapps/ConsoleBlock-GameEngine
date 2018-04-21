using UnityEngine;
using System.Collections;

public class EnergyAdder : MonoBehaviour {

	public EnergyInputSaver i1;
	public EnergyInputSaver i2;
	public EnergyOutputSaver o;
	
	// Update is called once per frame
	void Update () {
		o.EnergySource = i1.EnergySource + i2.EnergySource;
		i1.EnergySource = 0;
		i2.EnergySource = 0;
	}
}
