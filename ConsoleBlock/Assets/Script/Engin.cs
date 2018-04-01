using UnityEngine;
using System.Collections;

public class Engin : MonoBehaviour {

	public InputConnector input;
	public OutputConnecter output;
	public EnergyInputSaver ei;
	float TargetConsomation = 0f;
	float Speed = 0f;
	float TargetSpeed = 0f;
	bool Direction;

	public Transform Rotor;

	// Use this for initialization
	void Start () {
		input = input.GetComponent<InputConnector>();
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[2];
		output.DataId = new string[2] {
			"Input.Speed",
			"Input.Rotation"
		};
	}
	
	// Update is called once per frame
	void Update () {
		if(input.SendFunctionMailNameI.Count > 0) {
			int SaveCount = input.SendFunctionMailNameI.Count;
			for(int c = 0; c < SaveCount; c++) {
				switch(input.SendFunctionMailNameI[c]) {
				case "ApplySpeed":
					TargetSpeed = Mathf.Clamp(float.Parse(input.SendFunctionMailValueI[c]),0,60);
					break;
				case "SwitchDirection":
					Direction = !Direction;
					break;
				default:
					break;
				}
			}
		}
		TargetConsomation = TargetSpeed/60f;
		if(Direction) {
			if(TargetConsomation <= ei.GetComponent<EnergyInputSaver>().EnergySource) {
				Speed = TargetSpeed;
				ei.GetComponent<EnergyInputSaver>().EnergySource = ei.GetComponent<EnergyInputSaver>().EnergySource - TargetConsomation;
				Rotor.Rotate(0,0,Time.deltaTime*Speed);
			} else {
				Speed =	ei.GetComponent<EnergyInputSaver>().EnergySource * 60f;
				ei.GetComponent<EnergyInputSaver>().EnergySource = 0f;
				Rotor.Rotate(0,0,Time.deltaTime*Speed);
			}
		} else {
			if(TargetConsomation <= ei.GetComponent<EnergyInputSaver>().EnergySource) {
				Speed = TargetSpeed;
				ei.GetComponent<EnergyInputSaver>().EnergySource = ei.GetComponent<EnergyInputSaver>().EnergySource - TargetConsomation;
				Rotor.Rotate(0,0,-Time.deltaTime*Speed);
			} else {
				Speed =	ei.GetComponent<EnergyInputSaver>().EnergySource * 60f;
				ei.GetComponent<EnergyInputSaver>().EnergySource = 0f;
				Rotor.Rotate(0,0,-Time.deltaTime*Speed);
			}
		}
	}
}
