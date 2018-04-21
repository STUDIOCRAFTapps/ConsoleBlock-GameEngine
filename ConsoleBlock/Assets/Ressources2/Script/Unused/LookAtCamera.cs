using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {

	public Transform Player;
	float RotX;
	float RotY;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RotX += Input.GetAxis("Mouse Y") * -10f;
		RotY += Input.GetAxis("Mouse X") * 10f;
		transform.rotation = Quaternion.Euler(RotX, RotY, transform.rotation.z);
		Player.rotation = Quaternion.Euler(Player.rotation.x, RotY, Player.rotation.z);
	}
}
