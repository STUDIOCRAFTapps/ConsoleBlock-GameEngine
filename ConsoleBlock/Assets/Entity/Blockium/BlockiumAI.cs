using UnityEngine;
using System.Collections;

public class BlockiumAI : MonoBehaviour {

	public Transform[] Players;
	public Transform ClosestPlayer;
	public float Range;
	public float JumpForce = 5.2f;
	public float JumpHeight = 4.5f;

	Transform Block;
	bool AskForUnlock;
	bool LastUnlock;
	float CompareDist;

	// Use this for initialization
	void Start () {
		Block = transform.GetChild(3);
		transform.GetComponent<MeshRenderer>().enabled = false;
		Block.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
		//Display
		AskForUnlock = false;
		foreach(Transform player in Players) {
			if(Vector3.Distance(player.position, transform.position) <= Range) {
				AskForUnlock = true;
			}
		}
		if(!AskForUnlock && LastUnlock) {
			transform.GetComponent<MeshRenderer>().enabled = false;
			Block.gameObject.SetActive(true);
			transform.GetChild(0).GetComponent<ParticleSystem>().Emit(3);
			transform.GetComponent<Rigidbody>().isKinematic = false;
		} else if(AskForUnlock && !LastUnlock) {
			transform.GetComponent<MeshRenderer>().enabled = true;
			Block.gameObject.SetActive(false);
			transform.GetChild(0).GetComponent<ParticleSystem>().Emit(5);
			transform.GetChild(1).GetComponent<ParticleSystem>().Emit(1);
			transform.GetChild(2).GetComponent<ParticleSystem>().Emit(5);
			transform.GetComponent<Rigidbody>().isKinematic = false;
		}
		LastUnlock = AskForUnlock;

		//AI
		CompareDist = Mathf.Infinity;
		foreach(Transform player in Players) {
			if(Vector3.Distance(player.position, transform.position) <= CompareDist) {
				ClosestPlayer = player;
			}
		}
		if(Vector3.Distance(ClosestPlayer.position, transform.position) <= Range) {
			RaycastHit hit;
			Physics.Raycast(new Ray(transform.position + Vector3.down*(transform.localScale.y * 0.5f + 0.02f), Vector3.down), out hit);

			transform.localScale = new Vector3(Mathf.Clamp((transform.GetComponent<Rigidbody>().velocity.y+7)/6.2f, 0.25f, 1.75f), Mathf.Clamp((-transform.GetComponent<Rigidbody>().velocity.y+7)/6.8f, 0.15f, 1.85f) + (Mathf.Clamp(hit.distance/2,0,0.5f)-0.5f), Mathf.Clamp((transform.GetComponent<Rigidbody>().velocity.y+7)/6.2f, 0.25f, 1.75f));
			transform.LookAt(ClosestPlayer.position);
			transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y,0);
			Physics.Raycast(new Ray(transform.position + Vector3.down*(transform.localScale.y * 0.5f + 0.02f), Vector3.down), out hit);
			if(hit.distance < 0.05f && transform.localScale.y < 1.1f) {
				transform.GetComponent<Rigidbody>().velocity = MathClass.Convert.RadiusAngleToVector3_XZ(Vector3.zero,JumpForce,transform.eulerAngles.y) + Vector3.up * JumpHeight;
				transform.LookAt(ClosestPlayer.position);
				transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y,0);
			} else if(hit.distance < 0.05f) {
				//transform.GetComponent<Rigidbody>().velocity = new Vector3(transform.GetComponent<Rigidbody>().velocity.x, transform.GetComponent<Rigidbody>().velocity.y, transform.GetComponent<Rigidbody>().velocity.z);
			}
		} else {
			transform.localScale = new Vector3(1,1,1);
		}
	}
}
