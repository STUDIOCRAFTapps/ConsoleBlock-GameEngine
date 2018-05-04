using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public Rigidbody playerRigidbody;
    public bool IsUICurrentlyOpened = false;

    public UIManager uiManager;
    public BuildingManager buildingManager;

    public bool SpecificTypeModeActive = false;
    public SpecificTypeModes SpecificTypeMode;
    public Image SpecificTypeModeOverlay;

    WInteractable linksource;

    void Update () {
        if(IsUICurrentlyOpened) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if(InputControl.GetInputDown(InputControl.InputType.Close)) {
                CloseUI();
            }
            return;
        }

        if(InputControl.GetInput(InputControl.InputType.Building)) {
            if(Input.mouseScrollDelta.y > 0.4) {
                uiManager.EditWidgetValue("Build_BlockType", 1);
                buildingManager.BuildingBlockType = (BuildingBlock.BlockType)uiManager.GetWidgetValue("Build_BlockType");
            }
            if(Input.mouseScrollDelta.y < -0.4) {
                uiManager.EditWidgetValue("Build_BlockType", -1);
                buildingManager.BuildingBlockType = (BuildingBlock.BlockType)uiManager.GetWidgetValue("Build_BlockType");
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        buildingManager.PlaceHolderClear();
        if(!SpecificTypeModeActive && SpecificTypeModeOverlay.enabled) {
            SpecificTypeModeOverlay.enabled = false;
        } else if(SpecificTypeModeActive && !SpecificTypeModeOverlay.enabled) {
            SpecificTypeModeOverlay.enabled = true;
        }

        if(SpecificTypeModeActive) {
            if(SpecificTypeMode == SpecificTypeModes.Link) {
                SpecificTypeModeOverlay.color = new Color(0.8f, 1f, 0.8f, 0.6f);
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {

            if(!SpecificTypeModeActive && SpecificTypeModeOverlay.enabled) {
                SpecificTypeModeOverlay.enabled = false;
            } else if(SpecificTypeModeActive && !SpecificTypeModeOverlay.enabled) {
                SpecificTypeModeOverlay.enabled = true;
            }


            if(hit.collider.GetComponent<WInteractableCaller>() != null) {
                if(hit.collider.GetComponent<WInteractableCaller>().callType == CallType.Transmition) {
                    if(InputControl.GetInputDown(InputControl.InputType.MouseSecondairyPress) && SpecificTypeModeActive && SpecificTypeMode == SpecificTypeModes.Link) {
                        WInteractable interactable = hit.collider.GetComponent<WInteractableCaller>().Call();
                        if(interactable != null && interactable != linksource) {
                            if(interactable.transmitter.sources.Contains(linksource)) {
                                interactable.transmitter.sources.Remove(linksource);
                            } else {
                                interactable.transmitter.sources.Add(linksource);
                            }
                            SpecificTypeModeActive = false;
                        }
                    }
                } else if(hit.collider.GetComponent<WInteractableCaller>().callType == CallType.TactileInteraction) {
                    hit.collider.GetComponent<WInteractableCaller>().TactileCall(
                        hit.textureCoord,
                        InputControl.GetInputDown(InputControl.InputType.MouseSecondairyPress),
                        InputControl.GetInput(InputControl.InputType.MouseSecondairyPress),
                        InputControl.GetInputUp(InputControl.InputType.MouseSecondairyPress)
                    );
                }
            } else if(InputControl.GetInputDown(InputControl.InputType.MouseSecondairyPress) && !InputControl.GetInput(InputControl.InputType.Building)) {
                if(hit.collider.tag == "Interactable") {
                    WInteractable interactable = hit.collider.GetComponent<WInteractableCaller>().Call();
                    if(interactable != null) {
                        //TODO: Handle linking
                        if(hit.collider.GetComponent<WInteractableCaller>().callType == CallType.Transmition) {
                            SpecificTypeModeActive = true;
                            SpecificTypeMode = SpecificTypeModes.Link;
                            linksource = interactable;
                        }
                    }
                }
            } else if(InputControl.GetInput(InputControl.InputType.Building)) {
                buildingManager.PlaceHolderDisplay(hit);

                if(InputControl.GetInputDown(InputControl.InputType.MouseSecondairyPress)) {
                    if(hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
                        buildingManager.PlaceObject(hit);
                    }
                }
                if(InputControl.GetInputDown(InputControl.InputType.MouseMainPress)) {
                    if(hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
                        buildingManager.DestroyObject(hit);
                    }
                }
            }
        }
    }
    
    public void OpenUI () {
        IsUICurrentlyOpened = true;
    }

    public void CloseUI () {
        IsUICurrentlyOpened = false;
    }

    public enum SpecificTypeModes {
        Link
    }
}
