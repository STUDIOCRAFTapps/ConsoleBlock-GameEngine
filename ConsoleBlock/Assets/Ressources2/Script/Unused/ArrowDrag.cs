using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class ArrowDrag : MonoBehaviour {

	public string Syntax;

	public Transform Parent;
	public Transform AdoptiveParent;

	public RectTransform Guru;
	Vector3 Dist;
	public bool CanBeDeleted;
	public bool InDragging = false;
	public bool IsUnder = true;

	public bool IsFragile = false;

	public int BlockId = 0;
	public string DataValues;
	public Transform[] Sources;

	RectTransform rt;

	float LastTimeSinceTurn = 0f;
	float MaxTurnTime = 1f;

	// Use this for initialization
	void Start () {
		Parent = transform.parent;
		if(AdoptiveParent == null) {
			AdoptiveParent = transform.parent.GetChild(9).GetChild(0).GetChild(0);
		}

		rt = transform.GetComponent<RectTransform>();
	}

	public void SetDataValue (string Data) {
		if(Sources.Length > 0) {
			if(Sources[0].GetComponent<InputField>() != null) {
				Sources[0].GetComponent<InputField>().text = Data;
			} else if(Sources[0].GetComponent<Dropdown>() != null) {
				Sources[0].GetComponent<Dropdown>().value = Sources[0].GetComponent<Dropdown>().options.IndexOf(new Dropdown.OptionData(Data));
			} else if(Sources[0].GetComponent<Text>() != null) {
				Sources[0].GetComponent<Text>().text = DataValues;
				Skipper = true;
			}
		}
	}
	
	// Update is called once per frame
	bool Skipper = false;

	void Update () {
		//DataValues = "[";
		DataValues = "";
		for(int s = 0; s < Sources.Length; s++) {
			if(s > 0 && !Skipper) {
				DataValues += ",";
			} else if(Skipper) {
				Skipper = false;
			}
			if(Sources[s].GetComponent<InputField>() != null) {
				DataValues += Sources[s].GetComponent<InputField>().text;
			} else if(Sources[s].GetComponent<Dropdown>() != null) {
				DataValues += Sources[s].GetComponent<Dropdown>().options.ToArray()[Sources[s].GetComponent<Dropdown>().value].text;
			} else if(Sources[s].GetComponent<Text>() != null) {
				DataValues += Sources[s].GetComponent<Text>().text;
				Skipper = true;
			}
		}
		//DataValues += "]";
		if(IsFragile && !InDragging && IsUnder) {
			Destroy(gameObject);
		}
		if(transform.eulerAngles.z < 0) {
			transform.eulerAngles = new Vector3(0,0,360+transform.eulerAngles.z);
		}
		if(LastTimeSinceTurn > MaxTurnTime) {
			if(transform.eulerAngles.z < 180) {
				if(transform.eulerAngles.z + -2f < 0) {
					transform.eulerAngles = Vector3.zero;
				} else {
					gameObject.transform.Rotate(0,0,-2f);
				}
			} else if(transform.eulerAngles.z > 180) {
				if(transform.eulerAngles.z-360+2f > 0) {
					transform.eulerAngles = Vector3.zero;
				} else {
					gameObject.transform.Rotate(0,0,2f);
				}
			}
		} else {
			LastTimeSinceTurn += Time.deltaTime;
		}
		if(InDragging) {
			gameObject.transform.Rotate(0,0,Input.mouseScrollDelta.y*3f);
			if(Input.mouseScrollDelta.y != 0) {
				LastTimeSinceTurn = 0;
			}
		}
	}

	public void PreDrag () {
		//Debug.Log("PreDragging!");
		transform.SetAsLastSibling();
		Dist = transform.position - Input.mousePosition;
		if(Parent.GetComponent<NumberGestionnairy>().ABBoardObjects.Contains(gameObject)) {
			Parent.GetComponent<NumberGestionnairy>().RemoveABBoardObject(gameObject);
		}
	}

	public void Dragging () {
		//Debug.Log(Screen.height);
		Vector2 GuruSize = new Vector2(Guru.rect.xMax,-Guru.rect.yMin);
		Vector2 ScreenSize = new Vector2(Screen.width, Screen.height);
		Vector2 MousePosition = Input.mousePosition;
		//Debug.Log(GuruSize + " " + ScreenSize + " " + MousePosition);
	
		transform.position = new Vector3(Mathf.Clamp(MousePosition.x, (ScreenSize.x/2)+-GuruSize.x, (ScreenSize.x/2)+GuruSize.x), Mathf.Clamp(MousePosition.y, (ScreenSize.y/2)+-GuruSize.y, (ScreenSize.y/2)+GuruSize.y))+Dist;
		if(transform.position.y > (Screen.height/2)+54.5f) {
			IsUnder = false;
			transform.SetParent(AdoptiveParent);
		} else {
			IsUnder = true;
			transform.SetParent(Parent);
		}
		//Debug.Log(transform.position);
		InDragging = true;
		if(CanBeDeleted && Input.GetKey(KeyCode.Backspace)) {
			Destroy(transform.gameObject);
		}
		IsFragile = true;
	}

	public void Dropping () {
		//Debug.Log("No longer dragging!");
		InDragging = false;
		if(transform.parent == AdoptiveParent) {
			Parent.GetComponent<NumberGestionnairy>().AddABBoardObject(gameObject, true, false);
		}
	}
}
