using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class ObjectSystem : MonoBehaviour {

	public Image[] Hotbar;
	public string[] HotbarItems;
	public Image Selector;
	public GameObject ObjectSlot;
	public string[] objectId;
	int slotWidth = 5; //LimitByWidth
	public int SelectorCount = 0;
	int SlotCount = 3;
	int ObjectCount; //LimitOfObject/Slot
	string Platform;
	int SelectedSlot;
	public Transform CanvasObject;
	public Transform MainCam;
	Player CamPlayer;
	public Sprite EmptySlotSprite;

	public bool IsInventoryOpen = false;
	bool LockOpenInventory = false;
	bool IP = false;

	public Image[] InventorySlots;
	GameObject[] allObjects;
	bool LastWasIOTool;
	bool LastWasEnergyTool;
	public Material[] mList;

	// Use this for initialization
	void Start () {
		ObjectCount = objectId.Length;
		//MainCam = transform.FindChild("MainCamera");
		CamPlayer = MainCam.GetComponent<Player>();
		InventorySlots = new Image[ObjectCount];
		HotbarItems = new string[SlotCount];
		Platform = Application.platform.ToString();
		UpdateSlot();

		//Load Inventory;
		Vector3 pos = new Vector3(0,0,0);
		Vector3 BasePos;
		BasePos = new Vector3(Hotbar[0].rectTransform.anchoredPosition3D.x + 55, Hotbar[0].rectTransform.anchoredPosition3D.y, Hotbar[0].rectTransform.anchoredPosition3D.z);
		int r = 0;
		int l = 0;
		for(int i = 0; i < ObjectCount; i++) {
			if(i == 0) {
				pos = BasePos;
			}
			else {
				if(IsSameBound(i, slotWidth)) {
					r++;
					l = -1;
				}
				l++;
				pos = new Vector3(BasePos.x + (l * 55f), BasePos.y - (r * 55f), BasePos.z);
			}
			GameObject CurrentSlot = (GameObject)Instantiate(ObjectSlot, new Vector3(0,0,0), Quaternion.identity);
			CurrentSlot.GetComponent<RectTransform>().anchoredPosition3D = pos;
			CurrentSlot.GetComponent<Image>().sprite = Resources.Load<Sprite>("icon_" + objectId[i]);
			InventorySlots[i] = CurrentSlot.GetComponent<Image>();
		}
		foreach(Image Slot in InventorySlots) {
			Slot.rectTransform.SetParent(CanvasObject.transform, false);
			Slot.gameObject.SetActive(false);
		}
		UpdateInventorySize();
		IP = false;
	}
	
	// Update is called once per frame
	void Update () {
		if((HotbarItems[SelectorCount] == "item.IOSystem") && !LastWasIOTool) {
			allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(); //allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			//Debug.Log("Hey");
			foreach(GameObject go in allObjects) {
				if(go.tag == "CI") {
					go.GetComponent<Renderer>().material = mList[1];
				}
				if(go.tag == "CO") {
					go.GetComponent<Renderer>().material = mList[3];
				}
				if(go.tag == "IOLink") {
					go.SetActive(true);
				}
			}
			allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject go in allObjects) {
				if(go.tag == "IOLink") {
					go.SetActive(true);
				}
			}
		} else if(!(HotbarItems[SelectorCount] == "item.IOSystem") && LastWasIOTool) {
			allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(); //allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			//Debug.Log("Hey");
			foreach(GameObject go in allObjects) {
				if(go.tag == "CI") {
					//Debug.Log("Hey!");
					go.GetComponent<Renderer>().material = mList[0];
				}
				if(go.tag == "CO") {
					go.GetComponent<Renderer>().material = mList[2];
				}
			}
			allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject go in allObjects) {
				if(go.tag == "IOLink") {
					go.SetActive(false);
				}
			}
		}
		LastWasIOTool = (HotbarItems[SelectorCount] == "item.IOSystem");

		if((HotbarItems[SelectorCount] == "item.EnergyJoiner") && !LastWasEnergyTool) {
			allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(); //allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			//Debug.Log("Hey");
			foreach(GameObject go in allObjects) {
				if(go.activeInHierarchy) {
					if(go.tag == "Energie") {
						go.GetComponent<Renderer>().material = mList[5];
					}
					if(go.tag == "EnergieOutput") {
						go.GetComponent<Renderer>().material = mList[7];
					}
				}
			}
			allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject go in allObjects) {
				if(go.tag == "ELink") {
					go.SetActive(true);
				}
			}
		} else if(!(HotbarItems[SelectorCount] == "item.EnergyJoiner") && LastWasEnergyTool) {
			allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(); //allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			//Debug.Log("Hey");
			foreach(GameObject go in allObjects) {
				if(go.activeInHierarchy) {
					if(go.tag == "Energie") {
						go.GetComponent<Renderer>().material = mList[4];
					}
					if(go.tag == "EnergieOutput") {
						go.GetComponent<Renderer>().material = mList[6];
					}
				}
			}
			allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach(GameObject go in allObjects) {
				if(go.tag == "ELink") {
					go.SetActive(false);
				}
			}
		}
		LastWasEnergyTool = (HotbarItems[SelectorCount] == "item.EnergyJoiner");


		if(IsInventoryOpen) {
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
		}
		if(CamPlayer.SelectedConsole != null && IsInventoryOpen) {
			IsInventoryOpen = false;
			foreach(Image Slot in InventorySlots) {
				Slot.gameObject.SetActive(false);
			}
		}
		if(Input.GetKeyUp(KeyCode.E)) {
			if(!MainCam.GetComponent<Player>().PositionLock) {
				gameObject.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		if(Input.GetKeyDown(KeyCode.Backspace) && IsInventoryOpen && string.IsNullOrEmpty(MainCam.GetComponent<Player>().WTMode)) {
			Hotbar[SelectorCount].sprite = EmptySlotSprite;
			HotbarItems[SelectorCount] = null;
		}
		if(Input.GetKeyDown(KeyCode.Return) && IsInventoryOpen && string.IsNullOrEmpty(MainCam.GetComponent<Player>().WTMode)) {
			Hotbar[SelectorCount].sprite = InventorySlots[SelectedSlot].sprite;
			HotbarItems[SelectorCount] = objectId[SelectedSlot];
		}
		if(Input.GetKeyDown(KeyCode.E) && CamPlayer.SelectedConsole == null && string.IsNullOrEmpty(MainCam.GetComponent<Player>().WTMode)) {
			IP = true;
		} else {
			IP = false;
		}
		if(Input.anyKeyDown && IsInventoryOpen && string.IsNullOrEmpty(MainCam.GetComponent<Player>().WTMode)) {
			if(Input.GetKeyDown(KeyCode.UpArrow)) {
				if(SelectedSlot - slotWidth >= 0) {
					SelectedSlot = SelectedSlot - slotWidth;
					UpdateInventorySize();
				}
			}
			if(Input.GetKeyDown(KeyCode.DownArrow)) {
				if(SelectedSlot + slotWidth < ObjectCount) {
					SelectedSlot = SelectedSlot + slotWidth;
					UpdateInventorySize();
				}
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				if(SelectedSlot - 1 >= 0) {
					SelectedSlot = SelectedSlot - 1;
					UpdateInventorySize();
				}
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)) {
				if(SelectedSlot + 1 < ObjectCount) {
					SelectedSlot = SelectedSlot + 1;
					UpdateInventorySize();
				}
			}
		}
		if(IP && !LockOpenInventory) {
			if(!IsInventoryOpen) {
				IsInventoryOpen = true;
				foreach(Image Slot in InventorySlots) {
					Slot.gameObject.SetActive(true);
				}
			} else {
				IsInventoryOpen = false;
				foreach(Image Slot in InventorySlots) {
					Slot.gameObject.SetActive(false);
				}
			}
			LockOpenInventory = true;
		}
		if(Input.GetKeyUp(KeyCode.E)) {
			LockOpenInventory = false;
		}
		if(Platform.Contains("Windows")) {
			if(Input.mouseScrollDelta.y < 0) {
					if(SelectorCount - 1 >= 0) {
						SelectorCount -= 1;
						UpdateSlot();
					}
				}
			if(Input.mouseScrollDelta.y > 0) {
				if(SelectorCount + 1 <= SlotCount - 1) {
					SelectorCount += 1;
					UpdateSlot();
				}
			}
		} else if(Platform.Contains("OSX")) {
			if(Input.mouseScrollDelta.y > 0) {
				if(SelectorCount - 1 >= 0) {
					SelectorCount -= 1;
					UpdateSlot();
				}
			}
			if(Input.mouseScrollDelta.y < 0) {
				if(SelectorCount + 1 <= SlotCount - 1) {
					SelectorCount += 1;
					UpdateSlot();
				}
			}
		}
	}

	void UpdateSlot () {
		Selector.rectTransform.position = Hotbar[SelectorCount].rectTransform.position;
	}

	void UpdateInventorySize () {
		foreach(Image Slot in InventorySlots) {
			if((Vector3.Distance(Slot.transform.position, InventorySlots[SelectedSlot].transform.position) / 10) == 0) {
				Slot.rectTransform.sizeDelta = new Vector2(80,80);
			} else {
				Slot.rectTransform.sizeDelta = new Vector2(50-(Vector3.Distance(Slot.transform.position, InventorySlots[SelectedSlot].transform.position) / 10), 50-(Vector3.Distance(Slot.transform.position, InventorySlots[SelectedSlot].transform.position) / 10));
			}
		}
	}

	bool IsSameBound (float Value, float Bound) {
		if(Value == RoundToBound(Value, Bound)) {
			return true;
		} else {
			return false;
		}

	}

	float RoundToBound (float Value, float Bound) {
		return Mathf.Round(Value / Bound) * Bound;
	}
		
	/*public static List<T> FindObjectsOfTypeAll<T>()
	{
		List<T> results = new List<T>();
		for(int i = 0; i< SceneManager.sceneCount; i++)
		{
			var s = SceneManager.GetSceneAt(i);
			if (s.isLoaded)
			{
				var allGameObjects = s.GetRootGameObjects();
				for (int j = 0; j < allGameObjects.Length; j++)
				{
					var go = allGameObjects[j];
					results.AddRange(go.GetComponentsInChildren<T>(true));
				}
			}
		}
		return results;
	}*/
}
