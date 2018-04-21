using UnityEngine;
using System.Collections;

public class PlanetsGene : MonoBehaviour {

	public GameObject RockPrefabs;
	public GameObject BrokenRock;
	public int ChunkSize = 32;

	public float Pressision;
	public float Amplitude;

	public float StepsHeight;
	public float StepsAmplitude;
	public float StepsSize;
	public int StepsCount;

	public float CayonDraining;

	public Vector2 map;

	[Range(0.00f, 100.00f)]
	public float BrokenPercent;

	// Use this for initialization
	void Start () {
		map = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
		for(int x = 0; x < ChunkSize; x++) {
			for(int y = 0; y < StepsHeight+1; y++) {
				for(int z = 0; z < ChunkSize; z++) {
					if(Random.Range(0.00f, 100.00f) < BrokenPercent) {
						GameObject obj = (GameObject)Instantiate(BrokenRock, new Vector3(x, y + Mathf.PerlinNoise((x + map.x) / Pressision, (z + map.y) / Pressision) * Amplitude + ClosestSteps(Mathf.PerlinNoise((x + map.x) / StepsSize, (z + map.y) / StepsSize) * 2) * CayonDraining, z), Quaternion.identity);
					} else {
						GameObject obj = (GameObject)Instantiate(RockPrefabs, new Vector3(x, y + Mathf.PerlinNoise((x + map.x) / Pressision, (z + map.y) / Pressision) * Amplitude + ClosestSteps(Mathf.PerlinNoise((x + map.x) / StepsSize, (z + map.y) / StepsSize) * 2) * CayonDraining, z), Quaternion.identity);
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	float ClosestSteps (float NoiseValue) {
		return RoundToBound(NoiseValue * StepsAmplitude, StepsHeight/StepsCount);
	}

	float RoundToBound (float Value, float Bound) {
		return Mathf.Round(Value / Bound) * Bound;
	}
}
