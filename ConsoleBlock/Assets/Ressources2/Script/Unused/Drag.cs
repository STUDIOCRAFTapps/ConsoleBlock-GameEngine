using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine;

public class Drag : MonoBehaviour {

	public int orderInParent;
	public int order;
	public Transform Parent;
	public Transform AdoptiveParent;

	public Transform ParentObj;

	Vector3 Dist;

	public int Code;

	public bool CanBeDeleted;
	public int Aligment = 0;
	bool InDragging = false;

	RectTransform rt;

	public List<GameObject> childs;
	int currentOrder;

	public bool WasWithParent = true;

	public Transform[] Sources;
	public string[] strsources;
	public string DataSyntax = "";
	public string StartSyntax = "";
	public string EndSyntax = "";
	public string[] DoesNotContains;
	public bool IsResizableBlock;
	Vector2 LastMousePos;
	Vector2 CurrentMousePos;

	/*public void OnDrag(PointerEventData eventData) {
		if(eventData.dragging) {
			Debug.Log("whut");
		}
		transform.position = Input.mousePosition;
	}*/

	void Start () {
		if(childs == null) {
			childs = new List<GameObject>();
		}
		if(Parent==null) {
			Parent = transform.parent;
		}
		if(AdoptiveParent==null) {
			AdoptiveParent = GameObject.Find("EditingSurface").transform.GetChild(0).GetChild(0).GetChild(0);
		}
		rt = transform.GetComponent<RectTransform>();

		strsources = new string[Sources.Length];

	}

	public void PreDrag () {
		Dist = transform.position - Input.mousePosition;
		CurrentMousePos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		LastMousePos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		transform.localScale = transform.localScale*1.1f;
		if(ParentObj != null) {
			BeforeEditingParent = ParentObj.gameObject;
		}
	}

	GameObject BeforeEditingParent;

	public void Dragging () {
		CurrentMousePos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		transform.eulerAngles += new Vector3(0f,0f,(LastMousePos.y-CurrentMousePos.y)*0.05f);

		if(transform.parent == AdoptiveParent) {
			if(AdoptiveParent.GetComponent<EditingGestionnairy>().obj.Contains(gameObject)) {
				AdoptiveParent.GetComponent<EditingGestionnairy>().Remove(gameObject);
			}
		}
		if(ParentObj != null) {
			if(ParentObj.GetComponent<Drag>().childs.Contains(gameObject)) {
				ParentObj.GetComponent<Drag>().RemoveChild(gameObject);
			}
		}
		bool setup = false;
		if(GetComponent<Resize>()) {
			if(transform.parent == AdoptiveParent && childs.Count > 0) {
				GetComponent<Resize>().Steps = 2;
				setup = (childs.Count > 0);
				while(childs.Count > 0) {
					DegradeChild(childs.ToArray()[childs.Count-1],BeforeEditingParent);
				}
				if(GetComponent<Drag>().ParentObj == null) {
					transform.parent.GetComponent<EditingGestionnairy>().Replace();
				} else {
					GetComponent<Drag>().ParentObj.GetComponent<Drag>().Replace();
				}
			}
		}
		Aligment = 0;
		InDragging = true;
		if(CanBeDeleted && Input.GetKey(KeyCode.Backspace)) {
			Destroy(gameObject);
		}
		transform.position = Input.mousePosition+Dist;
		if(Input.mousePosition.x < Screen.width - (Screen.width / 12) * 3f) {
			if(transform.parent == Parent) {
				transform.SetParent(AdoptiveParent);
			}
		} else {
			if(transform.parent != Parent) {
				transform.SetParent(Parent);
			}
		}
		LastMousePos = CurrentMousePos;
	}

