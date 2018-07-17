using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Clean everything here
//TODO: Add stairs

public class BuildingManager : MonoBehaviour {

    public float BlockSize = 2;
    public BuildingBlock.BlockType BuildingBlockType;
    public Transform PlaceHolderObject;
    public Transform PlaceHolderFloor;
    public Transform PlaceHolderBlock;
    public Transform PlaceHolderWall;
    public Transform PlaceHolderStair;
    public GameObject ObjectFloor;
    public GameObject ObjectBlock;
    public GameObject ObjectWall;
    public GameObject ObjectStair;

    public Player player;

    public WObjectFile[] Blocks;
    public int CurrentBlock = 0;
    public int CurrentRotation = 0;

    public Vector3 RelativeDirection (Vector3 Direction, RaycastHit hit) {
        return hit.collider.GetComponent<BuildingBlock>().Parent.transform.TransformDirection(Direction);
    } 

    public Vector3 RelativeEuler (Vector3 Direction, Vector3 Rotation) {
        //return (Quaternion.Inverse(Quaternion.Euler(Direction)) * Quaternion.Euler(Rotation)).eulerAngles;
        return (Quaternion.Euler(Direction) * Quaternion.Euler(Rotation)).eulerAngles;
    }

    public float StepR (float Value, float Step) {
        return Mathf.Round(Value / Step) * Step;
    }

