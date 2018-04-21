using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col) {
		if(col.GetComponent<Rigidbody>()) {
			col.transform.position = target.position+(Vector3.up*1.9f);
			col.GetComponent<Rigidbody>().velocity = new Vector3(col.GetComponent<Rigidbody>().velocity.x*1.1f,3.2f,col.GetComponent<Rigidbody>().velocity.z*1.1f);
		}
	}
}
