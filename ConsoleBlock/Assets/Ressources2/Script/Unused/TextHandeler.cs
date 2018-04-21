using UnityEngine;
using System.Collections;

public class TextHandeler : MonoBehaviour {

	public Camera CameraObject;

	// Update is called once per frame
	void Update () {
		if(CameraObject != null) {
			//transform.rotation = Quaternion.Inverse(Quaternion.FromToRotation(transform.position, CameraObject.transform.position));
			transform.LookAt(CameraObject.transform.position);
			transform.localScale = new Vector3(0.6f / Vector3.Distance(transform.position, CameraObject.transform.position) * 5f, 0.6f / Vector3.Distance(transform.position, CameraObject.transform.position) * 5f, 1);
		}
	}
}
