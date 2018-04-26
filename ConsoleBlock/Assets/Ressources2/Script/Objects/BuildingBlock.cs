using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : WObject {

    public BlockType blockType;
    public BuildingBlock Parent;
    public List<BuildingBlock> Childs = new List<BuildingBlock>();

    public bool ContainsChildOfSameType (Vector3 position, Vector3 rotation, BlockType blockType) {
        foreach(BuildingBlock child in Childs) {
            if(child.blockType == blockType) {
                Vector3 p1 = new Vector3(Mathf.Round(child.transform.position.x), Mathf.Round(child.transform.position.y), Mathf.Round(child.transform.position.z));
                Vector3 p2 = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
                Vector3 r1 = new Vector3(Mathf.Round(child.transform.eulerAngles.x), Mathf.Round(child.transform.eulerAngles.y), Mathf.Round(child.transform.eulerAngles.z));
                Vector3 r2 = new Vector3(Mathf.Round(rotation.x), Mathf.Round(rotation.y), Mathf.Round(rotation.z));
                if(p1 == p2 && r1 == r2) {
                    return true;
                }
            }
        }
        return false;
    }

    public enum BlockType {
        Block,
        Wall,
        Floor,
        Stair
    }

}
