using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchCodingType : MonoBehaviour {

	public Material CodingType1Material;
	public Material CodingType2Material;

	public Sprite CodingType1Sprite;
	public Sprite CodingType2Sprite;

	public GameObject Window1;
	public GameObject Window2;

	int CurrentType = 1;

	public InputField consoleInput;
	public EditingGestionnairy edit;

	public void SwitchType () {
		if(CurrentType == 1) {
			CurrentType = 2;
			GetComponent<Image>().material = CodingType2Material;
			GetComponent<Image>().sprite = CodingType2Sprite;

			Window1.SetActive(false);
			Window2.SetActive(true);

			if(!edit.Initialized) {
				edit.PrepareLoading = true;
				edit.LoadValue = consoleInput.text;
			} else {
				edit.Load(consoleInput.text);
			}
		} else if(CurrentType == 2) {
			CurrentType = 1;
			GetComponent<Image>().material = CodingType1Material;
			GetComponent<Image>().sprite = CodingType1Sprite;

			Window1.SetActive(true);
			Window2.SetActive(false);
			edit.Save();
			consoleInput.text = edit.CompiledScript;
		}
	}
}
