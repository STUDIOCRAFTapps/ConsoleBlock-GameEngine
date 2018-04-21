using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : WObject {

    public BlockType blockType;

    public enum BlockType {
        Block,
        Wall,
        Floor,
        Stair
    }

}
