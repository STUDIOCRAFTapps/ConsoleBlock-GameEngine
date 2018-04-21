using UnityEngine;
using System.Collections;

public class ZoomFOV : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.GetComponent<Camera>().fieldOfView = Mathf.Clamp(gameObject.GetComponent<Camera>().fieldOfView + Input.mouseScrollDelta.y, 0.1f, 115f);
	}
}
