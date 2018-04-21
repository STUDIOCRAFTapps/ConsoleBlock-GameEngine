using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Rigidbody playerRigidbody;
    public bool IsUICurrentlyOpened = false;
    public float BlockSize = 2;

    //TEMP
    public BuildingBlock.BlockType TempBuildingBlockType;
    public Transform TempPlaceHolderFloor;
    public Transform TempPlaceHolderBlock;
    public Transform TempPlaceHolderWall;
    public Transform TempPlaceHolderStair;

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
                TempBuildingBlockType = (BuildingBlock.BlockType)Mathf.Clamp((int)TempBuildingBlockType + 1,0,3);
            }
            if(Input.mouseScrollDelta.y < -0.4) {
                TempBuildingBlockType = (BuildingBlock.BlockType)Mathf.Clamp((int)TempBuildingBlockType - 1, 0, 3);
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //TEMP
        TempPlaceHolderBlock.position = Vector3.zero;
        TempPlaceHolderFloor.position = Vector3.zero;
        TempPlaceHolderStair.position = Vector3.zero;
        TempPlaceHolderWall.position = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {

            if(InputControl.GetInput/*Down*/(InputControl.InputType.MouseSecondairyPress)) {
                if(hit.collider.tag == "Interactable") {
                    WInteractable interactable = hit.collider.GetComponent<WInteractableCaller>().Call();
                    if(interactable != null) {
                        //TODO: Handle linking
                        if(hit.collider.GetComponent<WInteractableCaller>().callType == CallType.Transmition) {

                        }
                    }
                } else if(hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {

                    //TODO: EXPERIMENTAL Building Mechanics
                    Vector3 n = hit.normal;
                    n = new Vector3(Mathf.Round(n.x),Mathf.Round(n.y),Mathf.Round(n.z));
                    Vector3 r = hit.collider.transform.localEulerAngles;
                    BuildingBlock bb = hit.collider.GetComponent<BuildingBlock>();
                    float v = transform.parent.eulerAngles.y;
                    if(v > 180) {
                        v -= 360;
                    }

                    if(bb.blockType == BuildingBlock.BlockType.Block) {
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Block) {
                            TempPlaceHolderBlock.position = bb.transform.position + BlockSize * n;
                        }
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Floor) {
                            if(Mathf.Abs(n.y) > 0) {
                                TempPlaceHolderFloor.position = bb.transform.position + (BlockSize / 2) * n;
                            } else {
                                if(bb.transform.position.y < hit.point.y) {
                                    TempPlaceHolderFloor.position = bb.transform.position + BlockSize * n + Vector3.up * (BlockSize / 2);
                                } else {
                                    TempPlaceHolderFloor.position = bb.transform.position + BlockSize * n + Vector3.down * (BlockSize / 2);
                                }
                            }
                        }
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Wall) {
                            if(Mathf.Abs(n.y) <= 0) {
                                if(Mathf.Abs(n.x) == 1) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + (BlockSize / 2) * n;
                                } else if(Mathf.Abs(n.z) == 1) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + (BlockSize / 2) * n;
                                }
                            } else {
                                if(v > -45 && v < 45) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, 0, BlockSize / 2) + (Vector3.up * BlockSize * n.y);
                                } else if(v > 45 && v < 135) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(BlockSize / 2, 0, 0) + (Vector3.up * BlockSize * n.y);
                                } else if(v > 135 && v < 180 || v > -180 && v < -135) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, 0, -BlockSize / 2) + (Vector3.up * BlockSize * n.y);
                                } else if(v > -135 && v < -45) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(-BlockSize / 2, 0, 0) + (Vector3.up * BlockSize * n.y);
                                }
                            }
                        }
                    }
                    if(bb.blockType == BuildingBlock.BlockType.Floor) {
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Block) {
                            if(Mathf.Abs(n.y) > 0) {
                                TempPlaceHolderBlock.position = bb.transform.position + (BlockSize / 2 * n);
                            }
                        }
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Floor) {
                            if(Mathf.Abs(n.x) > 0) {
                                TempPlaceHolderFloor.position = bb.transform.position + BlockSize * n;
                            } else if(Mathf.Abs(n.z) > 0) {
                                TempPlaceHolderFloor.position = bb.transform.position + BlockSize * n;
                            } else if(Mathf.Abs(n.y) > 0) {
                                if(v > -45 && v < 45) {
                                    TempPlaceHolderFloor.position = bb.transform.position + new Vector3(0, 0, BlockSize);
                                } else if(v > 45 && v < 135) {
                                    TempPlaceHolderFloor.position = bb.transform.position + new Vector3(BlockSize, 0, 0);
                                } else if(v > 135 && v < 180 || v > -180 && v < -135) {
                                    TempPlaceHolderFloor.position = bb.transform.position + new Vector3(0, 0, -BlockSize);
                                } else if(v > -135 && v < -45) {
                                    TempPlaceHolderFloor.position = bb.transform.position + new Vector3(-BlockSize, 0, 0);
                                }
                            }
                        }
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Wall) {
                            if(Mathf.Abs(n.y) > 0) {
                                if(v > -45 && v < 45) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, n.y * BlockSize / 2, BlockSize / 2);
                                } else if(v > 45 && v < 135) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(BlockSize / 2, n.y * BlockSize / 2, 0);
                                } else if(v > 135 && v < 180 || v > -180 && v < -135) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, n.y * BlockSize / 2, -BlockSize / 2);
                                } else if(v > -135 && v < -45) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + new Vector3(-BlockSize / 2, n.y * BlockSize / 2, 0);
                                }
                            }
                        }
                    }
                    if(bb.blockType == BuildingBlock.BlockType.Wall) {
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Block) {
                            if(bb.transform.eulerAngles.y == 0) {
                                if(Mathf.Abs(n.z) > 0) {
                                    TempPlaceHolderBlock.position = bb.transform.position + (BlockSize / 2 * n);
                                }

                            } else if(bb.transform.eulerAngles.y == 90) {
                                if(Mathf.Abs(n.x) > 0) {
                                    TempPlaceHolderBlock.position = bb.transform.position + (BlockSize / 2 * n);
                                }
                            }
                        }
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Wall) {
                            if(bb.transform.eulerAngles.y == 0) {
                                if(Mathf.Abs(n.x) > 0 && Mathf.Abs(n.z) <= 0) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + BlockSize * n;
                                } else if(Mathf.Abs(n.y) > 0) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + BlockSize * n;
                                } else {
                                    float curdirX = hit.point.x - bb.transform.position.x;
                                    float curdirY = hit.point.y - bb.transform.position.y;

                                    if(Mathf.Abs(curdirX) > Mathf.Abs(curdirY)) {
                                        if(curdirX > 0) {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(BlockSize, 0, 0);
                                        } else {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(-BlockSize, 0, 0);
                                        }
                                    } else {
                                        if(curdirY > 0) {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, BlockSize, 0);
                                        } else {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 0, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, -BlockSize, 0);
                                        }
                                    }
                                }
                            } else if(bb.transform.eulerAngles.y == 90) {
                                if(Mathf.Abs(n.z) > 0) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + BlockSize * n;
                                } else if(Mathf.Abs(n.y) > 0) {
                                    TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                    TempPlaceHolderWall.position = bb.transform.position + BlockSize * n;
                                } else {
                                    float curdirX = hit.point.z - bb.transform.position.z;
                                    float curdirY = hit.point.y - bb.transform.position.y;

                                    if(Mathf.Abs(curdirX) > Mathf.Abs(curdirY)) {
                                        if(curdirX > 0) {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, 0, BlockSize);
                                        } else {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, 0, -BlockSize);
                                        }
                                    } else {
                                        if(curdirY > 0) {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, BlockSize, 0);
                                        } else {
                                            TempPlaceHolderWall.eulerAngles = new Vector3(0, 90, 0);
                                            TempPlaceHolderWall.position = bb.transform.position + new Vector3(0, -BlockSize, 0);
                                        }
                                    }
                                }
                            }
                        }
                        if(TempBuildingBlockType == BuildingBlock.BlockType.Wall) {

                        }
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
}