    public void PlaceHolderDisplay (RaycastHit hit) {
        if(BuildingBlockType == BuildingBlock.BlockType.Objects && hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
            BuildingBlock bb = hit.collider.GetComponent<BuildingBlock>();                                                              //bb: Building Block
            Vector3 pos = bb.transform.position;                                                                                        //pos: Building Block Position
            Vector3 n = bb.Parent.transform.InverseTransformDirection(hit.normal);                                                      //n: Normal
            Vector3 rn = bb.Parent.transform.TransformDirection(n);                                                                     //rn: Parent Relative Normal
            Vector3 p = bb.Parent.transform.InverseTransformPoint(hit.point);                                                           //p: Parent Relative Hit Point
            Vector3 bp = bb.transform.InverseTransformPoint(hit.point);                                                                 //bp: Block Relative Hit Point
            n = new Vector3(Mathf.Round(n.x * 1024f) / 1024f, Mathf.Round(n.y * 1024f) / 1024f, Mathf.Round(n.z * 1024f) / 1024f);
            Vector3 r = hit.collider.transform.localEulerAngles;                                                                        //r: Rotation
            Vector3 pr = bb.Parent.transform.eulerAngles;                                                                               //pr: Parent Rotation
            float v = transform.parent.eulerAngles.y - pr.y;
            //float v = transform.parent.eulerAngles.y - 180;                                                                             //v: Player Look Direction
            float vx = bb.transform.localEulerAngles.x;
            if(v > 180) {
                v -= 360;
            }
            float bth = BlockSize / 16f;                                                                                                //bth: Building Block Thickness (Floor and Walls)

            if(PlaceHolderObject.GetChild(0).GetComponent<MeshFilter>().mesh != Blocks[CurrentBlock].PlaceHolderMesh) {
                PlaceHolderObject.GetChild(0).GetComponent<MeshFilter>().mesh = Blocks[CurrentBlock].PlaceHolderMesh;
            }

            if(bb.blockType == BuildingBlock.BlockType.Cube) {
                if(n.y > 0) {
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(0, CurrentRotation * 90 + Mathf.Round(v / 90f) * 90f, 0));
                    PlaceHolderObject.position = pos + RelativeDirection((BlockSize / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), 0f, StepR(bp.z * BlockSize, 0.2f)), hit);
                } else if(n.y < 0) {
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(0, CurrentRotation * 90 + Mathf.Round(v / 90f) * 90f, 180));
                    PlaceHolderObject.position = pos + RelativeDirection((BlockSize / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), 0f, StepR(bp.z * BlockSize, 0.2f)), hit);
                } else if(n.x > 0) {
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, 0, -90));
                    PlaceHolderObject.position = pos + RelativeDirection((BlockSize / 2) * n + new Vector3(0f, StepR(bp.y * BlockSize, 0.2f), StepR(bp.z * BlockSize, 0.2f)), hit);
                } else if(n.x < 0) {
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, 180, -90));
                    PlaceHolderObject.position = pos + RelativeDirection((BlockSize / 2) * n + new Vector3(0f, StepR(bp.y * BlockSize, 0.2f), StepR(bp.z * BlockSize, 0.2f)), hit);
                } else if(n.z > 0) {
                    //PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(90, CurrentRotation * 90, 0));
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, -90, -90));
                    PlaceHolderObject.position = pos + RelativeDirection((BlockSize / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), StepR(bp.y * BlockSize, 0.2f), 0f), hit);
                } else if(n.z < 0) {
                    //PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(90, CurrentRotation * 90, 180));
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, -90, 90));
                    PlaceHolderObject.position = pos + RelativeDirection((BlockSize / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), StepR(bp.y * BlockSize, 0.2f), 0f), hit);
                }
            } else if(bb.blockType == BuildingBlock.BlockType.Floor) {
                if(n.y > 0) {
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(0, CurrentRotation * 90 + Mathf.Round(v / 90f) * 90f, 0));
                    PlaceHolderObject.position = pos + RelativeDirection((bth / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), 0f, StepR(bp.z * BlockSize, 0.2f)), hit);
                } else if(n.y < 0) {
                    PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(0, CurrentRotation * 90 + Mathf.Round(v / 90f) * 90f, 180));
                    PlaceHolderObject.position = pos + RelativeDirection((bth / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), 0f, StepR(bp.z * BlockSize, 0.2f)), hit);
                }
            } else if(bb.blockType == BuildingBlock.BlockType.Wall) {
                if(Mathf.Round(bb.transform.localEulerAngles.y) == 0) {
                    if(n.z > 0) {
                        PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, -90, -90));
                        PlaceHolderObject.position = pos + RelativeDirection((bth / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), StepR(bp.y * BlockSize, 0.2f), 0f), hit);
                    } else if(n.z < 0) {
                        PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, -90, 90));
                        PlaceHolderObject.position = pos + RelativeDirection((bth / 2) * n + new Vector3(StepR(bp.x * BlockSize, 0.2f), StepR(bp.y * BlockSize, 0.2f), 0f), hit);
                    }
                } else if(Mathf.Round(bb.transform.localEulerAngles.y) == 90) {
                    if(n.x > 0) {
                        PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, 0, -90));
                        PlaceHolderObject.position = pos + RelativeDirection((bth / 2) * n + new Vector3(0f, StepR(bp.y * BlockSize, 0.2f), -StepR(bp.x * BlockSize, 0.2f)), hit);
                    } else if(n.x < 0) {
                        PlaceHolderObject.eulerAngles = RelativeEuler(pr, new Vector3(CurrentRotation * 90 + 90, 180, -90));
                        PlaceHolderObject.position = pos + RelativeDirection((bth / 2) * n + new Vector3(0f, StepR(bp.y * BlockSize, 0.2f), -StepR(bp.x * BlockSize, 0.2f)), hit);
                    }
                }
            }


        } else if(BuildingBlockType != BuildingBlock.BlockType.Objects && hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {

            BuildingBlock bb = hit.collider.GetComponent<BuildingBlock>();                                                              //bb: Building Block
            Vector3 pos = bb.transform.position;                                                                                        //pos: Building Block Position
            Vector3 n = bb.Parent.transform.InverseTransformDirection(hit.normal);                                                      //n: Normal
            Vector3 rn = bb.Parent.transform.TransformDirection(n);                                                                     //rn: Parent Relative Normal
            Vector3 p = bb.Parent.transform.InverseTransformPoint(hit.point);                                                           //p: Parent Relative Hit Point
            Vector3 bp = bb.transform.InverseTransformPoint(hit.point);                                                                 //bp: Block Relative Hit Point
            n = new Vector3(Mathf.Round(n.x * 1024f) / 1024f, Mathf.Round(n.y * 1024f) / 1024f, Mathf.Round(n.z * 1024f) / 1024f);
            Vector3 r = hit.collider.transform.localEulerAngles;                                                                        //r: Rotation
            Vector3 pr = bb.Parent.transform.eulerAngles;                                                                               //pr: Parent Rotation
            float v = transform.parent.eulerAngles.y - pr.y;
            //float v = transform.parent.eulerAngles.y-180;                                                                               //v: Player Look Direction
            float vx = bb.transform.localEulerAngles.x;
            if(v > 180) {
                v -= 360;
            }

            if(bb.blockType == BuildingBlock.BlockType.Cube) {
                if(BuildingBlockType == BuildingBlock.BlockType.Cube) {
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
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                            PlaceHolderWall.position = pos + (BlockSize / 2) * rn;
                        } else if(Mathf.Abs(n.z) > 0) {
                            PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                            PlaceHolderWall.position = pos + (BlockSize / 2) * rn;
                        }
                    } else {
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
                if(BuildingBlockType == BuildingBlock.BlockType.Cube) {
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
                if(BuildingBlockType == BuildingBlock.BlockType.Cube) {
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
                            float curdirY = bp.y;
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
                            float curdirY = bp.y;
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
            if(bb.blockType == BuildingBlock.BlockType.Anchor) {
                if(BuildingBlockType == BuildingBlock.BlockType.Floor) {
                    PlaceHolderFloor.eulerAngles = pr;
                    if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.GlobalBased) {
                        PlaceHolderFloor.position = pos + RelativeDirection(Vector3.up * bb.mainDirection.y * bb.transform.lossyScale.y * 0.5f, hit);
                    } else if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.LocalBased) {
                        PlaceHolderFloor.position = pos + RelativeDirection(Vector3.up * bb.mainDirection.y * bb.transform.localScale.y * 0.5f, hit);
                    }
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Wall) {
                    if(v > -45 && v < 45) {
                        PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                        if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.GlobalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.lossyScale.y * 0.5f, hit);
                        } else if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.LocalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.localScale.y * 0.5f, hit);
                        }
                    } else if(v > 45 && v < 135) {
                        PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                        if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.GlobalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.lossyScale.y * 0.5f, hit);
                        } else if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.LocalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.localScale.y * 0.5f, hit);
                        }
                    } else if(v > 135 && v < 180 || v > -180 && v < -135) {
                        PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 0, 0));
                        if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.GlobalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.lossyScale.y * 0.5f, hit);
                        } else if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.LocalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.localScale.y * 0.5f, hit);
                        }
                    } else if(v > -135 && v < -45) {
                        PlaceHolderWall.eulerAngles = RelativeEuler(pr, new Vector3(0, 90, 0));
                        if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.GlobalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.lossyScale.y * 0.5f, hit);
                        } else if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.LocalBased) {
                            PlaceHolderWall.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.localScale.y * 0.5f, hit);
                        }
                    }
                }
                if(BuildingBlockType == BuildingBlock.BlockType.Cube) {
                    PlaceHolderBlock.eulerAngles = pr;
                    if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.GlobalBased) {
                        PlaceHolderBlock.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.lossyScale.y * 0.5f, hit);
                    } else if(bb.anchorConfigurator == BuildingBlock.AnchorConfigurator.LocalBased) {
                        PlaceHolderBlock.position = pos + RelativeDirection(new Vector3(0, BlockSize / 2, 0) * bb.mainDirection.y + Vector3.up * bb.mainDirection.y * bb.transform.localScale.y * 0.5f, hit);
                    }
                }
            }

            if(PlaceHolderBlock.position != Vector3.zero) {
                if(bb.Parent.ContainsChildOfSameType(PlaceHolderBlock.position, PlaceHolderBlock.eulerAngles, BuildingBlock.BlockType.Cube)) {
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
        PlaceHolderObject.position = Vector3.zero;
    }

    public void PlaceObject (RaycastHit hit) {
        if(BuildingBlockType == BuildingBlock.BlockType.Objects && hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
            GameObject obj = (GameObject)Instantiate(Blocks[CurrentBlock].Prefab, PlaceHolderObject.position, PlaceHolderObject.rotation);
            WInteractable interactable = null;
            if(obj.transform.childCount > 0) {
                interactable = obj.transform.GetChild(0).GetComponent<WInteractable>();
            }
            if(interactable != null) {
                interactable.InfinitePPSFilling = player.EnableInfintePPSFilling;
            }
            if(obj.GetComponent<Rigidbody>() == null) {
                obj.transform.parent = hit.collider.GetComponent<BuildingBlock>().Parent.transform;
                if(obj.GetComponent<WObject>() != null) {
                    obj.GetComponent<WObject>().Parent = hit.collider.GetComponent<BuildingBlock>().Parent;
                    hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Add(obj.GetComponent<WObject>());
                }
                if(obj.GetComponent<WObject>() == null && obj.transform.GetChild(0).GetComponent<WObject>() != null) {
                    obj.transform.GetChild(0).GetComponent<WObject>().Parent = hit.collider.GetComponent<BuildingBlock>().Parent;
                    hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Add(obj.transform.GetChild(0).GetComponent<WObject>());
                }
            } else {
                player.loader.PhysicObjectTracker.Add(obj.transform.GetChild(0).GetComponent<BuildingBlock>());
            }
        } else if((BuildingBlockType == BuildingBlock.BlockType.Cube || BuildingBlockType == BuildingBlock.BlockType.Floor) && hit.collider.name == "WCollider") {
            if(PlaceHolderBlock.position != Vector3.zero) {
                GameObject obj = (GameObject)Instantiate(ObjectBlock, PlaceHolderBlock.position, PlaceHolderBlock.rotation);
                obj.transform.parent = null;
                obj.GetComponent<BuildingBlock>().Parent = obj.GetComponent<BuildingBlock>();
            }
            if(PlaceHolderFloor.position != Vector3.zero) {
                GameObject obj = (GameObject)Instantiate(ObjectFloor, PlaceHolderFloor.position, PlaceHolderFloor.rotation);
                obj.transform.parent = null;
                obj.GetComponent<BuildingBlock>().Parent = obj.GetComponent<BuildingBlock>();
            }
        } else if(BuildingBlockType != BuildingBlock.BlockType.Objects && hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
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
        if(hit.collider.transform.childCount > 0) {
            if(hit.collider.transform.GetChild(hit.collider.transform.childCount-1) == player.playerRigidbody.transform) {
                return;
            }
        }
        if(hit.collider.tag == "BuildingBlock" && hit.collider.GetComponent<BuildingBlock>() != null) {
            if(player.loader.PhysicObjectTracker.Contains(hit.collider.GetComponent<BuildingBlock>())) {
                player.loader.PhysicObjectTracker.Remove(hit.collider.GetComponent<BuildingBlock>());
            }
            hit.collider.GetComponent<BuildingBlock>().Parent.Childs.Remove(hit.collider.GetComponent<BuildingBlock>());
            Destroy(hit.collider.gameObject);
        } else if(hit.collider.GetComponent<WObject>() != null) {
            if(hit.collider.GetComponent<WObject>().Parent != null) {
                hit.collider.GetComponent<WObject>().Parent.Childs.Remove(hit.collider.GetComponent<WObject>());
            }
            Destroy(hit.collider.transform.parent.gameObject);
        }
    }
}
