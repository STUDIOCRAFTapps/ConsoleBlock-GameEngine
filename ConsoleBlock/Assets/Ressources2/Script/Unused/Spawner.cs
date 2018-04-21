using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject[] objects;
	public Vector3[] pos;

	public Transform[] transforms;

	// Use this for initialization
	void Start () {
		pos = new Vector3[objects.Length];
		transforms = new Transform[objects.Length];
		//rt.localPosition = new Vector3(rt.sizeDelta.x/2+6,-20-(order*27),0);
		for(int i = 0; i < objects.Length; i++) {
			transforms[i] = objects[i].transform;
			transforms[i].localPosition = new Vector3(transforms[i].GetComponent<RectTransform>().sizeDelta.x/2+6,-20-(objects[i].GetComponent<Drag>().order*27),0);
		}
		for(int i = 0; i < objects.Length; i++) {
			pos[i] = objects[i].GetComponent<RectTransform>().localPosition;
		}
	}

	public GameObject GetClone (int Index, string[] DataTags) {
		//THIS THING HAS SOME PROBLEMS!

		//Bug: Creating blocks inside resizable block place it after a space
		//Bug: The first block (of his kind) created is oky but all other cannont be deleted, place in the second/third/fourth... layer, ect

		/*GameObject obj = (GameObject)Instantiate(objects[Index], Vector3.zero, Quaternion.identity, transform);
		obj.GetComponent<RectTransform>().localPosition = pos[Index];
		obj.name = objects[Index].name;
		transforms[Index] = obj.transform;
		GameObject MainObj = objects[Index];
		objects[Index] = obj.gameObject;

		MainObj.transform.SetParent(MainObj.GetComponent<Drag>().AdoptiveParent);
		MainObj.GetComponent<Drag>().SetDataTag(DataTags);
		return MainObj;*/

		GameObject Copy = objects[Index].gameObject;
		Copy.GetComponent<RectTransform>().SetParent(Copy.GetComponent<Drag>().AdoptiveParent);
		for(int i = 0; i < transforms.Length; i++) {
			if(transforms[i].parent != transform) {
				GameObject obj = (GameObject)Instantiate(objects[i], Vector3.zero, Quaternion.identity, transform);
				obj.GetComponent<Drag>().Parent = Copy.GetComponent<Drag>().Parent;
				obj.GetComponent<Drag>().AdoptiveParent = Copy.GetComponent<Drag>().AdoptiveParent;
				obj.transform.SetParent(transform);
				obj.GetComponent<Drag>().WasWithParent = true;
				obj.transform.localScale = Vector2.one;
				obj.GetComponent<RectTransform>().localPosition = pos[i];
				obj.name = objects[i].name;
				transforms[i] = obj.transform;
				objects[i] = obj.gameObject;
			}
		}
		Copy.GetComponent<Drag>().Aligment = 0;
		Copy.GetComponent<Drag>().order = 0;
		Copy.GetComponent<Drag>().SetDataTag(DataTags);
		return Copy;
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < transforms.Length; i++) {
			if(transforms[i].parent != transform) {
				GameObject obj = (GameObject)Instantiate(objects[i], Vector3.zero, Quaternion.identity, transform);
				obj.transform.SetParent(transform);
				obj.GetComponent<Drag>().WasWithParent = true;
				obj.transform.localScale = Vector2.one;
				obj.GetComponent<RectTransform>().localPosition = pos[i];
				obj.name = objects[i].name;
				transforms[i] = obj.transform;
				objects[i] = obj.gameObject;
			}
		}
	}

	public int GetChildIndexInOrder (int Code) {
		for(int i = 0; i < objects.Length; i++) {
			if(objects[i].GetComponent<Drag>().Code == i) {
				return i;
			}
		}
		return 0;
	}

	public GameObject[] GetChildInOrder () {
		GameObject[] List = new GameObject[transform.childCount];
		for(int i = 0; i < objects.Length; i++) {
			List[objects[i].GetComponent<Drag>().Code] = objects[i];
		}
		return List;
	}

	public Transform[] GetTransformInOrder () {
		Transform[] List = new Transform[transform.childCount];
		for(int i = 0; i < objects.Length; i++) {
			List[objects[i].GetComponent<Drag>().Code] = objects[i].transform;
		}
		return List;
	}

	public Vector3[] GetPosInOrder () {
		Vector3[] List = new Vector3[transform.childCount];
		for(int i = 0; i < pos.Length; i++) {
			List[objects[i].GetComponent<Drag>().Code] = pos[i];
		}
		return List;
	}
}
