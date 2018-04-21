using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnergyInputSaver : MonoBehaviour {

	GameObject ReciverId;
	public float EnergySource = 0;
	public float MaxInputingLimit = 1;

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

		gameObject.name = "EI";
	}
	
	// Update is called once per frame
	void Update () {
		selS.SetActive(IsSelected);

		Name = gameObject.name;
		IsConnect = (Name.Contains("_EI") && GameObject.Find(Name.Replace("_EI", "_EO")) != null); //IConnect returns false
		if(IsConnect) {
			Friend = GameObject.Find(Name.Replace("_EI", "_EO"));
		} else {
			Friend = null;
		}
		if(!LastStats && IsConnect) {
			GameObject Line = (GameObject)Instantiate(Resources.Load("user.EnergyLink"), Vector3.one, Quaternion.identity);
			Line.name = (Name.Replace("_EI", "")+"_ELink");
			Line.GetComponent<LineRenderer>().SetPosition(0, transform.position);
			Line.GetComponent<LineRenderer>().SetPosition(1, GameObject.Find(Name.Replace("_EI", "_EO")).transform.position);
			LineName = Line.name;
		}
		if(LastStats && !IsConnect) {
			GameObject[] allObjects;
			allObjects = SceneManager.GetActiveScene().GetRootGameObjects(); //FindObjectsOfTypeAll<GameObject>().ToArray();
			foreach(GameObject go in allObjects) {
				if(go.tag == "ELink" && go.name == LineName) {
					LineName = "";
					Destroy(go);
				}
			}
		}
		LastStats = IsConnect;

		if(GameObject.Find(transform.name.Replace("_EI", "_EO")) != null) {
			if(GameObject.Find(transform.name.Replace("_EI", "_EO")) != gameObject) {
				ReciverId = GameObject.Find(name.Replace("_EI", "_EO"));
			}
		} else {
			ReciverId = null;
		}
		if(ReciverId != null) {
			if(ReciverId.GetComponent<EnergyOutputSaver>() != null) {
				EnergySource = Mathf.Clamp(ReciverId.GetComponent<EnergyOutputSaver>().EnergySource, 0.0f, MaxInputingLimit);
			}
		}
	}
}
