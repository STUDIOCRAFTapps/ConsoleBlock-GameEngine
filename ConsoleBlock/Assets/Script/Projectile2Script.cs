using UnityEngine;
using System.Collections;

public class Projectile2Script : MonoBehaviour {
	
	Rigidbody Self;
	public Player2AI Operator;
	
	// Use this for initialization
	void Start () {
		StartCoroutine("WaitForDeath");
		Self = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		Self.velocity = new Vector3(-5, Self.velocity.y, 0);
	}
	
	void OnCollisionEnter (Collision col) {
		if(col.gameObject.name == "Player1") {
			Operator.CreateNewGeneration = true;
			Destroy(gameObject);
		}
	}
	
	IEnumerator WaitForDeath () {
		yield return new WaitForSeconds(7.5f);
		Destroy(gameObject);
	}
}
