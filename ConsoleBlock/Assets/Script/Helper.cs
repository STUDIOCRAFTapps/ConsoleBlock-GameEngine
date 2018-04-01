using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour {
	
	public Text Message;
	public Results[] Objects;
	List<RectTransform> resultsobj;
	List<int> indexFinder;

	bool getOut = true;

	public InputField inputField;

	void Start () {
		resultsobj = new List<RectTransform>();
		indexFinder = new List<int>();
	}

	public void Open (bool GetOutput) {
		getOut = GetOutput;
		Message.text = "Enter the name of the block.";
		gameObject.SetActive(true);
	}

	public void Close () {
		for(int i = 0; i < resultsobj.Count; i++) {
			Destroy(resultsobj.ToArray()[i].gameObject);
		}
		resultsobj = new List<RectTransform>();
		gameObject.SetActive(false);
		inputField = null;
	}

	public void Search (string inputSearch) {
		List<Results> ResultList = new List<Results>();
		for(int i = 0; i < Objects.Length; i++) {
			//Debug.Log(SimilarString(Objects[i].Id, inputSearch));
			if(SimilarString(Objects[i].Id, inputSearch)>60) {
				ResultList.Add(Objects[i]);
				break;
			}
		}
		float BestScore = 0f;
		Results BestResult = null;
		for(int i = 0; i < ResultList.Count; i++) {
			if(SimilarString(ResultList.ToArray()[i].Id, inputSearch) > BestScore) {
				BestScore = SimilarString(ResultList.ToArray()[i].Id, inputSearch);
				BestResult = ResultList.ToArray()[i];
			}
		}
		if(BestResult != null) {
			DisplayResult(BestResult);
		} else {
			for(int i = 0; i < resultsobj.Count; i++) {
				Destroy(resultsobj.ToArray()[i].gameObject);
			}
			resultsobj = new List<RectTransform>();
			indexFinder = new List<int>();
			Message.text = "There's no block named \"" + inputSearch + "\" available.";
		}
	} 

	public void SetResultText (int resu) {
		inputField.text = resultsobj[resu].GetComponent<Text>().text.Split(' ')[0];
		transform.GetChild(0).GetComponent<InputField>().text = "";
		Close();
	}

	public void DisplayResult (Results res) {
		Message.text = "Here's the result for " + res.Id + ".";
		for(int i = 0; i < resultsobj.Count; i++) {
			Destroy(resultsobj.ToArray()[i].gameObject);
		}
		resultsobj = new List<RectTransform>();
		indexFinder = new List<int>();
		if(!getOut) { //GetOutput(true) or Input(false)
			for(int i = 0; i < res.InputResultsList.Length; i++) {
				resultsobj.Add(Instantiate(transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject, transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).position, Quaternion.identity, transform.GetChild(1).GetChild(0).GetChild(0)).GetComponent<RectTransform>());
				indexFinder.Add(i);
				resultsobj.ToArray()[i].GetComponent<Text>().text = res.InputResultsList[i] + " (" + res.InputType[i] + ")";
				resultsobj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(80, -70 - i * 20, 0);
				resultsobj.ToArray()[i].gameObject.SetActive(true);
				int saver = i;
				resultsobj.ToArray()[i].GetComponent<Button>().onClick.AddListener(() => SetResultText(saver));
			}
		} else {
			for(int i = 0; i < res.ResultsList.Length; i++) {
				resultsobj.Add(Instantiate(transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject, transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).position, Quaternion.identity, transform.GetChild(1).GetChild(0).GetChild(0)).GetComponent<RectTransform>());
				indexFinder.Add(i);
				resultsobj.ToArray()[i].GetComponent<Text>().text = res.ResultsList[i] + " (" + res.OutputType[i] + ")";
				resultsobj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(80, -70 - i * 20, 0);
				resultsobj.ToArray()[i].gameObject.SetActive(true);
				int saver = i;
				resultsobj.ToArray()[i].GetComponent<Button>().onClick.AddListener(() => SetResultText(saver));
			}
		}
		/*for(int i = 0; i < res.ResultsList.Length; i++) {
			resultsobj.Add(Instantiate(transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject, transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).position, Quaternion.identity, transform.GetChild(1).GetChild(0).GetChild(0)).GetComponent<RectTransform>());
			indexFinder.Add(i);
			resultsobj.ToArray()[i].GetComponent<Text>().text = res.ResultsList[i] + " (" + res.OutputType[i] + ")";
			resultsobj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(80, -70 - i * 20, 0);
			resultsobj.ToArray()[i].gameObject.SetActive(true);
			int saver = i;
			resultsobj.ToArray()[i].GetComponent<Button>().onClick.AddListener(() => SetResultText(saver));
		}*/
		transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, (168.8f + (Mathf.Clamp(resultsobj.Count-6,0,Mathf.Infinity)*30)));
	}

	public float SimilarString (string Original, string Copy) {
		int ErrorCount = 0;
		int ReadingO = 0;

		char[] CopyR = Copy.ToCharArray();
		char[] OriginalR = Original.ToCharArray();

		for(int i = 0; i < CopyR.Length; i++) {
			CopyR[i] = char.ToLower(CopyR[i]);
			if(ReadingO < OriginalR.Length) {
				//Debug.Log("Comparing " + CopyR[i] + " and " + char.ToLower(OriginalR[ReadingO]));
				if(CopyR[i] == char.ToLower(OriginalR[ReadingO])) {
					//Debug.Log("Sucessfuly Read at " + i);
					ReadingO+=1;
				} else {
					ErrorCount++;
					for(int e = ReadingO+1; e < OriginalR.Length; e++) {
						if(CopyR[i] == char.ToLower(OriginalR[e])) {
							ReadingO++;
							i--;
							break;
						}
					}
				}
			} else {
				ErrorCount++;
			}
		}
		if(OriginalR.Length > CopyR.Length) {
			ErrorCount += Mathf.RoundToInt(Mathf.Abs(OriginalR.Length - CopyR.Length) * 0.5f);
		}
		ErrorCount = Mathf.Clamp(ErrorCount, 0, CopyR.Length);
		return ((float)CopyR.Length-ErrorCount)/CopyR.Length*100f;
	}
}
	
[System.Serializable]
public class Results {
	public string Id;
	public string[] ResultsList;
	public string[] OutputType;

	public string[] InputResultsList;
	public string[] InputType;
}
