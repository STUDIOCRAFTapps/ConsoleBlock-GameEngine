using UnityEngine;
using System.Collections;

public class StayOriented : MonoBehaviour {

	public Rigidbody Target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Target.position;
		if(GetComponent<ParticleSystem>() != null) {
			//Debug.Log(Target.velocity.y);
			GetComponent<ParticleSystem>().Emit(Mathf.RoundToInt(Target.velocity.y * -0.05f));
		}
	}
}
