using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFlickering : MonoBehaviour {

    public Text text;
    public float Freq = 0.9f;
    public float Speed = 0.1f;
    public Color OnColor;
    public Color OffColor;

	// Update is called once per frame
	void Update () {
        if(Mathf.PerlinNoise(Time.unscaledTime*Speed,0f) < Freq) {
            text.color = OnColor;
        } else {
            text.color = OffColor;
        }
	}
}
