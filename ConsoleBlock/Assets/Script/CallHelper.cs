using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CallHelper : MonoBehaviour, IPointerDownHandler {

	public Helper help;
	public bool GetOutput = false;

	public void OnPointerDown (PointerEventData eventData) {
		help.Open(GetOutput);
		help.inputField = GetComponent<InputField>();
	}
}
