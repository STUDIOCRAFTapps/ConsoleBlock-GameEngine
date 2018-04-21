using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattryOperator : MonoBehaviour {

	public InputConnector input;
	public OutputConnecter output;
	public EnergyInputSaver einput;
	public EnergyOutputSaver eoutput;

	public Transform Indicator1;
	public Transform Indicator2;
	public Transform Indicator3;
	public Transform Indicator4;
	public Transform Canvas;

	public Material Open;
	public Material Close;

	public bool IsOpen;
	public bool AllowInputing;
	public bool AllowOutputing;
	public float Capacity = 20000f;
	public float EnergieAmout = 0f;

	[Range(0.4f, 30.0f)]
	public float MaxOutputingValue = 0.6f;
	[Range(0.4f, 30.0f)]
	public float MaxInputingValue = 0.6f;
	[Range(0.0f, 100.0f)]
	public float FillingPercent = 0.0f;

	// Use this for initialization
	void Start () {
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[9];
		output.DataId = new string[9] {
			"Input.EnergieAmout",
			"Input.Capacity",
			"Input.FillingPercent",
			"Input.AllowInputing", //Done
			"Input.AllowOutputing", //Done
			"Input.MaxInputingValue",
			"Input.MaxOutputingValue",
			"Input.isFull",
			"Input.isEmpty"
		};
	}
	
	// Update is called once per frame
	void Update () {
		//Recieve all Data
		if(input.SendFunctionMailNameI.Count > 0) {
			int SaveCount = input.SendFunctionMailNameI.Count;
			for(int c = 0; c < SaveCount; c++) {
				int i = 0;
				switch(input.SendFunctionMailNameI[i]) {
				case "AllowInputing":
					AllowInputing = (input.SendFunctionMailValueI[i] == "true");
					break;
				case "AllowOutputing":
					AllowOutputing = (input.SendFunctionMailValueI[i] == "true");
					break;
				case "MaxOutputingValue":
					MaxOutputingValue = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[i]), 0.0f, 30f);
					break;
				case "MaxInputingValue":
					MaxInputingValue = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[i]), 0.0f, 30f);
					break;
				}
				input.SendFunctionMailNameI.RemoveAt(i);
				input.SendFunctionMailValueI.RemoveAt(i);
			}
		}

		//Send all Data
		output.Data[0] = EnergieAmout.ToString();
		output.Data[1] = Capacity.ToString();
		output.Data[2] = FillingPercent.ToString();
		output.Data[5] = MaxInputingValue.ToString();
		output.Data[6] = MaxOutputingValue.ToString();
		if(AllowInputing) {
			output.Data[3] = "true";
		} else {
			output.Data[3] = "false";
		}
		if(AllowOutputing) {
			output.Data[4] = "true";
		} else {
			output.Data[4] = "false";
		}
		if(FillingPercent == 100) {
			output.Data[7] = "true";
		} else {
			output.Data[7] = "false";
		}
		if(FillingPercent == 0) {
			output.Data[8] = "true";
		} else {
			output.Data[8] = "false";
		}
		if(AllowInputing) {
			if(einput.EnergySource > MaxInputingValue) {
				EnergieAmout += MaxInputingValue;
				einput.EnergySource -= MaxInputingValue;
			} else {
				EnergieAmout += einput.EnergySource;
				einput.EnergySource -= einput.EnergySource;
			}
		}
		if(AllowOutputing && EnergieAmout > 0) {
			eoutput.EnergySource += Mathf.Clamp(EnergieAmout, 0.0f, MaxOutputingValue);
			EnergieAmout -= Mathf.Clamp(eoutput.EnergySource, 0.0f, MaxOutputingValue);
		}
		if(EnergieAmout > Capacity) {
			EnergieAmout = Capacity;
		} else if(0 > EnergieAmout) {
			EnergieAmout = 0;
		}
		FillingPercent = (EnergieAmout / Capacity) * 100f;
		if(IsOpen) {
			Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(0).GetComponent<Image>().fillAmount = FillingPercent * 0.01f;
			Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(4).GetComponent<Text>().text = (FillingPercent.ToString() + " %");
			Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(7).GetComponent<Text>().text = (EnergieAmout.ToString() + "/" + Capacity.ToString());
			MaxInputingValue = Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(5).GetComponent<Slider>().value;
			MaxOutputingValue = Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(6).GetComponent<Slider>().value;
			AllowInputing = Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(3).GetComponent<Toggle>().isOn;
			AllowOutputing = Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(2).GetComponent<Toggle>().isOn;
			Canvas.GetComponent<CanvasObjects>().childs[9].transform.Find("InpulseSlider").Find("InpulseSpeed").GetComponent<Text>().text = ("Inpulsing Value: " + (Mathf.Round(Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(5).GetComponent<Slider>().value*100)/100).ToString());
			Canvas.GetComponent<CanvasObjects>().childs[9].transform.Find("ExpulseSlider").Find("ExpulseSpeed").GetComponent<Text>().text = ("Expulsing Value: " + (Mathf.Round(Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(6).GetComponent<Slider>().value*100)/100).ToString());
		} else {
			Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(5).GetComponent<Slider>().value = MaxInputingValue;
			Canvas.GetComponent<CanvasObjects>().childs[9].transform.GetChild(6).GetComponent<Slider>().value = MaxOutputingValue;
		}
		if(FillingPercent < 25f) {
			Indicator1.GetComponent<MeshRenderer>().material = Close;
			Indicator2.GetComponent<MeshRenderer>().material = Close;
			Indicator3.GetComponent<MeshRenderer>().material = Close;
			Indicator4.GetComponent<MeshRenderer>().material = Close;
		} else if(FillingPercent >= 25f && FillingPercent < 50f) {
			Indicator1.GetComponent<MeshRenderer>().material = Open;
			Indicator2.GetComponent<MeshRenderer>().material = Close;
			Indicator3.GetComponent<MeshRenderer>().material = Close;
			Indicator4.GetComponent<MeshRenderer>().material = Close;
		} else if(FillingPercent >= 50f && FillingPercent < 75f) {
			Indicator1.GetComponent<MeshRenderer>().material = Open;
			Indicator2.GetComponent<MeshRenderer>().material = Open;
			Indicator3.GetComponent<MeshRenderer>().material = Close;
			Indicator4.GetComponent<MeshRenderer>().material = Close;
		} else if(FillingPercent >= 75f && FillingPercent < 100f) {
			Indicator1.GetComponent<MeshRenderer>().material = Open;
			Indicator2.GetComponent<MeshRenderer>().material = Open;
			Indicator3.GetComponent<MeshRenderer>().material = Open;
			Indicator4.GetComponent<MeshRenderer>().material = Close;
		} else if(FillingPercent >= 100f) {
			Indicator1.GetComponent<MeshRenderer>().material = Open;
			Indicator2.GetComponent<MeshRenderer>().material = Open;
			Indicator3.GetComponent<MeshRenderer>().material = Open;
			Indicator4.GetComponent<MeshRenderer>().material = Open;
		}
	}
}
