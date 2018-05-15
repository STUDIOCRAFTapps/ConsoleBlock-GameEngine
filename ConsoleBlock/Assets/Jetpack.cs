using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {

    public Rigidbody rigidbody;
    public Transform CameraTransform;
    public ParticleSystem particleSystem;
    public float JetMaxForce = 5f;
    public float JetIncrementation = 0.1f;
    public float DashForce = 15f;

    void Update () {
        if(Input.GetKey(KeyCode.Space)) {
            if(rigidbody.velocity.y < JetMaxForce) {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y + JetIncrementation, Mathf.NegativeInfinity, JetMaxForce), rigidbody.velocity.z);
            }
            var emission = particleSystem.emission;
            emission.enabled = true;
            if(Input.GetKey(KeyCode.W)) {
                rigidbody.velocity += CameraTransform.forward * DashForce;
            }
        } else {
            var emission = particleSystem.emission;
            emission.enabled = false;
        }
    }
}
