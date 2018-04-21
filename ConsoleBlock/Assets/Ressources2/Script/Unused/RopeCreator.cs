using UnityEngine;
using System.Collections;

public class RopeCreator : MonoBehaviour {

	public Transform NextPoint;
	public GameObject RopeObject;
	GameObject oldObject;
	GameObject currentObject;
	HingeJoint currentHinge;
	FixedJoint FinishPoint;
	int TotalBlock;

	int GetDistance;
	float SaveDistance;

	// Use this for initialization
	void Start () {
		currentObject = null;
		currentHinge = null;
		GetDistance = Mathf.RoundToInt(Vector3.Distance(NextPoint.position, transform.position) * 5f) - 1;
		GetDistance -= (GetDistance / 3);
		TotalBlock = GetDistance;
		Debug.Log(GetDistance);
		Vector3 direction = (transform.position - NextPoint.position).normalized;
		SaveDistance = Vector3.Distance(NextPoint.position, transform.position);
		while(GetDistance > 0) {
			if(currentObject != null) {
				oldObject = currentObject;
			} else {
				oldObject = NextPoint.gameObject;
			}
			currentObject = (GameObject)Instantiate(RopeObject, (GetDistance * (SaveDistance / TotalBlock)) * direction + NextPoint.position, Quaternion.identity);
			currentObject.name = "RopeItem (" + GetDistance + ")";
			currentHinge = currentObject.GetComponent<HingeJoint>();
			currentHinge.connectedBody = oldObject.GetComponent<Rigidbody>();
			GetDistance--;
		}
		FinishPoint = gameObject.GetComponent<FixedJoint>();
		FinishPoint.connectedBody = currentObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator StopTime () {
		yield return new WaitForSeconds(0.1f);
		//Time.timeScale = 0.0f;
	}
}
