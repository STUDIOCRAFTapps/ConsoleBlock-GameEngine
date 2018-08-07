using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    public int SizeX = 5;
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
        UpdateCurrentSelection();
    }
	
	// Update is called once per frame
	void Update () {
        int SizeY = Mathf.FloorToInt(buildingManager.GetInventorySlotCount() / SizeX);
        if(SizeX != CSizeX || SizeY != CSizeY) {
            UpdateInventory();
            UpdateCurrentSelection();
        }

        if(InputControl.GetInputDown(InputControl.InputType.SelectionLeft)) {
            buildingManager.CurrentBlock -= 1;
            if(buildingManager.CurrentBlock < 0) {
                buildingManager.CurrentBlock = buildingManager.Blocks.Length-1;

            }
            UpdateCurrentSelection();
        }
        if(InputControl.GetInputDown(InputControl.InputType.SelectionRight)) {
            buildingManager.CurrentBlock += 1;
            if(buildingManager.CurrentBlock >= buildingManager.Blocks.Length) {
                buildingManager.CurrentBlock = 0;
            }
            UpdateCurrentSelection();
        }
        if(InputControl.GetInputDown(InputControl.InputType.SelectionUp)) {
            buildingManager.CurrentBlock -= SizeX;
            if(buildingManager.CurrentBlock < 0) {
                buildingManager.CurrentBlock += SizeX;

            }
            UpdateCurrentSelection();
        }
        if(InputControl.GetInputDown(InputControl.InputType.SelectionDown)) {
            buildingManager.CurrentBlock += SizeX;
            if(buildingManager.CurrentBlock >= buildingManager.Blocks.Length) {
                buildingManager.CurrentBlock -= SizeX;
            }
            UpdateCurrentSelection();
        }
    }

    void UpdateCurrentSelection () {
        CurrentSlotIndicator.anchoredPosition = inventorySlots[buildingManager.CurrentBlock].anchoredPosition;
        CloseUpIcon.sprite = buildingManager.GetSpriteAtIndex(buildingManager.CurrentBlock);
        CloseUpTitle.text = buildingManager.GetNameAtIndex(buildingManager.CurrentBlock);
        CloseUpDescription.text = buildingManager.GetDescriptionAtIndex(buildingManager.CurrentBlock);

        if(buildingManager.CurrentInventory == 0) {
            uiManager.widget[0].Display.sprite = buildingManager.Blocks[buildingManager.CurrentBlock].Icon;
            buildingManager.PlaceHolderObject.GetChild(0).transform.localPosition = buildingManager.Blocks[buildingManager.CurrentBlock].CustomPlaceHolderPosition;
            buildingManager.PlaceHolderObject.GetChild(0).transform.localScale = buildingManager.Blocks[buildingManager.CurrentBlock].CustomPlaceHolderScale;
            buildingManager.PlaceHolderObject.GetChild(0).transform.localEulerAngles = buildingManager.Blocks[buildingManager.CurrentBlock].CustomPlaceHolderRotation;
        } else if(buildingManager.CurrentInventory == 1) {
            buildingManager.CurrentMaterial = buildingManager.Materials[buildingManager.CurrentBlock];
        }
    }

    void UpdateInventory () {
        foreach(RectTransform iSlots in inventorySlots) {
            Destroy(iSlots.gameObject);
        }
        inventorySlots.Clear();

        int SizeY = Mathf.FloorToInt(buildingManager.GetInventorySlotCount() / SizeX);
        inventoryRect.sizeDelta = new Vector2(Steps + Slots * SizeX, Steps + Slots * SizeY);
        CSizeX = SizeX;
        CSizeY = SizeY;

        for(int i = 0; i < buildingManager.GetInventorySlotCount(); i++) {
            RectTransform nSlot = Instantiate(inventorySlotTemplate, inventoryMask.transform);
            nSlot.gameObject.SetActive(true);
            nSlot.GetComponent<Image>().sprite = buildingManager.GetSpriteAtIndex(i);
            inventorySlots.Add(nSlot);
            nSlot.anchoredPosition = new Vector2(Steps/2f+(2/Ratio)+(i-Mathf.FloorToInt(i/SizeX)*SizeX)*Slots,-Steps/2f+(-2/Ratio)+(Mathf.FloorToInt(i / SizeX))*-Slots);
        }
    }
}
