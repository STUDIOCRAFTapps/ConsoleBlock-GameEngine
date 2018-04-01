using UnityEngine;
using System.Collections;

public class WorldGeneration : MonoBehaviour {

	public int NoiseScale = 25;
	public int Octaves = 3;
	public float Persistance = 0.5f;
	public int Lacuarity = 2;
	public Vector2 Offset = new Vector2(51.11f, 0);

	public GameObject Grass;
	public GameObject Dirt;
	public GameObject Stone;
	public GameObject Sand;

	public float NoiseConcentation = 0.05f;
	public float Modifier = 2.5f;

	int width = 128;
	int height = 128;

	float[,] Map;
	float[,] OceanicMap;

	// Use this for initialization
	void Start () {

	}
	
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		float[,] noiseMap = new float[mapWidth,mapHeight];
		
		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}
		
		if (scale <= 0) {
			scale = 0.0001f;
		}
		
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;
		
		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;
		
		
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;
				
				for (int i = 0; i < octaves; i++) {
					float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;
					
					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;
					
					amplitude *= persistance;
					frequency *= lacunarity;
				}
				
				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				noiseMap [x, y] = noiseHeight;
			}
		}
		
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
			}
		}
		
		return noiseMap;
	}
	
}
