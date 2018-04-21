using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Contoller : MonoBehaviour {

	public InputConnector input;
	public OutputConnecter output1;
	public OutputConnecter output2;

	List<string> TemporaryNameSave1;
	List<string> TemporaryValueSave1;
	List<string> TemporaryNameSave2;
	List<string> TemporaryValueSave2;

	string[] TempN1;
	string[] TempV1;
	string[] TempN2;
	string[] TempV2;

	bool HaveNewContent = false;

	// Use this for initialization
	void Start () {
		TempN1 = new string[0];
		TempV1 = new string[0];
		TempN2 = new string[0];
		TempV2 = new string[0];
		input = input.GetComponent<InputConnector>();
		output1 = output1.GetComponent<OutputConnecter>();
		output2 = output2.GetComponent<OutputConnecter>();
		/*output.Data = new string[3];
		output.DataId = new string[3] {
			"Input.HorizontalRotation",
			"Input.VerticalRotation",
			"Input.AdjustClaw"
		};*/
		TemporaryNameSave1 = new List<string>();
		TemporaryValueSave1 = new List<string>();
		TemporaryNameSave2 = new List<string>();
		TemporaryValueSave2 = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {
		TemporaryNameSave1 = new List<string>();
		TemporaryNameSave2 = new List<string>();
		TemporaryValueSave1 = new List<string>();
		TemporaryValueSave2 = new List<string>();
		int d = input.SendFunctionMailNameI.Count;
		for(int i = 0; i < d; i++) {
			//Debug.Log("I'm pretty! : " + i);
			//Debug.Log("Fix : " + i);
			//Debug.Log("Oh! Look! It's a new mail! Let's see what's inside! The name is " + input.SendFunctionMailNameI[i] + ".");
			string Type; //Mail or Save
			int Port; //1 or 2
			string GPort; //Global Port Gestionnairy
			string Action; //Output.
			string Data; //Value
			//Port + Type + Action, Value
			//Debug.Log(input.SendFunctionMailNameI[i]);
			Type = input.SendFunctionMailNameI[0].Split(',')[1];
			Port = int.Parse((input.SendFunctionMailNameI[0].Split(',')[0]).Split('/')[0]);
			GPort = (input.SendFunctionMailNameI[0].Split(',')[0]);
			Action = input.SendFunctionMailNameI[0].Split(',')[2];
			Data = input.SendFunctionMailValueI[0];
			//Debug.Log("Let's see what's inside the mail! Here's what I found:("+Port+","+Type+","+Action+","+Data+")");
			//SendingMessage(Data, Port + "," + Type + "," + Action, true);
			if((input.SendFunctionMailNameI[0].Split(',')[0]).Split('/').Length > 1) {
				if(Type == "Mail") {
					if(Port == 1) {
						TemporaryNameSave1.Add(PortRemoveUsed(GPort) + "," + Type.ToString() + "," + Action.ToString());
						TemporaryValueSave1.Add(Data);
					} else {
						TemporaryNameSave2.Add(PortRemoveUsed(GPort) + "," + Type.ToString() + "," + Action.ToString());
						TemporaryValueSave2.Add(Data);
					}
				} else {
					if(Port == 1) {
						TempN1.ToList().Add(PortRemoveUsed(GPort) + "," + Type.ToString() + "," + Action.ToString());
						TempV1.ToList().Add(Data);
					} else {
						TempN2.ToList().Add(PortRemoveUsed(GPort) + "," + Type.ToString() + "," + Action.ToString());
						TempV2.ToList().Add(Data);
					}
				}
			} else {
				if(Type == "Mail") {
					if(Port == 1) {
						TemporaryNameSave1.Add(Action);
						TemporaryValueSave1.Add(Data);
					} else {
						TemporaryNameSave2.Add(Action);
						TemporaryValueSave2.Add(Data);
					}
				} else {
					if(Port == 1) {
						TempN1.ToList().Add(Action);
						TempV1.ToList().Add(Data);
					} else {
						TempN2.ToList().Add(Action);
						TempV2.ToList().Add(Data);
					}
				}
			}
			//Debug.Log("Everything as been send to the Temporary Value Saver...");
			input.SendFunctionMailNameI.RemoveAt(0);
			input.SendFunctionMailValueI.RemoveAt(0);

			HaveNewContent = true;
		}
		if(TempN1 != output1.DataId) {
			output1.DataId = TempN1;
		}
		if(TempV1 != output1.Data) {
			output1.Data = TempV1;
		}
		if(TempN2 != output2.DataId) {
			output2.DataId = TempN2;
		}
		if(TempV2 != output2.Data) {
			output2.Data = TempV2;
		}
		if(HaveNewContent) {
			//Debug.Log("New contentent!");
			output1.SendFunctionMailNameI = TemporaryNameSave1;
			output1.SendFunctionMailValueI = TemporaryValueSave1;
			output2.SendFunctionMailNameI = TemporaryNameSave2;
			output2.SendFunctionMailValueI = TemporaryValueSave2;
			HaveNewContent = false;
		}
	}

	string PortRemoveUsed (string Ports) {
		string Base = string.Empty;
		for(int i = 0; i > Ports.Split('/').Length; i++) {
			if(i != 0) {
				Base+=Ports.Split('/')[i];
				if(i > Ports.Split('/').Length-1) {
					Base+="/";
				}
			}
		}
		return Base;
	}
}