	public void Dropping () {
		transform.localScale = transform.localScale*0.9090909091f;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
		InDragging = false;
		if(transform.parent == Parent) {
			rt.localPosition = new Vector3(rt.sizeDelta.x/2+6,-20-(order*27),0);
			if(AdoptiveParent.GetComponent<EditingGestionnairy>().obj.Contains(gameObject)) {
				AdoptiveParent.GetComponent<EditingGestionnairy>().Remove(gameObject);
			}
		} else {
			int orderS = Mathf.RoundToInt(RoundByBounds(-20-rt.localPosition.y, 27)/27);
			rt.localPosition = new Vector3(rt.sizeDelta.x/2+6+(24*Aligment),-20-(orderS*27),0);
			if(!AdoptiveParent.GetComponent<EditingGestionnairy>().obj.Contains(gameObject)) {
				//Debug.ClearDeveloperConsole();
				//Debug.Log("The inserting was call in the \"Dropping Section\"");
				AdoptiveParent.GetComponent<EditingGestionnairy>().InsertAt(orderS, gameObject);
			} else {
				//Debug.ClearDeveloperConsole();
				//Debug.Log("The inserting was call in the \"Dropping Section/Change\"");
				AdoptiveParent.GetComponent<EditingGestionnairy>().Remove(gameObject);
				AdoptiveParent.GetComponent<EditingGestionnairy>().InsertAt(orderS, gameObject);
			}
		}
		//transform.GetComponent<RectTransform>().localPosition = new Vector3(69,0,0);
		SearchParent();
	}

	public float RoundByBounds (float Value, float Bound) {
		return Mathf.RoundToInt(Value / Bound) * Bound;
	}

	void Update () {
		if(AdoptiveParent!=null&&ParentObj==null) {
			if(AdoptiveParent.GetComponent<EditingGestionnairy>().obj.Contains(gameObject)) {
				orderInParent = AdoptiveParent.GetComponent<EditingGestionnairy>().obj.IndexOf(gameObject);
			}
		} else if(AdoptiveParent!=null) {
			if(ParentObj.GetComponent<Drag>().childs.Contains(gameObject)) {
				orderInParent = ParentObj.GetComponent<Drag>().childs.IndexOf(gameObject);
			}
		}
		//Replace();
		/*if(ParentObj != null) {
			if(!ParentObj.GetComponent<Drag>().childs.Contains(gameObject)) {
				ParentObj.GetComponent<Drag>().AddChild(gameObject, Mathf.Clamp(order,0,ParentObj.GetComponent<Resize>().Steps-1));
			}
		}*/

		if(transform.parent == AdoptiveParent && !InDragging) {
			//short orderS = short.Parse((RoundByBounds(-20-transform.GetComponent<RectTransform>().localPosition.y, 27)/27).ToString());
			rt.localPosition = new Vector3(rt.sizeDelta.x/2+6+(24*Aligment),rt.localPosition.y,0);
			order = Mathf.RoundToInt(1-RoundByBounds((rt.localPosition.y)/27,1)-2);
		}
		if(WasWithParent == false && transform.parent == Parent) {
			Destroy(gameObject);
		}
		WasWithParent = (transform.parent == Parent);

		if(gameObject.activeInHierarchy) {
			for(int i = 0; i < Sources.Length; i++) {
				if(Sources[i].GetComponent<InputField>() != null) {
					strsources[i] = Sources[i].GetComponent<InputField>().text;
				} else if(Sources[i].GetComponent<Dropdown>() != null) {
					strsources[i] = Sources[i].GetComponent<Dropdown>().options.ToArray()[Sources[i].GetComponent<Dropdown>().value].text;
				} else if(Sources[i].GetComponent<CallNumberEditor>() != null) {
					strsources[i] = Sources[i].GetComponent<CallNumberEditor>().DataValue;
				} else if(Sources[i].GetComponent<Text>() != null) {
					strsources[i] = Sources[i].GetComponent<Text>().text;
				}
			}
		}
	}

