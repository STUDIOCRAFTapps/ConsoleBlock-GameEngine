using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmAnimator : MonoBehaviour {

    public float RotPS;
    float currentRot = 0f;

	// Update is called once per frame
	void Update () {
        currentRot += RotPS * Time.deltaTime;
        transform.localEulerAngles = Vector3.right * currentRot;
	}
}
