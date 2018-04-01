using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortInputField : MonoBehaviour {

	InputField inputf;

	// Use this for initialization
	void Start () {
		inputf = GetComponent<InputField>();
	}
	
	// Update is called once per frame
	void Update () {
		for(int c = 0; c < inputf.text.ToCharArray().Length; c++) {
			if(inputf.text.ToCharArray()[c] != '/' && inputf.text.ToCharArray()[c] != '1' && inputf.text.ToCharArray()[c] != '2') {
				Debug.Log("Error");
				Debug.Log(inputf.text + ", " + c);
				inputf.text = inputf.text.Remove(c,1);
				Debug.Log(inputf.text);
				inputf.text = inputf.text.Insert(c,"#");
				Debug.Log(inputf.text);
			}
		}
		inputf.text = inputf.text.Replace("#","");

		inputf.text = inputf.text.Replace("11","1/1");
		inputf.text = inputf.text.Replace("12","1/2");
		inputf.text = inputf.text.Replace("21","2/1");
		inputf.text = inputf.text.Replace("22","2/2");

		if(inputf.text.StartsWith("/")) {
			inputf.text = inputf.text.Remove(0,1);
		}
		if(inputf.text.EndsWith("/")) {
			inputf.text = inputf.text.Remove(inputf.text.Length-1,1);
		}

		inputf.text = inputf.text.Replace("//","/");
	}
}