	public void AddChild (GameObject childObj, int index) {
		if(gameObject.activeInHierarchy) {
			for(int i = 0; i < Sources.Length; i++) {
				if(Sources[i].GetComponent<InputField>() != null) {
					strsources[i] = Sources[i].GetComponent<InputField>().text;
				} else if(Sources[i].GetComponent<Dropdown>() != null) {
					strsources[i] = Sources[i].GetComponent<Dropdown>().options.ToArray()[Sources[i].GetComponent<Dropdown>().value].text;
				} else if(Sources[i].GetComponent<Text>() != null) {
					strsources[i] = Sources[i].GetComponent<Text>().text;
				}
			}
		}
		//Debug.Log("My name is " + order + "(" + strsources[0] + ") and my new child is "  + childObj.name + ". He will stay at index no"+index);
		int SkipsPoint = 0;
		int indexR = index;
		int Count = 0;

		//|||||||||||||||||||||||||||||||||||||||||||||||
		if(childs.Contains(childObj)) {
			for(int i = 0; Count < index; i++) {
				Count++;
				if(childs.ToArray()[i].GetComponent<Resize>()) {
					Count += childs.ToArray()[i].GetComponent<Resize>().Steps;
					indexR-=childs.ToArray()[i].GetComponent<Resize>().Steps;
				}
				//Count++;
			}

			childObj.GetComponent<Drag>().ParentObj = transform;
			//childs.Insert(indexR, childObj); //
			foreach(GameObject child in childs) {
				child.GetComponent<Drag>().Aligment=Aligment+1;
			}
			if(GetComponent<Resize>()) {
				int beforeChildCount = childs.Count;
				for(int i = 0; i < beforeChildCount; i++) {
					//obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-(i*27),0);
					childs.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(childs.ToArray()[i].GetComponent<RectTransform>().sizeDelta.x/2+6+(24*Aligment),-20-((order+i+1+SkipsPoint)*27),0);
					if(childs.ToArray()[i].GetComponent<Resize>()) {
						SkipsPoint+=childs.ToArray()[i].GetComponent<Resize>().Steps;
					}
				}
				GetComponent<Resize>().Steps = childs.Count+SkipsPoint+2;
			}
			return;
		}
		//|||||||||||||||||||||||||||||||||||||||||||||||
		if(childs.Count > 0) {
			for(int i = 0; Count < Mathf.Clamp(index, 0, childs.Count); i++) {
				Count++;
				//Debug.Log("I: " + i + ", " + childs.Count);
				if(childs.ToArray()[i].GetComponent<Resize>()) {
					Count += childs.ToArray()[i].GetComponent<Resize>().Steps;
					indexR-=childs.ToArray()[i].GetComponent<Resize>().Steps;
				}
				//Count++;
			}
		}

		childObj.GetComponent<Drag>().ParentObj = transform;
		childs.Insert(Mathf.Clamp(indexR, 0, childs.Count), childObj); //
		foreach(GameObject child in childs) {
			child.GetComponent<Drag>().Aligment=Aligment+1;
		}
		if(GetComponent<Resize>()) {
			for(int i = 0; i < childs.Count; i++) {
				//obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-(i*27),0);
				childs.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(childs.ToArray()[i].GetComponent<RectTransform>().sizeDelta.x/2+6+(24*Aligment),-20-((order+i+1+SkipsPoint)*27),0);
				if(childs.ToArray()[i].GetComponent<Resize>()) {
					SkipsPoint+=childs.ToArray()[i].GetComponent<Resize>().Steps;
				}
			}
			GetComponent<Resize>().Steps = childs.Count+SkipsPoint+2;
		}
		if(ParentObj != null) {
			ParentObj.GetComponent<Drag>().Replace();
		}
	}

	public void DegradeChild (GameObject childObj, GameObject originalParent) {
		childs.Remove(childObj);
		childObj.GetComponent<Drag>().Aligment--;
		if(originalParent != null) {
			childObj.GetComponent<Drag>().ParentObj = originalParent.transform/*null*/;
		} else {
			childObj.GetComponent<Drag>().ParentObj = null;
		}

		if(originalParent == null) {
			transform.parent.GetComponent<EditingGestionnairy>().InsertAt(order, childObj);
			//transform.parent.GetComponent<EditingGestionnairy>().obj.Insert(orderInParent, childObj);
		} else {
			childObj.GetComponent<Drag>().ParentObj.GetComponent<Drag>().AddChild(childObj,orderInParent);
		}

		int SkipsPoint = 0;
		if(GetComponent<Resize>()) {
			for(int i = 0; i < childs.Count; i++) {
				//obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-(i*27),0);
				childs.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(childs.ToArray()[i].GetComponent<RectTransform>().sizeDelta.x/2+6+(24*Aligment),-20-((order+i+1+SkipsPoint)*27),0);
				if(childs.ToArray()[i].GetComponent<Resize>()) {
					SkipsPoint+=childs.ToArray()[i].GetComponent<Resize>().Steps;
				}
			}
			GetComponent<Resize>().Steps = childs.Count+SkipsPoint+2;
		}
		SearchParent();
	}

