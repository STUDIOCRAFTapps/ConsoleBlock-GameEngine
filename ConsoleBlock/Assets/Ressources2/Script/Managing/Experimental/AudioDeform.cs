using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDeform : MonoBehaviour {

	AudioSource audioS;
	public AnimationCurve audioDeformingCurve = AnimationCurve.Linear(0f,0f,1f,0f);
	public float deformingSpeed = 1f;
	public float deformingAmplitude = 5.0f;
	float defaultPitch;

	// Use this for initialization
	void Start () {
		audioS = GetComponent<AudioSource>();
		defaultPitch = audioS.pitch;
	}
	
	// Update is called once per frame
	void Update () {
        audioS.pitch = defaultPitch + (audioDeformingCurve.Evaluate(Time.time * deformingSpeed) * deformingAmplitude);
    }
}
