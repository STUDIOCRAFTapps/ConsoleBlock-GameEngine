using UnityEngine;
using System.Collections;

public class EnergyDiviser : MonoBehaviour {

	public EnergyInputSaver i;
	public EnergyOutputSaver o1;
	public EnergyOutputSaver o2;

	[HideInInspector]
	public int Percent1;

	// Update is called once per frame
	void Update () {
		o1.EnergySource = i.EnergySource*(Percent1/100);
		o2.EnergySource = i.EnergySource*((100-Percent1)/100);
		i.EnergySource = 0f;
	}
}
