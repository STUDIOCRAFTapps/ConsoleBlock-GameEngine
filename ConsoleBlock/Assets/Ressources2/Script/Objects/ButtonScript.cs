using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : WInteractable {

	public Transform ButtonTop;

	public bool ButtonDown = false;
	public bool ButtonPress = false;
	public bool ButtonUp = false;

	private void Start () {
		GlobalVariable.Add(new Variable("IsButtonDown",VariableType.v_bool,false,new VariableParameters(true,VariableAccessType.v_readonly)));
		GlobalVariable.Add(new Variable("IsButtonUp",VariableType.v_bool,false,new VariableParameters(true,VariableAccessType.v_readonly)));
		GlobalVariable.Add(new Variable("IsButtonPress",VariableType.v_bool,false,new VariableParameters(true,VariableAccessType.v_readonly)));
	}

	override public void OnInteraction(Player player) {
		if(Input.GetMouseButtonDown(1)) {
			ButtonDown = true;
		}

		if(Input.GetMouseButtonUp(1)) {
			ButtonUp = true;
		}

		if(Input.GetMouseButton(1)) {
			ButtonPress = true;
		}

		GlobalVariable[0].source = ButtonDown;
		GlobalVariable[1].source = ButtonUp;
		GlobalVariable[2].source = ButtonPress;

		ButtonDown = false;
		ButtonPress = false;
		ButtonUp = false;
	}

	void LateUpdate() {
		if(ButtonPress) {
			ButtonTop.localScale = new Vector3(transform.localScale.x,0.5f,transform.localScale.z);
		} else {
			ButtonTop.localScale = new Vector3(transform.localScale.x,1f,transform.localScale.z);
		}
	}
}
