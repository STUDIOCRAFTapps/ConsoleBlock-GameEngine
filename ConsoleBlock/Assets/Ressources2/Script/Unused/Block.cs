using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	const string PlayerName = "RigidBodyFPSController";
	public float limitDistance = 96f;
	Transform player;
	byte UpdateRate = 16;
	byte CurrentRate = 0;
	bool IsClose;
	MeshRenderer meshr;

	Transform particle;

	// Use this for initialization
	void Start () {
		player = GameObject.Find(PlayerName).transform; 
		particle = transform.GetChild(0);
		meshr = GetComponent<MeshRenderer>();

		UpdateRate = (byte)Mathf.Clamp(UpdateRate + Random.Range(-3,4), byte.MinValue, byte.MaxValue);
		particle.GetComponent<MeshRenderer>().material = meshr.material;
		if(Vector3.Distance(player.position, transform.position) > limitDistance) {
			IsClose = false;
		} else {
			IsClose = true;
		}
		if(IsClose) {
			particle.GetComponent<MeshRenderer>().enabled = false;
			meshr.enabled = true;
		} else {
			particle.GetComponent<MeshRenderer>().enabled = true;
			meshr.enabled = false;
		}
		particle.rotation = Quaternion.LookRotation(transform.position - player.position);
	}
	
	// Update is called once per frame
	void Update () {
		//particle.rotation = Quaternion.LookRotation(transform.position - player.position);
		if(CurrentRate == UpdateRate) {
			CurrentRate = 0;
			particle.GetComponent<MeshRenderer>().material = meshr.material;
			bool CurrentStat = IsClose;
			if(Vector3.Distance(player.position, transform.position) > limitDistance) {
				IsClose = false;
			} else {
				IsClose = true;
			}
			if(CurrentStat != IsClose) {
				if(IsClose) {
					particle.GetComponent<MeshRenderer>().enabled = false;
					meshr.enabled = true;
				} else {
					particle.GetComponent<MeshRenderer>().enabled = true;
					meshr.enabled = false;
				}
			}
		} else {
			CurrentRate++;
		}
	}
}
