using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTest : MonoBehaviour {
    
    public Vector3 Rot = Vector3.zero;
    public Vector3 Move = Vector3.zero;
    public Vector3 Vel = Vector3.zero;
    public float MoveLenght = 5f;

	void Update () {
        transform.eulerAngles += Rot*Time.deltaTime;
        if(Mathf.Repeat(Time.time, MoveLenght * 2) < MoveLenght) {
            transform.position += Move * Time.deltaTime;
        } else {
            transform.position -= Move * Time.deltaTime;
        }
        if(Vel != Vector3.zero) {
            GetComponent<Rigidbody>().velocity = Vel;
        }
	}
}
