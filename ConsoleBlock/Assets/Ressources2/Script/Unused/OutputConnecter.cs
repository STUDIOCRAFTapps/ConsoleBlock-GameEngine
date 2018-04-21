using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutputConnecter : MonoBehaviour {
	
	public string[] DataId;
	public string[] Data;
	public List<string> SendFunctionMailNameI;
	public List<string> SendFunctionMailValueI;

	[HideInInspector]
	public bool IsConnect;
	public GameObject Friend;
	public string Name;

	public bool IsSelected = false;
	GameObject selS;

	// Use this for initialization
	void Start () {
		GameObject sel = (GameObject)Instantiate(Resources.Load("user.ConnectorOutline"), transform, false);
		sel.transform.localPosition = Vector3.zero;
		sel.transform.localScale = (Vector3.one*1.3f);
		selS = sel;

		gameObject.name = "CO";
	}
	
	// Update is called once per frame
	void Update () {
		selS.SetActive(IsSelected);

		Name = gameObject.name;
		IsConnect = (Name.Contains("_Output") && GameObject.Find(Name.Replace("_Output", "_Input")) != null);
		if(IsConnect) {
			Friend = GameObject.Find(Name.Replace("_Output", "_Input"));
		} else {
			Friend = null;
		}

		/*if(SendFunctionMailNameI.Count > 0) {
			Debug.Log(gameObject);
		}*/
		//Debug.Log(SendFunctionMailNameI.Count);
	}
}
