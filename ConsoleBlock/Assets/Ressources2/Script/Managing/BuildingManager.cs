using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Clean everything here
//TODO: Add stairs

public class BuildingManager : MonoBehaviour {

    public float BlockSize = 2;
    public BuildingBlock.BlockType BuildingBlockType;
    public Transform PlaceHolderFloor;
    public Transform PlaceHolderBlock;
    public Transform PlaceHolderWall;
    public Transform PlaceHolderStair;
    public GameObject ObjectFloor;
    public GameObject ObjectBlock;
    public GameObject ObjectWall;
    public GameObject ObjectStair;

    public Vector3 RelativeDirection (Vector3 Direction, RaycastHit hit) {
        return hit.collider.GetComponent<BuildingBlock>().Parent.transform.TransformDirection(Direction);
    } 

    public Vector3 RelativeEuler (Vector3 Direction, Vector3 Rotation) {
        //return (Quaternion.Inverse(Quaternion.Euler(Direction)) * Quaternion.Euler(Rotation)).eulerAngles;
        return (Quaternion.Euler(Direction) * Quaternion.Euler(Rotation)).eulerAngles;
    }

    public void PlaceHolderDisplay (RaycastHit hit) {
        if(hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {

            BuildingBlock bb = hit.collider.GetComponent<BuildingBlock>();                                                              //bb: Building Block
            Vector3 pos = bb.transform.position;                                                                                        //pos: Building Block Position
            Vector3 n = bb.Parent.transform.InverseTransformDirection(hit.normal);                                                   //n: Normal
            Vector3 rn = bb.Parent.transform.TransformDirection(n);                                                                     //rn: Parent Relative Normal
            Vector3 p = bb.Parent.transform.InverseTransformPoint(hit.point);                                                           //p: Parent Relative Hit Point
            n = new Vector3(Mathf.Round(n.x * 1024f) / 1024f, Mathf.Round(n.y * 1024f) / 1024f, Mathf.Round(n.z * 1024f) / 1024f);
            Vector3 r = hit.collider.transform.localEulerAngles;                                                                        //r: Rotation
            Vector3 pr = bb.Parent.transform.eulerAngles;                                                                               //pr: Parent Rotation
            float v = transform.parent.eulerAngles.y;                                                                                   //v: Player Look Direction
            if(v > 180) {
                v -= 360;
            }

            if(bb.blockType == BuildingBlock.BlockType.Block) {
                if(BuildingBlockType == BuildingBlock.BlockType.Block) {
                    PlaceHolderBlock.eulerAngles = pr + new Vector3(0, 0, 0);
                    PlaceHolderBlock.position = pos + rn*BlockSize;
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Floor) {
                    if(Mathf.Abs(n.y) > 0) {
                        PlaceHolderFloor.eulerAngles = pr;
                        PlaceHolderFloor.position = pos + (BlockSize / 2) * rn;
                    } else {
                        if(bb.transform.localPosition.y < p.y) {
                            PlaceHolderFloor.eulerAngles = pr;
                            PlaceHolderFloor.position = pos + RelativeDirection(BlockSize * n + Vector3.up * (BlockSize / 2), hit);
                        } else {
                            PlaceHolderFloor.eulerAngles = pr;
                            PlaceHolderFloor.position = pos + RelativeDirection(BlockSize * n + Vector3.down * (BlockSize / 2), hit);
                        }
                    }
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Wall) {
                    if(Mathf.Abs(n.y) <= 0) {
                        if(Mathf.Abs(n.x) > 0) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0)); //Add the 90o before the rotation of pr?
                            PlaceHolderWall.position = pos + (BlockSize / 2) * rn;
                        } else if(Mathf.Abs(n.z) > 0) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = pos + (BlockSize / 2) * rn;
                        }
                    } else {

                        //TODO: Localize following code
                        if(v > -45 && v < 45) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, 0, BlockSize / 2) + (Vector3.up * BlockSize * n.y), hit);
                        } else if(v > 45 && v < 135) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(BlockSize / 2, 0, 0) + (Vector3.up * BlockSize * n.y), hit);
                        } else if(v > 135 && v < 180 || v > -180 && v < -135) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, 0, -BlockSize / 2) + (Vector3.up * BlockSize * n.y), hit);
                        } else if(v > -135 && v < -45) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(-BlockSize / 2, 0, 0) + (Vector3.up * BlockSize * n.y), hit);
                        }
                    }
                }
            }
            if(bb.blockType == BuildingBlock.BlockType.Floor) {
                if(BuildingBlockType == BuildingBlock.BlockType.Block) {
                    if(Mathf.Abs(n.y) > 0) {
                        PlaceHolderBlock.eulerAngles = pr;
                        PlaceHolderBlock.position = pos + (BlockSize / 2 * rn);
                    }
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Floor) {
                    if(Mathf.Abs(n.x) > 0) {
                        PlaceHolderFloor.eulerAngles = pr;
                        PlaceHolderFloor.position = pos + BlockSize * rn;
                    } else if(Mathf.Abs(n.z) > 0) {
                        PlaceHolderFloor.eulerAngles = pr;
                        PlaceHolderFloor.position = pos + BlockSize * rn;
                    } else if(Mathf.Abs(n.y) > 0) {
                        if(v > -45 && v < 45) {
                            PlaceHolderFloor.eulerAngles = pr;
                            PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(0, 0, BlockSize), hit);
                        } else if(v > 45 && v < 135) {
                            PlaceHolderFloor.eulerAngles = pr;
                            PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(BlockSize, 0, 0), hit);
                        } else if(v > 135 && v < 180 || v > -180 && v < -135) {
                            PlaceHolderFloor.eulerAngles = pr;
                            PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(0, 0, -BlockSize), hit);
                        } else if(v > -135 && v < -45) {
                            PlaceHolderFloor.eulerAngles = pr;
                            PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(-BlockSize, 0, 0), hit);
                        }
                    }
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Wall) {
                    if(Mathf.Abs(n.y) > 0) {
                        if(v > -45 && v < 45) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = bb.transform.position + RelativeDirection(new Vector3(0, n.y * BlockSize / 2, BlockSize / 2), hit);
                        } else if(v > 45 && v < 135) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                            PlaceHolderWall.position = bb.transform.position + RelativeDirection(new Vector3(BlockSize / 2, n.y * BlockSize / 2, 0), hit);
                        } else if(v > 135 && v < 180 || v > -180 && v < -135) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = bb.transform.position + RelativeDirection(new Vector3(0, n.y * BlockSize / 2, -BlockSize / 2), hit);
                        } else if(v > -135 && v < -45) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                            PlaceHolderWall.position = bb.transform.position + RelativeDirection(new Vector3(-BlockSize / 2, n.y * BlockSize / 2, 0), hit);
                        }
                    }
                }
            }
            if(bb.blockType == BuildingBlock.BlockType.Wall) {
                if(BuildingBlockType == BuildingBlock.BlockType.Block) {
                    if(Mathf.Round(bb.transform.localEulerAngles.y) == 0) {
                        if(Mathf.Abs(n.z) > 0) {
                            PlaceHolderBlock.eulerAngles = pr;
                            PlaceHolderBlock.position = pos + (BlockSize / 2 * rn);
                        }

                    } else if(Mathf.Round(bb.transform.localEulerAngles.y) == 90) {
                        if(Mathf.Abs(n.x) > 0) {
                            PlaceHolderBlock.eulerAngles = pr;
                            PlaceHolderBlock.position = pos + (BlockSize / 2 * rn);
                        }
                    }
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Wall) {
                    if(Mathf.Round(bb.transform.localEulerAngles.y) == 0) {
                        if(Mathf.Abs(n.x) > 0) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = pos + BlockSize * rn;
                        } else if(Mathf.Abs(n.y) > 0) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = pos + BlockSize * rn;
                        } else {
                            float curdirX = bb.Parent.transform.InverseTransformPoint(hit.point).x - bb.Parent.transform.InverseTransformPoint(hit.collider.transform.position).x;
                            float curdirY = bb.Parent.transform.InverseTransformPoint(hit.point).y - bb.Parent.transform.InverseTransformPoint(hit.collider.transform.position).y;

                            float rot = 0f;
                            float blockAmp = 1f;
                            float twist = 0f;

                            if(!(v > -45 && v < 45 || (v > 135 && v < 180 || v > -180 && v < -135))) {
                                rot = 90f;
                                blockAmp = 0.5f;
                                twist = n.z;
                            }

                            if(Mathf.Abs(curdirX) > Mathf.Abs(curdirY)) {
                                if(curdirX > 0) {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, rot, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(BlockSize * blockAmp, 0, twist * BlockSize / 2),hit);
                                } else {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, rot, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(-BlockSize * blockAmp, 0, twist * BlockSize / 2),hit);
                                }
                            } else {
                                if(curdirY > 0) {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize, 0),hit);
                                } else {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, -BlockSize, 0),hit);
                                }
                            }
                        }
                    } else if(Mathf.Round(bb.transform.localEulerAngles.y) == 90) {
                        if(Mathf.Abs(n.z) > 0) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                            PlaceHolderWall.position = pos + BlockSize * rn;
                        } else if(Mathf.Abs(n.y) > 0) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                            PlaceHolderWall.position = pos + BlockSize * rn;
                        } else {
                            float curdirX = bb.Parent.transform.InverseTransformPoint(hit.point).z - bb.Parent.transform.InverseTransformPoint(hit.collider.transform.position).z;
                            float curdirY = bb.Parent.transform.InverseTransformPoint(hit.point).y - bb.Parent.transform.InverseTransformPoint(hit.collider.transform.position).y;
                            Debug.Log(curdirX + ", " + curdirY);

                            float rot = 90f;
                            float blockAmp = 1f;
                            float twist = 0f;

                            if(v > -45 && v < 45 || (v > 135 && v < 180 || v > -180 && v < -135)) {
                                rot = 0f;
                                blockAmp = 0.5f;
                                twist = n.x;
                            }

                            if(Mathf.Abs(curdirX) > Mathf.Abs(curdirY)) {
                                if(curdirX > 0) {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, rot, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(twist * BlockSize / 2, 0, BlockSize * blockAmp), hit);
                                } else {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, rot, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(twist * BlockSize / 2, 0, -BlockSize * blockAmp), hit);
                                }
                            } else {
                                if(curdirY > 0) {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize, 0), hit);
                                } else {
                                    PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                                    PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, -BlockSize, 0), hit);
                                }
                            }
                        }
                    }
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Floor) {
                    if(Mathf.Round(bb.transform.localEulerAngles.y) == 0) {
                        if(n == Vector3.forward || n == Vector3.back) {
                            float curdirY = p.y - pos.y;
                            if(curdirY > 0) {
                                PlaceHolderFloor.eulerAngles = pr;
                                PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(BlockSize / 2 * n.x, BlockSize / 2, BlockSize / 2 * n.z), hit);
                            } else {
                                PlaceHolderFloor.eulerAngles = pr;
                                PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(BlockSize / 2 * n.x, -BlockSize / 2, BlockSize / 2 * n.z), hit);
                            }
                        }
                    } else if(Mathf.Round(bb.transform.localEulerAngles.y) == 90) {
                        if(n == Vector3.right || n == Vector3.left) {
                            float curdirY = p.y - pos.y;
                            if(curdirY > 0) {
                                PlaceHolderFloor.eulerAngles = pr;
                                PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(BlockSize / 2 * n.x, BlockSize / 2, BlockSize / 2 * n.z), hit);
                            } else {
                                PlaceHolderFloor.eulerAngles = pr;
                                PlaceHolderFloor.position = pos + RelativeDirection(new Vector3(BlockSize / 2 * n.x, -BlockSize / 2, BlockSize / 2 * n.z), hit);
                            }
                        }
                    }
                }
            }

            if(PlaceHolderBlock.position != Vector3.zero) {
                if(bb.Parent.ContainsChildOfSameType(PlaceHolderBlock.position, PlaceHolderBlock.eulerAngles, BuildingBlock.BlockType.Block)) {
                    PlaceHolderBlock.position = Vector3.zero;
                }
            } else if(PlaceHolderFloor.position != Vector3.zero) {
                if(bb.Parent.ContainsChildOfSameType(PlaceHolderFloor.position, PlaceHolderFloor.eulerAngles, BuildingBlock.BlockType.Floor)) {
                    PlaceHolderFloor.position = Vector3.zero;
                }
            } else if(PlaceHolderWall.position != Vector3.zero) {
                if(bb.Parent.ContainsChildOfSameType(PlaceHolderWall.position, PlaceHolderWall.eulerAngles, BuildingBlock.BlockType.Wall)) {
                    PlaceHolderWall.position = Vector3.zero;
                }
            } else if(PlaceHolderStair.position != Vector3.zero) {
                if(bb.Parent.ContainsChildOfSameType(PlaceHolderStair.position, PlaceHolderStair.eulerAngles, BuildingBlock.BlockType.Stair)) {
                    PlaceHolderStair.position = Vector3.zero;
                }
            }
        }
    }

    public void PlaceHolderClear () {
        PlaceHolderBlock.position = Vector3.zero;
        PlaceHolderFloor.position = Vector3.zero;
        PlaceHolderStair.position = Vector3.zero;
        PlaceHolderWall.position = Vector3.zero;
    }

    public void PlaceObject (RaycastHit hit) {
        if(hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
            if(PlaceHolderBlock.position != Vector3.zero) {
                GameObject obj = (GameObject)Instantiate(ObjectBlock, PlaceHolderBlock.position, PlaceHolderBlock.rotation);
                obj.transform.parent = hit.collider.GetComponent<BuildingBlock>().Parent.transform;
                obj.GetComponent<BuildingBlock>().Parent = hit.collider.GetComponent<BuildingBlock>().Parent;
                hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Add(obj.GetComponent<BuildingBlock>());
            }
            if(PlaceHolderFloor.position != Vector3.zero) {
                GameObject obj = (GameObject)Instantiate(ObjectFloor, PlaceHolderFloor.position, PlaceHolderFloor.rotation);
                obj.transform.parent = hit.collider.GetComponent<BuildingBlock>().Parent.transform;
                obj.GetComponent<BuildingBlock>().Parent = hit.collider.GetComponent<BuildingBlock>().Parent;
                hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Add(obj.GetComponent<BuildingBlock>());
            }
            if(PlaceHolderWall.position != Vector3.zero) {
                GameObject obj = (GameObject)Instantiate(ObjectWall, PlaceHolderWall.position, PlaceHolderWall.rotation);
                obj.transform.parent = hit.collider.GetComponent<BuildingBlock>().Parent.transform;
                obj.GetComponent<BuildingBlock>().Parent = hit.collider.GetComponent<BuildingBlock>().Parent;
                hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Add(obj.GetComponent<BuildingBlock>());
            }
            if(PlaceHolderStair.position != Vector3.zero) {
                GameObject obj = (GameObject)Instantiate(ObjectStair, PlaceHolderStair.position, PlaceHolderStair.rotation);
                obj.transform.parent = hit.collider.GetComponent<BuildingBlock>().Parent.transform;
                obj.GetComponent<BuildingBlock>().Parent = hit.collider.GetComponent<BuildingBlock>().Parent;
                hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Add(obj.GetComponent<BuildingBlock>());
            }
        }
    }

    public void DestroyObject (RaycastHit hit) {
        if(hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
            hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Remove(hit.collider.GetComponent<BuildingBlock>());
            Destroy(hit.collider.gameObject);
        }
    }
}
