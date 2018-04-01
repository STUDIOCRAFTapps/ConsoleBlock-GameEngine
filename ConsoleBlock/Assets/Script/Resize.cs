using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resize : MonoBehaviour {

	public int Steps = 2;

	RectTransform Bottom;
	RectTransform Middle;

	void Start () {
		Bottom = transform.GetChild(0).GetComponent<RectTransform>();
		Middle = transform.GetChild(1).GetComponent<RectTransform>();
	}

	// Update is called once per frame
	void Update () {
		Bottom.localPosition = new Vector3(-1.3f,-27.15f*Steps,0);
		Middle.sizeDelta = new Vector2(19.2f, 20.1f*(Steps+1)+7.05f*Steps);
		Middle.localPosition = new Vector3(Middle.localPosition.x,-((20.1f*(Steps+1)+7.05f*Steps)-(20.1f))/2,0);
	}
}
