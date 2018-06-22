using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour {

    public Image image;
    public float Speed = 0.07f;
    public Sprite[] Frames;

	void Update () {
        image.sprite = Frames[(int)(Time.time*Speed)%Frames.Length];
	}
}
