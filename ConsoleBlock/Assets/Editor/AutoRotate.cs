using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

	public Vector3 Direction;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Time.deltaTime*Direction);
	}
}
