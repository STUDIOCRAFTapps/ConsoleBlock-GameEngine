using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationSystem : MonoBehaviour {

	public globalWorld world;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

[System.Serializable]
public class globalWorld {
	public uint globalWidth;
	public uint globalHeight;

	public sbyte globalChunkSizeX;
	public sbyte globalChunkSizeY;

	public float freezingPoint;
	public float freezingAmplitude;
	public float freezingHeightAmplitude;

	public float getNormalTemperature (uint Width, uint Height) {
		return Mathf.Abs((Height-Height/2)/(Height/2))*freezingAmplitude+freezingPoint;
	}
}
