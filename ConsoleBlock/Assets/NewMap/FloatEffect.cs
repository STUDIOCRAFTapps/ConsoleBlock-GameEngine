using UnityEngine;
using System.Collections;

public class FloatEffect : MonoBehaviour {

	Vector3 pos;
	Vector3 rot;
	public AnimationCurve curve;
	public float T;

	public float Speed = 0.5f;
	public float Force = 0.8f;

	public bool DoesShake = false;
	public bool ShakeZ = false;

	// Use this for initialization
	void Start () {
		pos = transform.position;
		rot = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		T+=Time.deltaTime*Speed;
		if(T > 1) {
			T -=1;
		}
		if(DoesShake && !ShakeZ) {
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0) + new Vector3(0,0,-1)*(curve.Evaluate(T) * Force)*10f;
		} else if(DoesShake && ShakeZ) {
			transform.eulerAngles = new Vector3(rot.x,transform.eulerAngles.y,transform.eulerAngles.z) + new Vector3(1,0,0)*(curve.Evaluate(T) * Force)*10f;
		}
		transform.position = pos + (Vector3.up * (curve.Evaluate(T) * Force));
	}
}
