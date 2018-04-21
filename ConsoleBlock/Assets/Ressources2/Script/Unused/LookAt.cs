using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

	public Transform Target;
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(Target);
		transform.Rotate(-90,0,0);
	}
}
