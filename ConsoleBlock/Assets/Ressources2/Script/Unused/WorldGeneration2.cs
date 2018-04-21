using UnityEngine;
using System.Collections;

public class WorldGeneration2 : MonoBehaviour {

	public GameObject PlanetsRock;
	public int ChunkSize = 32;
	public float InvertingPoint;
	public float Pressision;
	public float AmplifingPressision;

	public Vector2 map;

	// Use this for initialization
	void Start () {
		map = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
		for(int x = 0; x < ChunkSize; x++) {
			for(int y = 0; y < 32; y++) {
				for(int z = 0; z < ChunkSize; z++) {
					if(Mathf.PerlinNoise(((x + map.x) / Pressision), ((z + map.y) / Pressision)) * y > y - (InvertingPoint * (Mathf.PerlinNoise(x * map.x / (Pressision * AmplifingPressision), z * map.y / (Pressision * AmplifingPressision)) * 3))) {
						Instantiate(PlanetsRock, new Vector3(x, y, z), Quaternion.identity);
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
