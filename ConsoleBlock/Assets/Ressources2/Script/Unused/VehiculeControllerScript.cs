using UnityEngine;
using System.Collections;

public class VehiculeControllerScript : MonoBehaviour {

	//public Transform TestCube;

	public Transform stearingWheel;
	public float MaximumRotation = 85f;
	public float ReposeSpeed = 1f;
	public float Value = 0;
	public float VerticalValue = 0;
	bool ValueHold = false;
	public OutputConnecter output;
	public AnimationCurve Acceleration;
	public AnimationCurve Break;

	public float TargetAcceTime = 5f;
	//public float MaximalAcceSpeedUp = 0.5f;
	public float TargetDecceTime = 5f;
	//public float MaximalDecceSpeedUp = 0.5f;
	public float AcceTime;
	public float DecceTime;

	public float MaximalSpeed = 10f;

	public bool Static = false;

	// Use this for initialization
	void Start () {
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[2];
		output.DataId = new string[2] {
			"Input.TurningRate",
			"Input.Acceleration"
		};
	}
	
	// Update is called once per frame
	void Update () {
		//TestCube.localScale = new Vector3(0.3f, VerticalValue * 0.1f, 0.3f);
		output.Data[0] = Value.ToString();
		output.Data[1] = VerticalValue.ToString();
		if(Static) {
			if(Input.GetKey(KeyCode.W)) {
				AcceTime += Time.deltaTime;
			} else {
				AcceTime = 0f;
			}
			if(Input.GetKey(KeyCode.S)) {
				DecceTime += Time.deltaTime;
			} else {
				DecceTime = 0f;
			}
			if(AcceTime / TargetAcceTime < 1) {
				VerticalValue += Acceleration.Evaluate(AcceTime / TargetAcceTime);
			} else {
				VerticalValue += Acceleration.Evaluate(1);
			}

			if(DecceTime / TargetDecceTime < 1) {
				VerticalValue -= 1 - Break.Evaluate(DecceTime / TargetDecceTime);
			} else {
				VerticalValue -= 1 - Break.Evaluate(1);
			}

			if(Input.GetKey(KeyCode.D)) {
				ValueHold = true;
			} else if(Input.GetKey(KeyCode.A)) {
				ValueHold = true;
			} else {
				ValueHold = false;
			}

			if(ValueHold) {
				if(Input.GetKey(KeyCode.D)) {
					Value = Mathf.Lerp(Value, -MaximumRotation, Time.deltaTime * (ReposeSpeed/100));
				} else if(Input.GetKey(KeyCode.A)) {
					Value = Mathf.Lerp(Value, MaximumRotation, Time.deltaTime * (ReposeSpeed/100));
				}
			} else {
				if(Value < -0.25f) {
					Value += Time.deltaTime * ReposeSpeed;
				} else if(Value > 0.25f) {
					Value -= Time.deltaTime * ReposeSpeed;
				} else if(Value != 0){
					Value = 0;
				}
			}
		} else {
			AcceTime = 0;
			DecceTime = 0;
			if(Value < -0.25f) {
				Value += Time.deltaTime * ReposeSpeed;
			} else if(Value > 0.25f) {
				Value -= Time.deltaTime * ReposeSpeed;
			} else if(Value != 0){
				Value = 0;

			}
		}
		if(AcceTime == 0 && DecceTime == 0) {
			if(VerticalValue > 0) {
				if(VerticalValue - (Time.deltaTime * ReposeSpeed) < 0) {
					VerticalValue = 0;
				} else {
					VerticalValue -= (Time.deltaTime * ReposeSpeed);
				}
			}
			if(VerticalValue < 0) {
				if(VerticalValue + (Time.deltaTime * ReposeSpeed) > 0) {
					VerticalValue = 0;
				} else {
					VerticalValue += (Time.deltaTime * ReposeSpeed);
				}
			}
		}
		/*if(!ValueHold) {
			if(Value < -0.25f) {
				Value += Time.deltaTime * ReposeSpeed;
			} else if(Value > 0.25f) {
				Value -= Time.deltaTime * ReposeSpeed;
			} else if(Value != 0){
				Value = 0;

			}
		}*/
		VerticalValue = Mathf.Clamp(VerticalValue, -200f, 200f);
		stearingWheel.localRotation = Quaternion.Euler(new Vector3(0f,0f,Value));
	}
}
