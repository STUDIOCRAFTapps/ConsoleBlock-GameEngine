using UnityEngine;
using System.Collections;

public class ButtonObject : MonoBehaviour {

	public OutputConnecter output;
	public bool GetButtonDown;
	public bool GetButton;
	public bool GetButtonUp;

	// Use this for initialization
	void Start () {
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[3];
		output.DataId = new string[3] {
			"Input.GetButtonDown",
			"Input.GetButtonPress",
			"Input.GetButtonUp"
		};
	}
	
	// Update is called once per frame
	void Update () {
		if(GetButtonDown) {
			output.Data[0] = "true";
		} else {
			output.Data[0] = "false";
		}
		if(GetButton) {
			output.Data[1] = "true";
		} else {
			output.Data[1] = "false";
		}
		if(GetButtonUp) {
			output.Data[2] = "true";
		} else {
			output.Data[2] = "false";
		}
		if(GetButton) {
			transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
		} else {
			transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
		}
	}
}
