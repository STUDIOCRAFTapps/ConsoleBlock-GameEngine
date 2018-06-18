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

	public void OnButtonIntercation() {
		if(Input.GetMouseButtonDown(1)) {
			ButtonDown = true;
		}

		if(Input.GetMouseButtonUp(1)) {
			ButtonUp = true;
		}

        ButtonPress = Input.GetMouseButton(1);

		GlobalVariable[0].source = ButtonDown;
		GlobalVariable[1].source = ButtonUp;
		GlobalVariable[2].source = ButtonPress;

		ButtonDown = false;
		ButtonUp = false;
	}

	void LateUpdate() {
		if(ButtonPress) {
			ButtonTop.localScale = new Vector3(1,0.5f,1);
		} else {
			ButtonTop.localScale = new Vector3(1,1,1);
		}
	}
}
