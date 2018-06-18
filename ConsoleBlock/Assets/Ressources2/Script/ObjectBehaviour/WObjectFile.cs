using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Object", menuName = "ConsoleBlock/Object File")]
public class WObjectFile : ScriptableObject {
    public string Name;
    public GameObject Prefab;
    public Sprite Icon;

    public Mesh PlaceHolderMesh;
    public Vector3 CustomPlaceHolderPosition = Vector3.zero;
    public Vector3 CustomPlaceHolderScale = Vector3.one;
    public Vector3 CustomPlaceHolderRotation = Vector3.zero;
}
