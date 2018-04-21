using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InputConnector : MonoBehaviour {

	GameObject ReciverId;
	public string[] DataId;
	public string[] Data;
	public List<string> SendFunctionMailNameI;
	public List<string> SendFunctionMailValueI;

	public bool IsConnect;
	public GameObject Friend;
	public string Name;
	bool LastStats;
	GameObject[] allObjects;

	public bool IsSelected = false;
	GameObject selS;

	string LineName = "";

	// Use this for initialization
	void Start () {
		GameObject sel = (GameObject)Instantiate(Resources.Load("user.ConnectorOutline"), transform, false);
		sel.transform.localPosition = Vector3.zero;
		sel.transform.localScale = (Vector3.one*1.3f);
		selS = sel;

		gameObject.name = "CI";
	}
	
	// Update is called once per frame
	void Update () {
		selS.SetActive(IsSelected);

		Name = gameObject.name;
		IsConnect = (Name.Contains("_Input") && GameObject.Find(Name.Replace("_Input", "_Output")) != null);
		if(IsConnect) {
			Friend = GameObject.Find(Name.Replace("_Input", "_Output"));
		} else {
			Friend = null;
		}
		if(!LastStats && IsConnect) {
			GameObject Line = (GameObject)Instantiate(Resources.Load("user.IOSystemLink"), Vector3.one, Quaternion.identity);
			Line.name = (Name.Replace("_Input", "")+"_IOLink");
			Line.GetComponent<LineRenderer>().SetPosition(1, transform.position);
			Line.GetComponent<LineRenderer>().SetPosition(0, GameObject.Find(Name.Replace("_Input", "_Output")).transform.position);
			LineName = Line.name;
		}
		if(LastStats && !IsConnect) {
			GameObject[] allObjects;
			allObjects = SceneManager.GetActiveScene().GetRootGameObjects(); //FindObjectsOfTypeAll<GameObject>().ToArray();
			foreach(GameObject go in allObjects) {
				if(go.tag == "IOLink" && go.name == LineName) {
					LineName = "";
					Destroy(go);
				}
			}
		}
		LastStats = IsConnect;
		//MAY CAUSE BUGS or BREAK THE GAME COMPLETLY (delete unused mail, may delete mail that are not been recieve but sould have)
		//Debug.Log(SendFunctionMailNameI.Count);
		/*if(SendFunctionMailNameI.Count > 0) {
			Debug.Log("Hey!");
		}*/
		//SendFunctionMailNameI = new List<string>();
		//SendFunctionMailValueI = new List<string>();
		//-//-//-//-//-//
		if(Name.Contains("_Input") && GameObject.Find(transform.name.Replace("_Input", "_Output")) != null) {
			if(GameObject.Find(transform.name.Replace("_Input", "_Output")) != gameObject) {
				ReciverId = GameObject.Find(name.Replace("_Input", "_Output"));
			}
		} else {
			ReciverId = null;
		}
		if(ReciverId != null) {
			if(ReciverId.GetComponent<OutputConnecter>() != null) {
				if(ReciverId.GetComponent<OutputConnecter>().Data != null && ReciverId.GetComponent<OutputConnecter>().DataId != null) {
					//Debug.Log(ReciverId.GetComponent<OutputConnecter>().SendFunctionMailNameI.Count);
					Data = ReciverId.GetComponent<OutputConnecter>().Data;
					DataId = ReciverId.GetComponent<OutputConnecter>().DataId;
				}
				if(ReciverId.GetComponent<OutputConnecter>().SendFunctionMailNameI != null && ReciverId.GetComponent<OutputConnecter>().SendFunctionMailValueI != null) {
					/*if(ReciverId.GetComponent<OutputConnecter>().SendFunctionMailNameI.Count > 0) {
						Debug.Log("My name is " + gameObject.name + " and i've steal " + ReciverId.GetComponent<OutputConnecter>().SendFunctionMailNameI.Count + " items.");
					}*/
					//Debug.Log("Hey?");
					SendFunctionMailNameI = ReciverId.GetComponent<OutputConnecter>().SendFunctionMailNameI;
					SendFunctionMailValueI = ReciverId.GetComponent<OutputConnecter>().SendFunctionMailValueI;
					ReciverId.GetComponent<OutputConnecter>().SendFunctionMailNameI = new List<string>();
					ReciverId.GetComponent<OutputConnecter>().SendFunctionMailValueI = new List<string>();
				}
			}
		}
	}
}
