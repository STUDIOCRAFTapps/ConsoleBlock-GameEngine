using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour {

	public bool isOpen = false;
	bool LastStat = false;

	public AnimationClip anim;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isOpen != LastStat) {
			if(isOpen) {
				GetComponent<RectTransform>().rect.Set(187.5f, -216f, 187.5f, 282f);
				GetComponent<Animator>().Play("Open");
				StartCoroutine(WaitBeforeSetPos(anim.length));

			} else {
				GetComponent<RectTransform>().rect.Set(187.5f, 26f, 187.5f, 40f);
				GetComponent<Animator>().Play("Close");
			}
		}
		LastStat = isOpen;
	}

	public void Open () {
		isOpen = true;
	}

	public void Close () {
		isOpen = false;
		GetComponent<NumberGestionnairy>().PrepareForClosing();
	}

	IEnumerator WaitBeforeSetPos (float AnimLength) {
		yield return new WaitForSeconds(AnimLength);
		GetComponent<NumberGestionnairy>().SetPos();
	}
}
