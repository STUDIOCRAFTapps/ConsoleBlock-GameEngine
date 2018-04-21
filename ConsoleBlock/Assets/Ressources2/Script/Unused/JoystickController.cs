using UnityEngine;
using System.Collections;

public class JoystickController : MonoBehaviour {

	public bool Static = false;
	Vector2 Pos = Vector2.zero;
	float Inclining = 20f;
	public OutputConnecter output;

	// Use this for initialization
	void Start () {
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[2];
		output.DataId = new string[2] {
			"Input.AxisX",
			"Input.AxisY"
		};
	}
	
	// Update is called once per frame
	void Update () {
		if(Static) {
			if(Input.GetKey(KeyCode.D)) {
				Pos = new Vector2(1,0);
			} else if(Input.GetKey(KeyCode.A)) {
				Pos = new Vector2(-1,0);
			} else if(Input.GetKey(KeyCode.W)) {
				Pos = new Vector2(0,1);
			} else if(Input.GetKey(KeyCode.S)) {
				Pos = new Vector2(0,-1);
			} else {
				Pos = Vector2.zero;
			}
		} else {
			Pos = Vector2.zero;
		}

		transform.parent.GetChild(2).localRotation = Quaternion.Lerp(transform.parent.GetChild(2).localRotation, Quaternion.Euler(Pos.x*Inclining, 0, Pos.y*Inclining), 0.05f);

		output.Data[0] = Pos.x.ToString();
		output.Data[1] = Pos.y.ToString();
	}
}
