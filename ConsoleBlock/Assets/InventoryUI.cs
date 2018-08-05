using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    public int SizeX = 5;
    public int SizeY = 2;
    int CSizeX = 5;
    int CSizeY = 4;

    float Steps = 22.85714285714286f;
    float Slots = 68.57142857142857f;
    float Ratio = 0.35f;

    public RectTransform inventoryRect;
    public RectTransform inventoryMask;
    public RectTransform inventorySlotTemplate;
    public BuildingManager buildingManager;
    public UIManager uiManager;
    public List<RectTransform> inventorySlots;
    public RectTransform CurrentSlotIndicator;

    public Image CloseUpIcon;
    public Text CloseUpTitle;
    public Text CloseUpDescription;

    // Use this for initialization
    void Start () {
        UpdateInventory();
        UpdateCurrentBlock();
    }
	
	// Update is called once per frame
	void Update () {
        if(SizeX != CSizeX || SizeY != CSizeY) {
            UpdateInventory();
            UpdateCurrentBlock();
        }

        if(InputControl.GetInputDown(InputControl.InputType.SelectionLeft)) {
            buildingManager.CurrentBlock -= 1;
            if(buildingManager.CurrentBlock < 0) {
                buildingManager.CurrentBlock = buildingManager.Blocks.Length-1;

            }
            UpdateCurrentBlock();
        }
        if(InputControl.GetInputDown(InputControl.InputType.SelectionRight)) {
            buildingManager.CurrentBlock += 1;
            if(buildingManager.CurrentBlock >= buildingManager.Blocks.Length) {
                buildingManager.CurrentBlock = 0;
            }
            UpdateCurrentBlock();
        }
        if(InputControl.GetInputDown(InputControl.InputType.SelectionUp)) {
            buildingManager.CurrentBlock -= SizeX;
            if(buildingManager.CurrentBlock < 0) {
                buildingManager.CurrentBlock += SizeX;

            }
            UpdateCurrentBlock();
        }
        if(InputControl.GetInputDown(InputControl.InputType.SelectionDown)) {
            buildingManager.CurrentBlock += SizeX;
            if(buildingManager.CurrentBlock >= buildingManager.Blocks.Length) {
                buildingManager.CurrentBlock -= SizeX;
            }
            UpdateCurrentBlock();
        }
    }

    void UpdateCurrentBlock () {
        CurrentSlotIndicator.anchoredPosition = inventorySlots[buildingManager.CurrentBlock].anchoredPosition;
        uiManager.widget[0].Display.sprite = buildingManager.Blocks[buildingManager.CurrentBlock].Icon;
        CloseUpIcon.sprite = buildingManager.Blocks[buildingManager.CurrentBlock].Icon;
        CloseUpTitle.text = buildingManager.Blocks[buildingManager.CurrentBlock].Name;
        CloseUpDescription.text = buildingManager.Blocks[buildingManager.CurrentBlock].Description;

        buildingManager.PlaceHolderObject.GetChild(0).transform.localPosition = buildingManager.Blocks[buildingManager.CurrentBlock].CustomPlaceHolderPosition;
        buildingManager.PlaceHolderObject.GetChild(0).transform.localScale = buildingManager.Blocks[buildingManager.CurrentBlock].CustomPlaceHolderScale;
        buildingManager.PlaceHolderObject.GetChild(0).transform.localEulerAngles = buildingManager.Blocks[buildingManager.CurrentBlock].CustomPlaceHolderRotation;
    }

    void UpdateInventory () {
        foreach(RectTransform iSlots in inventorySlots) {
            Destroy(iSlots.gameObject);
        }
        inventorySlots.Clear();

        inventoryRect.sizeDelta = new Vector2(Steps + Slots * SizeX, Steps + Slots * SizeY);
        CSizeX = SizeX;
        CSizeY = SizeY;

        for(int i = 0; i < buildingManager.Blocks.Length; i++) {
            RectTransform nSlot = Instantiate(inventorySlotTemplate, inventoryMask.transform);
            nSlot.gameObject.SetActive(true);
            nSlot.GetComponent<Image>().sprite = buildingManager.Blocks[i].Icon;
            inventorySlots.Add(nSlot);
            nSlot.anchoredPosition = new Vector2(Steps/2f+(2/Ratio)+(i-Mathf.FloorToInt(i/SizeX)*SizeX)*Slots,-Steps/2f+(-2/Ratio)+(Mathf.FloorToInt(i / SizeX))*-Slots);
        }
    }
}
