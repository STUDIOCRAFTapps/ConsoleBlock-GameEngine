using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Body : MonoBehaviour {
	
	public float HorizontalRotation;
	float VerticalRotation;
	public float ForceAngle;
	public float Half = 0.5f;
	public float CompleteTour = 360f;
	public float MainBodyHeight;
	bool UseMainArm;
	public AnimationCurve TLegZPos;
	public AnimationCurve TLegZRot;
	public AnimationCurve BLegYPos;
	public AnimationCurve BLegZPos;
	public AnimationCurve BLegZRot;
	public bool IsWalking;
	float index;
	public float WalkTLegZPosMultipliyer = 0.75f;
	public float WalkTLegZRotMultipliyer = 160f;
	public float WalkBLegYPosMultipliyer = 0.75f;
	public float WalkBLegZPosMultipliyer = 0.75f;
	public float WalkBLegZRotMultipliyer = -100f;
	public AnimationCurve HandMovement;
	public float HandMouvementMultipliyer = 1f;
	public float HeadTurningSpeed = 3f;

	public float Speed;

	// Use this for initialization
	void Start () {
		///Application.targetFrameRate = 500;
		//Debug.Log(Swipe(0.35f));
	}
	
	// Update is called once per frame
	void Update () {
		if(IsWalking) {
			transform.GetComponent<Rigidbody>().velocity = new Vector3(DirectionFromAngle(Speed, transform.Find("TopChest").Find("HeadHolder").Find("Head").eulerAngles.y + 90f).x, transform.GetComponent<Rigidbody>().velocity.y, DirectionFromAngle(Speed, transform.Find("TopChest").Find("HeadHolder").Find("Head").eulerAngles.y + 90f).z);
		}
		if(Input.GetKeyDown(KeyCode.W)) {
			StartCoroutine("Walking");
		}
		if(Input.GetKeyUp(KeyCode.W)) {
			StartCoroutine("StopWalking");
		}
		HorizontalRotation += CrossPlatformInputManager.GetAxis("Mouse X");
		VerticalRotation += CrossPlatformInputManager.GetAxis("Mouse Y");
		VerticalRotation = Mathf.Clamp(VerticalRotation, -80, 80);
		transform.Find("TopChest").Find("HeadHolder").Find("Head").rotation = Quaternion.Euler(transform.eulerAngles.x, HorizontalRotation, VerticalRotation);
		if(transform.eulerAngles.y > CompleteTour * Half) {
			//Debug.Log("Negative");
			if(!RotationInAngle(transform.eulerAngles.y, ForceAngle, transform.Find("TopChest").Find("HeadHolder").Find("Head").eulerAngles.y) || IsWalking) {
				transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + CrossPlatformInputManager.GetAxis("Mouse X"), transform.eulerAngles.z);
			}
		} else {
			//Debug.Log("Positive");
			if(!RotationInAngle(transform.eulerAngles.y, ForceAngle, transform.Find("TopChest").Find("HeadHolder").Find("Head").eulerAngles.y) || IsWalking) {
				transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + CrossPlatformInputManager.GetAxis("Mouse X"), transform.eulerAngles.z);
			}
		}
		if(IsWalking) {
			index = ((Time.time * 60) / 60) % 1;
			transform.Find("TopLegLeft").localPosition = new Vector3(TLegZPos.Evaluate(index) * WalkTLegZPosMultipliyer, 0.4f, 0.16f);
			transform.Find("TopLegRight").localPosition = new Vector3(-TLegZPos.Evaluate(index) * WalkTLegZPosMultipliyer, 0.4f, -0.16f);

			transform.Find("TopLegLeft").localEulerAngles = new Vector3(0f, 0f, TLegZRot.Evaluate(index) * WalkTLegZRotMultipliyer);
			transform.Find("TopLegRight").localEulerAngles = new Vector3(0f, 0f, -TLegZRot.Evaluate(index) * WalkTLegZRotMultipliyer);

			transform.Find("TopLegLeft").GetChild(0).GetChild(0).localPosition = new Vector3(BLegZPos.Evaluate(index) * WalkBLegZPosMultipliyer + 0f,BLegYPos.Evaluate(index) * WalkBLegYPosMultipliyer + -0.4f, 0f);
			transform.Find("TopLegRight").GetChild(0).GetChild(0).localPosition = new Vector3(BLegZPos.Evaluate(Swipe(index)) * WalkBLegZPosMultipliyer + -0.2096118f, BLegYPos.Evaluate(Swipe(index)) * WalkBLegYPosMultipliyer + -0.2604516f, -0.1254461f);
			
			transform.Find("TopLegLeft").GetChild(0).GetChild(0).localEulerAngles = new Vector3(0f, 0f, -BLegZRot.Evaluate(index) * WalkBLegZRotMultipliyer);
			transform.Find("TopLegRight").GetChild(0).GetChild(0).localEulerAngles = new Vector3(0f, 0f, -BLegZRot.Evaluate(Swipe(index)) * WalkBLegZRotMultipliyer);

			transform.Find("TopChest").Find("LeftArmP").GetChild(0).localEulerAngles = new Vector3(0,0,HandMovement.Evaluate(Swipe(index)) * HandMouvementMultipliyer);
			transform.Find("TopChest").Find("RightArmP").GetChild(0).localEulerAngles = new Vector3(0,0,HandMovement.Evaluate(index) * HandMouvementMultipliyer);
		}
	}

	bool RotationInAngle (float Real, float Angle, float Value) {
		return (Value >= Real - Angle && Value <= Real + Angle);
	}

	float FloatToAngle (float Value) {
		if(Value >= 0) {
			return Value - RoundToBound(360f, Value);
		} else {
			return 360 - (-1f * (Value - RoundToBound(360f, Value)));
		}
	}

	float RoundToBound (float Bound, float Value) {
		return Bound * Mathf.RoundToInt(Value / Bound);
	}

	IEnumerator Walking () {
		index = 0f;
		yield return new WaitForSeconds(0);
		IsWalking = true;
	}

	IEnumerator StopWalking () {
		yield return new WaitForSeconds(0);
		StopCoroutine("Walking");
		IsWalking = false;
		//Reset Pos
	}

	float Swipe (float Value) {
		if(Value <= 0.5f) {
			return Value + 0.5f;
		} else {
			return Value - 0.5f;
		}
	}

	public static Vector3 DirectionFromAngle (float Radius, float Angle) {
		return new Vector3(Mathf.Sin(Mathf.Deg2Rad * Angle) * Radius, 0, Mathf.Cos(Mathf.Deg2Rad * Angle) * Radius);
	}
}
