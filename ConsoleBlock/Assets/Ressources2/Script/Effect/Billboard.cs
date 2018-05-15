using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Transform Target;
    public Vector3 Direction = Vector3.forward;

	// Update is called once per frame
	void Update () {
        Quaternion rot = Quaternion.FromToRotation(Direction, Target.position - transform.position);
        transform.rotation = Quaternion.Euler(rot.eulerAngles.x,rot.eulerAngles.y,0f);
    }
}
