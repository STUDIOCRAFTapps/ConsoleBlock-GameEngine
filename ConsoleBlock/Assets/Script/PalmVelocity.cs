using UnityEngine;
using System.Collections;

public class PalmVelocity : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frameDelivery Status Notification (Failure)
	void Update () {
		transform.root.GetComponent<Rigidbody>().velocity = new Vector3(transform.root.GetComponent<Rigidbody>().velocity.x, 10f, transform.root.GetComponent<Rigidbody>().velocity.z);
	}
}
