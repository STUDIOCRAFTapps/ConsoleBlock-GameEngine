using UnityEngine;
using System.Collections;

public class WaterPhysic : MonoBehaviour {

	Rigidbody Self;

	// Use this for initialization
	void Start () {
		Self = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay (Collider col) {
		if(col.attachedRigidbody != null) {
			Self.velocity = col.attachedRigidbody.velocity * 1.2f;
		} else if(col != null) {
			Self.velocity = Vector3.up * Time.deltaTime * 50;
		}
	}
}