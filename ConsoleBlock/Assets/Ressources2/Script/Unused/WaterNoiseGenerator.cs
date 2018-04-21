using UnityEngine;
using System.Collections;

public class WaterNoiseGenerator : MonoBehaviour {

	public GameObject WaterObject;
	public float ObjectSize = 1;
	public int Witdh = 10;
	public int Height = 10;
	public float Density = 1;
	public float Force = 1;
	public float TimeValue = 0.1f;
	public float Speed = 0.1f;

	Vector2 Position = new Vector2(0, 0);

	Transform[,] Objects;

	// Use this for initialization
	void Start () {
		Objects = new Transform[Witdh, Height];
		for(int xW = 0; xW < Witdh; xW++) {
			for(int yH = 0; yH < Witdh; yH++) {
				GameObject CurrentObject = (GameObject)Instantiate(WaterObject, transform.position + new Vector3(xW * ObjectSize, 0, yH * ObjectSize), Quaternion.identity);
				Objects[xW,yH] = CurrentObject.transform;
			}
		}
		StartCoroutine(Loop());
	}

	IEnumerator Loop () {
		while(true) {
			for(int x = 0; x < Witdh; x++) {
				for(int y = 0; y < Height; y++) {
					Objects[x,y].position = new Vector3(Objects[x,y].position.x, Mathf.PerlinNoise(((x + Position.x) / Density), ((y + Position.y) / Density)) * Force, Objects[x,y].position.z);
				}
			}
			Position = new Vector2(Position.x + Speed, Position.y + Speed);
			yield return new WaitForSeconds(TimeValue);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
