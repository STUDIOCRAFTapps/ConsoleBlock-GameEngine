using UnityEngine;
using System.Collections;

public class MapColor : MonoBehaviour {

	public Settings settings;
	public bool IsDisplayColor = true;
	public bool IsGraphicsHeight = true;

	[Range(0.00f, 1.00f)]
	public float DryLevel;
	[Range(0.00f, 1.00f)]
	public float MoutainLevel;
	[Range(0.00f, 1.00f)]
	public float DryMoutainLevel; // was 0.616
	[Range(0.00f, 1.00f)]
	public float SnowyMoutainLevel;
	[Range(0.00f, 1.00f)]
	public float RockDryness;

	public Color Dry;
	public Color Wet;
	public Color Cold;

	public Color Ocean;
	public Color Sea;
	public Color TopicalBeach;
	public Color TeamperateBeach;
	public Color TropicalLake;
	public Color TeamperateLake;
	public Color ColdLake;

	public Color DrySand;
	public Color WetSand;

	public Color WetRock;
	public Color ColdRock;
	public Color Snow;
	public Color Ice;

	public Renderer textureRender;

	public int mapWidth;
	public int mapHeight;
	public int seed;
	public float scale;
	public int octaves;
	public float persistance;
	public float lacunarity;
	public Vector2 offset;
	[Range(0.00f, 1.00f)]
	public float WaterLevel;
	[Range(0.00f, 1.00f)]
	public float BeachLevel;
	[Range(0.00f, 1.00f)]
	public float SeaLevel;

	public int ChunksByWorld = 0;
	public Vector2 LoadedChunk = Vector2.zero;

	[Range(0.00f, 1.00f)]
	public float GlobalIceMapping;
	[Range(0.00f, 1.00f)]
	public float GlobalSnowMapping;

	public int FakePosX = 0;
	public int FakePosY = 0;
	
	public void DrawNoiseMap(float[,] noiseMap) {
		int width = noiseMap.GetLength(0);
		int height = noiseMap.GetLength(1);
		
		Texture2D texture = new Texture2D(width, height);
		
		Color[] colourMap = new Color[width * height];
		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {
				colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap [x, y]);
			}
		}
		texture.SetPixels(colourMap);
		texture.Apply();
		
		textureRender.sharedMaterial.mainTexture = texture;
	}

	public Color[,] PreGeneratedColorMap () {
		return NoiseGenerator.ColorMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset, WaterLevel, gameObject.GetComponent<MapColor>());
	}

	public float[,] PreGeneratedHeightMap () {
		return NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset + new Vector2((LoadedChunk.x + FakePosX) * mapWidth, (LoadedChunk.y + FakePosY) * mapHeight));
	}

	public void DrawColoredMap(Color[,] colorMap) {
		int width = colorMap.GetLength(0);
		int height = colorMap.GetLength(1);
		
		Texture2D texture = new Texture2D(width, height);

		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {
				texture.SetPixel(x, y, colorMap[x,y]);
			}
		}
		texture.Apply();
		
		textureRender.sharedMaterial.mainTexture = texture;
	}

	// Use this for initialization
	void Start () {
		settings = new Settings();
		settings.DisplayColor = IsDisplayColor;
		settings.GraphicsHeight = IsGraphicsHeight;
	}
	
	// Update is called once per frame
	void Update () {
		settings.DisplayColor = IsDisplayColor;
		settings.GraphicsHeight = IsGraphicsHeight;
		//DrawColoredMap(NoiseGenerator.ColorMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset, WaterLevel, gameObject.GetComponent<MapColor>()));
		if(Input.GetKeyDown(KeyCode.Return)) {
			if(settings.DisplayColor) {
				DrawColoredMap(NoiseGenerator.ColorMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset, WaterLevel, gameObject.GetComponent<MapColor>()));
			} else {
				if(settings.GraphicsHeight) {
					DrawNoiseMap(NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset));
				} else {
					DrawNoiseMap(NoiseGenerator.TempMap(mapWidth, mapHeight, NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset), gameObject.GetComponent<MapColor>()));
				}
			}
		}
	}
}

public class Settings {
	public bool DisplayColor = true;
	public bool GraphicsHeight = true;
}

