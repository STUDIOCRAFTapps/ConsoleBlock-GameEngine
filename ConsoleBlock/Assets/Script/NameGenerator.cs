using UnityEngine;
using System.Collections;

public class NameGenerator : MonoBehaviour {

	public int MinName = 1;
	public int MaxName = 2;

	public string[] PartStarts;
	public string[] PartMiddleComposer;
	public string[] PartEnder;

	public int MinNamePart = 1;
	public int MaxNamePart = 5;
	

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) {
			Debug.Log(RandomName());
		}
	}

	string RandomName () {
		string Name = "";
		for(int i = 0; i < Random.Range(MinName, MaxName + 1); i++) {
			Name = (Name + PartStarts[Random.Range(0, PartStarts.Length)]);
			for(int r = 0; r < Random.Range(MinNamePart, MaxNamePart); r++) {
				Name = (Name + PartMiddleComposer[Random.Range(0, PartMiddleComposer.Length)]);
			}
			Name = (Name + PartEnder[Random.Range(0, PartEnder.Length)] + " ");
		}
		return Name;
	}
}
