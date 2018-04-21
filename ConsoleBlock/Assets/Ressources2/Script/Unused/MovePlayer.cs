using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {

	Rigidbody r;

	void Start () {
		r = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.W)) {
			r.velocity = new Vector3(r.velocity.x, r.velocity.y, 5);
		}
		if(Input.GetKey(KeyCode.S)) {
			r.velocity = new Vector3(r.velocity.x, r.velocity.y, -5);
		}
		if(Input.GetKey(KeyCode.A)) {
			r.velocity = new Vector3(-5, r.velocity.y, r.velocity.z);
		}
		if(Input.GetKey(KeyCode.D)) {
			r.velocity = new Vector3(5, r.velocity.y, r.velocity.z);
		}
		if(Input.GetKeyDown(KeyCode.Space) && r.velocity.y == 0) {
			r.velocity = new Vector3(r.velocity.x * 0.5f, 5, r.velocity.z * 0.5f);
		}
	}
}
