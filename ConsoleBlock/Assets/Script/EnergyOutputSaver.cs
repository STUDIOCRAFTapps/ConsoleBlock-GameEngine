using UnityEngine;
using System.Collections;

public class EnergyOutputSaver : MonoBehaviour {

	public float EnergySource = 0;
	public float MaxOutputingLimit = 1;

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

		gameObject.name = "EO";
	}
	
	// Update is called once per frame
	void Update () {
		selS.SetActive(IsSelected);

		Name = gameObject.name;
		IsConnect = (Name.Contains("_EO") && GameObject.Find(Name.Replace("_EO", "_EI")) != null);
		if(IsConnect) {
			Friend = GameObject.Find(Name.Replace("_EO", "_EI"));
		} else {
			Friend = null;
		}

		EnergySource = Mathf.Clamp(EnergySource, 0.0f, MaxOutputingLimit);
	}
}
