using UnityEngine;
using System.Collections;

public class VehiculeHolderScript : MonoBehaviour {

	public SpringJoint sj;
	Rigidbody rsave;
	public bool IsActive = true;
	public GameObject Expulser;

	void Start () {
		sj.connectedBody = ((GameObject)Instantiate(Expulser, transform.position + Vector3.up*1.55f, Quaternion.Euler(180,0,0))).GetComponent<Rigidbody>();
		rsave = sj.connectedBody;
	}

	public void SwitchMode () {
		IsActive = !IsActive;
		if(IsActive) {
			sj.connectedBody = rsave;
			sj.connectedAnchor = Vector3.up * 3.375f;
			rsave.useGravity = false;
			rsave.transform.root.rotation = Quaternion.Euler(180,0,0);
			rsave.constraints = RigidbodyConstraints.FreezeRotation;
		} else {
			sj.connectedBody = null;
			rsave.useGravity = true;

			rsave.constraints = RigidbodyConstraints.None;
		}
	}
}
