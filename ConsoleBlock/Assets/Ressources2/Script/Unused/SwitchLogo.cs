using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class SwitchLogo : MonoBehaviour {

	byte Status = 0;

	public KeyCode keycode;
	public Sprite[] Logo;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(keycode)) {
			Status++; 
			if(Status >= Logo.Length) {
				Status = 0;
			}
			gameObject.GetComponent<Image>().sprite = Logo[Status];
		}
	}
}
