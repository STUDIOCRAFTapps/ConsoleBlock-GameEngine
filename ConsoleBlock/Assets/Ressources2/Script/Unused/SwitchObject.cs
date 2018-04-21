using UnityEngine;
using System.Collections;

public class SwitchObject : MonoBehaviour {

	public OutputConnecter output;
	public bool IsPress;
	public bool Updated;
	
	// Update is called once per frame
	void Start () {
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[2];
		output.DataId = new string[2] {
			"Input.IsOn",
			"Input.ValueChange"
		};
	}

	void Update () {
		if(IsPress) {
			output.Data[0] = "true";
		} else {
			output.Data[0] = "false";
		}
		if(Updated) {
			output.Data[1] = "true";
			Updated = false;
		} else {
			output.Data[1] = "false";
		}
		if(IsPress) {
			transform.localRotation = Quaternion.Euler(10, 0, 0);
		} else {
			transform.localRotation = Quaternion.Euler(-10, 0, 0);
		}
	}
}
