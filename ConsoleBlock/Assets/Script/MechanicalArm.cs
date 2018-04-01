using UnityEngine;
using System.Collections;

public class MechanicalArm : MonoBehaviour {

	public InputConnector input;
	public OutputConnecter output;
	public EnergyInputSaver EnergieInput;
	float Consomation = 0.4f;

	public Transform FirstHolder;
	public Transform SecondHolder;
	public Transform Claw;

	public float FHolderV = 0f;
	public float FHolderH = 0f;
	public float SHV = 0f;
	public float SHH = 0f;

	public float AdjClaw;
	public float AC;

	float speed = 1f;
	float t;
	float tS;
	float tC;
	public bool ClawStat;

	// Use this for initialization
	void Start () {
		input = input.GetComponent<InputConnector>();
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[3];
		output.DataId = new string[3] {
			"Input.HorizontalRotation",
			"Input.VerticalRotation",
			"Input.AdjustClaw"
		};
	}
	
	// Update is called once per frame
	void Update () {
		if(Consomation <= EnergieInput.GetComponent<EnergyInputSaver>().EnergySource) {
			output.Data[0] = FHolderH.ToString();
			output.Data[1] = FHolderV.ToString();
			output.Data[2] = AdjClaw.ToString();
			EnergieInput.GetComponent<EnergyInputSaver>().EnergySource = EnergieInput.GetComponent<EnergyInputSaver>().EnergySource - Consomation;
			if(input.SendFunctionMailNameI.Count > 0) {
				//Debug.Log("Hey! It's me!");
				int SaveCount = input.SendFunctionMailNameI.Count;
				for(int c = 0; c < SaveCount; c++) {
					switch(input.SendFunctionMailNameI[c]) {
					case "HorizontalRotation":
						//SHH = FHolderH;
						SHH = Mathf.RoundToInt(FirstHolder.localEulerAngles.y);
						FHolderH = int.Parse(input.SendFunctionMailValueI[c]);
						t=0;
						break;
					case "VerticalRotation":
						//SHV = FHolderV;
						SHV = Mathf.RoundToInt(FirstHolder.localEulerAngles.z);
						FHolderV = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[c]), 13f, -70f);
						tS=0;
						break;
					case "AjustClaw":
						//AC = AdjClaw;
						AC = Mathf.RoundToInt(Claw.localEulerAngles.x);
						AdjClaw = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[c]), -100f, 50f);
						tC=0;
						break;
					case "ClawStat":
						ClawStat = (input.SendFunctionMailValueI[c] == "true");
						break;
					default:
						//Debug.Log("Hello it's me!" + input.SendFunctionMailNameI[i]);
						break;
					}
					input.SendFunctionMailNameI.RemoveAt(c);
					input.SendFunctionMailValueI.RemoveAt(c);
				}
			}
			if(ClawStat) {
				Claw.GetChild(0).localRotation = Quaternion.Lerp(Claw.GetChild(0).localRotation, Quaternion.Euler(0f, 230, 154f), Time.deltaTime);
				Claw.GetChild(1).localRotation = Quaternion.Lerp(Claw.GetChild(1).localRotation, Quaternion.Euler(0f, 300, 154f), Time.deltaTime);
			} else {
				Claw.GetChild(0).localRotation = Quaternion.Lerp(Claw.GetChild(0).localRotation, Quaternion.Euler(0f, 280, 154f), Time.deltaTime);
				Claw.GetChild(1).localRotation = Quaternion.Lerp(Claw.GetChild(1).localRotation, Quaternion.Euler(0f, 260, 154f), Time.deltaTime);
			}
			if(Mathf.RoundToInt(FirstHolder.localEulerAngles.y) != Mathf.RoundToInt(FHolderH)) {
				
				t = t + Time.deltaTime * speed;
				FirstHolder.localRotation = Quaternion.Lerp(Quaternion.Euler(0f, SHH, 0f), Quaternion.Euler(0f, FHolderH, 0f), t);
				if(t >= 1) {
					SHH = Mathf.RoundToInt(FirstHolder.localEulerAngles.y);
					FirstHolder.localRotation = Quaternion.Euler(0f, FHolderH, 0f);
				}
			} else {
				t = 0;
			}
			if(Mathf.RoundToInt(FirstHolder.localEulerAngles.z) != Mathf.RoundToInt(FHolderV)) {
				tS = tS + Time.deltaTime * speed;
				FirstHolder.localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, SHV), Quaternion.Euler(0f, 0f, FHolderV), tS);
				if(tS >= 1) {
					SHV = Mathf.RoundToInt(FirstHolder.localEulerAngles.z);
					FirstHolder.localRotation = Quaternion.Euler(0f, 0f, FHolderV);
				}
			} else {
				tS = 0;
			}
			if(Mathf.RoundToInt(Claw.localEulerAngles.x) != Mathf.RoundToInt(AdjClaw)) {
				tC = tC + Time.deltaTime * speed;
				Claw.localRotation = Quaternion.Lerp(Quaternion.Euler(AC, 0f, 0f), Quaternion.Euler(AdjClaw, 0f, 0f), tC);
				if(tC >= 1) {
					AC = Mathf.RoundToInt(Claw.localEulerAngles.x);
					Claw.localRotation = Quaternion.Euler(AdjClaw, 0f, 0f);
				}
			} else {
				tC = 0;
			}
		}
	}
}