	public void RemoveChild (GameObject childObj) {
		childs.Remove(childObj);
		childObj.GetComponent<Drag>().Aligment--;
		childObj.GetComponent<Drag>().ParentObj = null;

		int SkipsPoint = 0;
		if(GetComponent<Resize>()) {
			for(int i = 0; i < childs.Count; i++) {
				//obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-(i*27),0);
				childs.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(childs.ToArray()[i].GetComponent<RectTransform>().sizeDelta.x/2+6+(24*Aligment),-20-((order+i+1+SkipsPoint)*27),0);
				if(childs.ToArray()[i].GetComponent<Resize>()) {
					SkipsPoint+=childs.ToArray()[i].GetComponent<Resize>().Steps;
				}
			}
			GetComponent<Resize>().Steps = childs.Count+SkipsPoint+2;
		}
		SearchParent();
	}

	public void RedistributeReplaceAction () {
		Replace();
		/*foreach(GameObject gameObj in childs) {
			gameObj.GetComponent<Drag>().RedistributeReplaceAction();
		}*/
	}

	public void SearchParent () {
		if(ParentObj != null) {
			ParentObj.GetComponent<Drag>().SearchParent();
		} else {
			AdoptiveParent.GetComponent<EditingGestionnairy>().RedistributeReplaceAction();
		}
	}

	public void Replace () {

		//There's a problem with replace... WHY? @#!$&!

		string replaceName = order.ToString();
		int SkipsPoint = 0;
		//Debug.Log("A guy named " + replaceName + " or something like that was replaced.");
		if(ParentObj != null) {
			Aligment = ParentObj.GetComponent<Drag>().Aligment+1;
		} else {
			Aligment = 0;
		}
		if(GetComponent<Resize>()) {
			for(int i = 0; i < childs.Count; i++) {
				//Debug.Log("Changing the pos of my good friend (Located at " + i + ")");

				//Note: Is ORDER good or bad? Or: What will happen before the replace function that allow a better positionning?

				//obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-(i*27),0);
				childs.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(childs.ToArray()[i].GetComponent<RectTransform>().sizeDelta.x/2+6+(24*Aligment),-20-((order+i+1+SkipsPoint)*27),0);
				childs.ToArray()[i].GetComponent<Drag>().order = Mathf.RoundToInt(1-RoundByBounds((childs.ToArray()[i].GetComponent<RectTransform>().localPosition.y)/27,1)-2);
				childs.ToArray()[i].GetComponent<Drag>().Replace();
				if(childs.ToArray()[i].GetComponent<Resize>()) {
					SkipsPoint+=childs.ToArray()[i].GetComponent<Resize>().Steps;
				}
			}
			GetComponent<Resize>().Steps = childs.Count+SkipsPoint+2;
			/*if(ParentObj != null) {
				ParentObj.GetComponent<Drag>().Replace();
			} else {
				AdoptiveParent.GetComponent<EditingGestionnairy>().Replace();
			}*/
		}
	}

	public void SetDataTag (string[] DataTags) {
		for(int i = 0; i < DataTags.Length; i++) {
			if(i < Sources.Length) {
				if(Sources[i].GetComponent<InputField>()) {
					Sources[i].GetComponent<InputField>().text = DataTags[i];
				} else if(Sources[i].GetComponent<Dropdown>()) {
					int valide = -1;
					for(int s = 0; s < Sources[i].GetComponent<Dropdown>().options.Count; s++) {
						if(Sources[i].GetComponent<Dropdown>().options[s].text == DataTags[i]) {
							valide = s;
						}
						if(valide > -1) {
							Sources[i].GetComponent<Dropdown>().value = valide;
						}
					}

				} else if(Sources[i].GetComponent<Text>() != null) {
					Sources[i].GetComponent<Text>().text = DataTags[i];
				} else if(Sources[i].GetComponent<CallNumberEditor>() != null) {
					Sources[i].GetComponent<CallNumberEditor>().SetEncoding(DataTags[i]);
				}
			}
		}
	}
}
