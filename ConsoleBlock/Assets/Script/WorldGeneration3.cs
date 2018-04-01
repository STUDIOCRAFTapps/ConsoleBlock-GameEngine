using UnityEngine;
using System.Collections;

public class WorldGeneration3 : MonoBehaviour {

	public GameObject PlanetsRock;
	public int ChunkSize = 32;
	public AnimationCurve ChoppingCurve;
	public float Modifier;
	public float Pressision;
	public float WavesPressision;
	public int Height = 16;

	public Vector2 map;

	// Use this for initialization
	void Start () {
		map = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
		for(int x = 0; x < ChunkSize; x++) {
			for(int y = 0; y < Height; y++) {
				for(int z = 0; z < ChunkSize; z++) {
					if(!(Mathf.PerlinNoise((x + map.x) / Pressision, (z + map.y) / Pressision) > ChoppingCurve.Evaluate(ChoppingCurve.keys[ChoppingCurve.length - 1].time / Height * y))) {
						Instantiate(PlanetsRock, new Vector3(x, y + Mathf.PerlinNoise((x + map.x) / WavesPressision, (z + map.y) / WavesPressision), z), Quaternion.identity);
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
