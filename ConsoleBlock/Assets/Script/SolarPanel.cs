using UnityEngine;
using System.Collections;

public class SolarPanel : MonoBehaviour {

	public EnergyOutputSaver output;

	// Use this for initialization
	void Start () {
		output = output.GetComponent<EnergyOutputSaver>();
	}
	
	// Update is called once per frame
	void Update () {
		if(output.EnergySource < 1) {
			output.EnergySource = 0.6f;
		}
	}
}