//Thanks to Sebastian Lague for this NoiseGenerator function ;)
public class NoiseGenerator {
	public static Color[,] ColorMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float WaterLevel, MapColor colors) {
		float[,] heightMap = new float[mapWidth,mapHeight];
		float[,] temparatureMap = new float[mapWidth,mapHeight];
		Color[,] writingColorMap = new Color[mapWidth,mapHeight];
		heightMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset + new Vector2((colors.LoadedChunk.x + colors.FakePosX) * mapWidth, (colors.LoadedChunk.y + colors.FakePosY) * mapHeight));
		temparatureMap = NoiseGenerator.TempMap(mapWidth, mapHeight, heightMap, colors);
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				Color BeachColor = new Color(Mathf.Lerp(colors.TeamperateBeach.r, colors.TopicalBeach.r, 1 - temparatureMap[x,y]), Mathf.Lerp(colors.TeamperateBeach.g, colors.TopicalBeach.g, 1 - temparatureMap[x,y]), Mathf.Lerp(colors.TeamperateBeach.b, colors.TopicalBeach.b, 1 - temparatureMap[x,y]));
				BeachColor.r = BeachColor.r + heightMap[x,y] - WaterLevel;
				BeachColor.g = BeachColor.g + heightMap[x,y] - WaterLevel;
				BeachColor.b = BeachColor.b + heightMap[x,y] - WaterLevel;
				Color OceanColor = new Color((colors.Ocean.r + heightMap[x,y]) - WaterLevel, (colors.Ocean.g + heightMap[x,y]) - WaterLevel, (colors.Ocean.b + heightMap[x,y]) - WaterLevel);
				Color SeaColor = new Color((colors.Sea.r + heightMap[x,y]) - WaterLevel, (colors.Sea.g + heightMap[x,y]) - WaterLevel, (colors.Sea.b + heightMap[x,y]) - WaterLevel);
				Color UnEditedGrass = new Color(Mathf.Lerp(colors.Cold.r, colors.Wet.r, 1 - temparatureMap[x,y]), Mathf.Lerp(colors.Cold.g, colors.Wet.g, 1 - temparatureMap[x,y]), Mathf.Lerp(colors.Cold.b, colors.Wet.b, 1 - temparatureMap[x,y]));
				if(heightMap[x,y] >= WaterLevel) {
					if(heightMap[x,y] > colors.DryLevel) {
						if(heightMap[x,y] > colors.MoutainLevel) {
							writingColorMap[x,y] = new Color(Mathf.Lerp(colors.WetRock.r, colors.ColdRock.r, heightMap[x,y]) - colors.RockDryness, Mathf.Lerp(colors.WetRock.g, colors.ColdRock.g, heightMap[x,y]) - colors.RockDryness, Mathf.Lerp(colors.WetRock.b, colors.ColdRock.b, heightMap[x,y]) - colors.RockDryness);
						} else {
							if(temparatureMap[x,y] < 0.40f) {
								writingColorMap[x,y] = colors.DrySand;
							} else {
								writingColorMap[x,y] = new Color(Mathf.Lerp(UnEditedGrass.r, colors.Dry.r, heightMap[x,y]), Mathf.Lerp(UnEditedGrass.g, colors.Dry.g, heightMap[x,y]), Mathf.Lerp(UnEditedGrass.b, colors.Dry.b, heightMap[x,y]));
							}
						}
					} else {
						writingColorMap[x,y] = UnEditedGrass;
					}
				} else {
					if(temparatureMap[x,y] > colors.GlobalIceMapping) {
						writingColorMap[x,y] = colors.Ice;
					} else {
						if(heightMap[x,y] > colors.BeachLevel) {
							writingColorMap[x,y] = NoiseGenerator.ColorLerp(BeachColor, SeaColor, 1 - (heightMap[x,y] - colors.BeachLevel) / (WaterLevel - colors.BeachLevel));
						} else {
							if(heightMap[x,y] > colors.SeaLevel) {
								writingColorMap[x,y] = NoiseGenerator.ColorLerp(SeaColor, OceanColor, 1 - (heightMap[x,y] - colors.SeaLevel) / (colors.BeachLevel - colors.SeaLevel));
							} else {
								writingColorMap[x,y] = OceanColor;
							}
						}
					}
				}
			}
		}
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				if(temparatureMap[x,y] > colors.GlobalSnowMapping && heightMap[x,y] > WaterLevel) {
					writingColorMap[x,y] = colors.Snow;
				}
			}
		}
		return writingColorMap;
	}
	public static Color ColorLerp (Color A, Color B, float T) {
		return new Color(Mathf.Lerp(A.r, B.r, T), Mathf.Lerp(A.g, B.g, T), Mathf.Lerp(A.b, B.b, T));
	}
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		float[,] noiseMap = new float[mapWidth,mapHeight];
		
		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for(int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}
		
		if(scale <= 0) {
			scale = 0.0001f;
		}
		
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;
		
		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;
		
		
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;
				
				for(int i = 0; i < octaves; i++) {
					float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency;
					
					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;
					
					amplitude *= persistance;
					frequency *= lacunarity;
				}
				
				if(noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if(noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				noiseMap[x, y] = noiseHeight;
			}
		}
		
		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				//Debug.Log(minNoiseHeight + ", " + maxNoiseHeight);
				//noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
				noiseMap[x, y] = Mathf.InverseLerp(-11.54316f, 10.00745f, noiseMap[x, y]); //UseEstimations
				//float saveValue = (noiseMap[x,y] + 1) / (maxPossibleHeight / 0.9f);
				//noiseMap[x, y] = Mathf.Clamp(saveValue, 0, int.MaxValue);
			}
		}
		
		return noiseMap;
	}
	public static float[,] TempMap(int mapWidth, int mapHeight, float[,] noiseMap, MapColor info) {
		float[,] save = new float[mapWidth,mapHeight];
		float[,] coldHeight = noiseMap;
		for(int x = 0; x < mapWidth; x++) {
			for(int y = 0; y < mapHeight; y++) {
				save[x,y] = ReturnValue(((y + mapHeight * info.LoadedChunk.y) - (mapHeight * info.ChunksByWorld / 2))) / (mapHeight * info.ChunksByWorld / 2);
			}
		}
		for(int x = 0; x < mapWidth; x++) {
			for(int y = 0; y < mapHeight; y++) {
				save[x,y] = save[x,y] + coldHeight[x,y];
			}
		}
		for(int x = 0; x < mapWidth; x++) {
			for(int y = 0; y < mapHeight; y++) {
				save[x,y] = save[x,y] / 2;
			}
		}
		return save;
	}
	public static float ReturnValue (float Value) {
		if(Value < 0) {
			return Value * -1;
		} else {
			return Value;
		}
	}

}
