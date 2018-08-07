using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Material", menuName = "ConsoleBlock/Material File")]
public class WMaterialFile : ScriptableObject {
    public string Name;
    public string Description;
    public Sprite Icon;
    public Material Material;
}
