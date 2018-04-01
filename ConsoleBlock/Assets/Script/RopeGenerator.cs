using UnityEngine;
using System.Collections;

public class RopeGenerator : MonoBehaviour {

	public Transform NextObject;
	public GameObject ItemPrefab;

	// Use this for initialization
	/*void Start () {
		CreateRope(transform, NextObject, ItemPrefab);
	}*/
	
	/*void CreateRope (Transform Point1, Transform Point2, GameObject RopeItem) {
		Vector3 direction = (Point1.position - Point2.position).normalized;
		int PointNeeded = Mathf.RoundToInt(Vector3.Distance(Point1.position, Point2.position) * 5);
		int ReturnValue = 0;
		float Intersection = Vector3.Distance(Point1.position, Point2.position) / PointNeeded;
		FixedJoint StartPoint = Point1.GetComponent<FixedJoint>();
		FixedJoint EndPoint = Point2.GetComponent<FixedJoint>();
		HingeJoint currentPoint = null;
		HingeJoint lastPoint = null;
		while(ReturnValue < PointNeeded) {
			if(ReturnValue != 0) {
				GameObject Point = (GameObject)Instantiate(RopeItem, Point1.position + (Intersection * ReturnValue) * -direction, Quaternion.identity);
				Point.name = "Point(" + ReturnValue + ")";
				currentPoint = Point.GetComponent<HingeJoint>();
				if(ReturnValue == 1) {
					StartPoint.connectedBody = currentPoint.GetComponent<Rigidbody>();
					currentPoint.connectedBody = StartPoint.GetComponent<Rigidbody>();
				} else {
					currentPoint.connectedBody = lastPoint.GetComponent<Rigidbody>();
				}
			}
			ReturnValue++;
			lastPoint = currentPoint;
		}
		EndPoint.connectedBody = currentPoint.GetComponent<Rigidbody>();
	}*/
}
