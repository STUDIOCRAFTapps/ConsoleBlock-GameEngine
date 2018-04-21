using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CallNumberEditor : MonoBehaviour {

	public string DataValue = "";
	public string Encoding = "";

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Button>().onClick.AddListener(() => Open());
	}

	void Open () {
		if(!GameObject.Find("Canvas").GetComponent<CanvasObjects>().childs[13].GetComponent<PlayAnimation>().isOpen) {
			GameObject.Find("Canvas").GetComponent<CanvasObjects>().childs[13].GetComponent<PlayAnimation>().isOpen = true;
			GameObject.Find("Canvas").GetComponent<CanvasObjects>().childs[13].GetComponent<NumberGestionnairy>().PrepareForOpening(gameObject);
		}
	}

	public void SetEncoding (string Data) {
		DataValue = Data;
		Encoding = GameObject.Find("Canvas").GetComponent<CanvasObjects>().childs[13].GetComponent<NumberGestionnairy>().GetEncodingByDataValue(Data);
	}
}
