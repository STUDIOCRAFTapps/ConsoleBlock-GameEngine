using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Messages : MonoBehaviour {

	public TextAsset asset; // Assign that variable through inspector
	public Transform MessageIcon;
	public Text MSText;
	private string[] assetText;
	bool IsPlaying;

	void Start() {
		assetText = new string[asset.text.Split('\n').Length];
		assetText = asset.text.Split('\n');
	}

	public void StartPlaying () {
		if(!IsPlaying) {
			StartCoroutine(PlayRandomSplashe());
		}
	}

	public void LoadTestingWorld () {
		SceneManager.LoadScene("World");
	}

	// Update is called once per frame
	public IEnumerator PlayRandomSplashe () {
		IsPlaying = true;
		int i = Random.Range(0, assetText.Length);
		if(assetText[i].StartsWith("@")) {
			MSText.text = assetText[Random.Range(0, assetText.Length)];
		} else {
			MSText.text = assetText[Random.Range(0, assetText.Length)];
		}
		MessageIcon.gameObject.SetActive(true);
		yield return new WaitForSeconds(3f);
		MessageIcon.gameObject.SetActive(false);
		IsPlaying = false;
	}

	string SpecialSplashes (int Code) {
		/*if(Code == 1) {
			return ("Happy " + UnityEngine.iOS.CalendarUnit.Weekday.ToString() + "!");
		} else if(Code == 2) {
			return ("Things about " + UnityEngine.iOS.CalendarUnit.Year.ToString());
		} else if(Code == 3) {
			if((int.Parse(UnityEngine.iOS.CalendarUnit.Day.ToString()) > 23 && int.Parse(UnityEngine.iOS.CalendarUnit.Day.ToString()) < 27) || UnityEngine.iOS.CalendarUnit.Month.ToString() == "December") {
				return ("Happy Christmas!");
			} else {
				return ("The date today is " + UnityEngine.iOS.CalendarUnit.Month.ToString() + " " + UnityEngine.iOS.CalendarUnit.Day.ToString());
			}
		} else if(Code == 4) {
			return UnityEngine.iOS.CalendarUnit.Week.ToString() + " is Nice!";
		} else if(Code == 5) {
			return "You speak " + Application.systemLanguage.ToString() + "?";
		} else if(Code == 6) {
			return Application.platform + " is a nice computer!";
		} else {
			return assetText[Random.Range(0, assetText.Length)];
		}*/
		return "Running on " + Application.platform.ToString();
	}
}
