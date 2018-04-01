using UnityEngine;
using System.Collections;

public class Eolien : MonoBehaviour {

	public Transform Output;
	public Transform EnergieOutput;

	public float WindForce;
	public float ProductionPerSecond;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		WindForce = Random.Range(Random.Range(-60.0f * /*Time.time*/1, 50), Random.Range(60.0f * /*Time.time*/1, -50));
		WindForce = (WindForce + 60f) / 10;
		ProductionPerSecond = (WindForce * Random.Range(9, 11));
	}
}
