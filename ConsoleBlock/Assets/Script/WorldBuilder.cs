using UnityEngine;
using System.Collections;

public class WorldBuilder : MonoBehaviour {

	public MapColor info;
	public Transform Generator;
	public GameObject BlockPrefab;
	public Vector2 LoadedChunk = -Vector2.one;
	public Material DefaultMaterial;
	public int ChunkRange = 1;

	public GameObject Cactus;
	public GameObject PalmTree;
	public GameObject OakTree;

	public float Ampl;
	public GameObject[,,] Objects;
	Vector2 oldLoadedChunk = Vector2.zero;

	int rx;
	int ry;

	// Use this for initialization
	void Start () {
		Objects = new GameObject[info.mapWidth,info.mapHeight,(ChunkRange*2+1)*(ChunkRange*2+1)+1];
		load();
	}
	
	// Update is called once per frame
	void Update () {
		rx = Mathf.RoundToInt((Generator.position.x - (info.mapWidth / 2)) / info.mapWidth);
		ry = Mathf.RoundToInt((Generator.position.z - (info.mapHeight / 2)) / info.mapHeight);
		if(oldLoadedChunk != new Vector2(rx, ry)) {
			//Debug.Log("Start Intentiating Process: " + Time.time + "/ " + LoadedChunk + " : " + new Vector2(rx, ry));
			StartCoroutine(SlowProcess());
		}
		oldLoadedChunk = new Vector2(rx, ry);
	}

	//if((xp == rx-ChunkRange || xp == rx+ChunkRange) && (yp == ry-ChunkRange || yp == ry+ChunkRange)) {

	void load () {
		int rx = Mathf.RoundToInt((Generator.position.x - (info.mapWidth / 2)) / info.mapWidth);
		int ry = Mathf.RoundToInt((Generator.position.z - (info.mapHeight / 2)) / info.mapHeight);
		for(int xp = rx-ChunkRange; xp < rx+ChunkRange+1; xp++) {
			for(int yp = ry-ChunkRange; yp < ry+ChunkRange+1; yp++) {
				//Debug.Log(a);

				info.LoadedChunk = new Vector2(Mathf.RoundToInt((Generator.position.x - (info.mapWidth / 2)) / info.mapWidth), Mathf.RoundToInt((Generator.position.z - (info.mapHeight / 2)) / info.mapHeight));
				info.FakePosX = Mathf.RoundToInt(xp - info.LoadedChunk.x);
				info.FakePosY = Mathf.RoundToInt(yp - info.LoadedChunk.y);
				float[,] HeightMap = new float[info.mapWidth,info.mapHeight];
				HeightMap = info.PreGeneratedHeightMap();
				Color[,] ColorMap = new Color[info.mapWidth,info.mapHeight];
				ColorMap = info.PreGeneratedColorMap();
				//End

				int chunkRSize = (ChunkRange * 2 + 1);

				int cl_x= ((xp - rx) + ChunkRange);
				int cl_y = ((yp - ry) + ChunkRange);
				//Debug.Log((xp - rx) + ", " + (yp - ry));

				int a = cl_x+cl_y*chunkRSize;
				//---
				for(int x = 0; x < info.mapWidth; x++) {
					for(int y = 0; y < info.mapHeight; y++) {
						//Debug.Log((LoadedChunk.x * info.mapWidth + x).ToString() + ", " + x);
						//Debug.Log("["+x+","+y+","+a+"]");
						//Debug.Log(a);
						if(Objects[x,y,a] != null) {
							Destroy(Objects[x,y,a]);
						}
						Objects[x,y,a] = (GameObject)Instantiate(BlockPrefab, new Vector3(xp * info.mapWidth + x, HeightMap[x,y] * Ampl, yp * info.mapHeight + y), Quaternion.identity);
						Objects[x,y,a].GetComponent<MeshRenderer>().material = DefaultMaterial;
						Objects[x,y,a].GetComponent<MeshRenderer>().material.color = ColorMap[x,y];
					}
				}

			}
		}
	}

	IEnumerator SlowProcess () {
		rx = Mathf.RoundToInt((Generator.position.x - (info.mapWidth / 2)) / info.mapWidth);
		ry = Mathf.RoundToInt((Generator.position.z - (info.mapHeight / 2)) / info.mapHeight);

		float[,,] DownloadHeightMap = new float[info.mapWidth,info.mapHeight, (ChunkRange*2+1)*(ChunkRange*2+1)];
		float[,] CHeightMap = new float[info.mapWidth,info.mapHeight];
		Color[,,] DownloadColorMap = new Color[info.mapWidth,info.mapHeight, (ChunkRange*2+1)*(ChunkRange*2+1)];
		Color[,] CColorMap = new Color[info.mapWidth,info.mapHeight];
		for(int w = rx-ChunkRange; w < rx+ChunkRange+1; w++) {
			for(int h = ry-ChunkRange; h < ry+ChunkRange+1; h++) {
				info.LoadedChunk = new Vector2(rx,ry);
				info.FakePosX = Mathf.RoundToInt(w - info.LoadedChunk.x);
				info.FakePosY = Mathf.RoundToInt(h - info.LoadedChunk.y);

				int chunkRSize = (ChunkRange * 2 + 1);

				int cl_x= ((w - rx) + ChunkRange);
				int cl_y = ((h - ry) + ChunkRange);

				int a = cl_x+cl_y*chunkRSize;

				CHeightMap = info.PreGeneratedHeightMap();
				Debug.Log(a);
				for(int x = 0; x < info.mapWidth; x++) {
					for(int y = 0; y < info.mapHeight; y++) {
						DownloadHeightMap[x,y,a] = CHeightMap[x,y];
					}
				}
				CColorMap = info.PreGeneratedColorMap();
				for(int x = 0; x < info.mapWidth; x++) {
					for(int y = 0; y < info.mapHeight; y++) {
						DownloadColorMap[x,y,a] = CColorMap[x,y];
					}
				}
				yield return new WaitForSeconds(0.1f);
			}
		}

		//Vector2 movement = new Vector2(rx, ry) - oldLoadedChunk;
		//GameObject[,,] Copy = Objects;

		/*
		for(int xp = rx-ChunkRange; xp < rx+ChunkRange+1; xp++) {
			for(int yp = ry-ChunkRange; yp < ry+ChunkRange+1; yp++) {
				Debug.Log(((xp == rx-ChunkRange || xp == rx+ChunkRange) || (yp == ry-ChunkRange || yp == ry+ChunkRange)) + ", " + new Vector2(xp, yp));
				if(Vector2.Distance(oldLoadedChunk, new Vector2(rx, ry)) < 2) {
					if(((xp == rx-ChunkRange || xp == rx+ChunkRange) || (yp == ry-ChunkRange || yp == ry+ChunkRange))) {
						int chunkRSize = (ChunkRange * 2 + 1);

						int cl_x1 = ((xp - (rx + Mathf.RoundToInt(movement.x))) + ChunkRange);
						int cl_y1 = ((yp - (ry + Mathf.RoundToInt(movement.y))) + ChunkRange);

						int a1 = cl_x1+cl_y1*chunkRSize;

						int cl_x2 = ((xp - rx) + ChunkRange);
						int cl_y2 = ((yp - ry) + ChunkRange);

						int a2 = cl_x2+cl_y2*chunkRSize;

						Debug.Log(movement);
						Debug.Log(a2 + ", Modif : " + a1);
						for(int x = 0; x < info.mapWidth; x++) {
							for(int y = 0; y < info.mapHeight; y++) {
								Objects[x,y,a2] = Copy[x,y,a1];
							}
						}
					}
				}
			}
		}
		*/

		for(int xp = rx-ChunkRange; xp < rx+ChunkRange+1; xp++) {
			for(int yp = ry-ChunkRange; yp < ry+ChunkRange+1; yp++) {
				//Debug.Log(a);
				//((xp == rx-ChunkRange || xp == rx+ChunkRange) || (yp == ry-ChunkRange || yp == ry+ChunkRange))
				// && !(Vector2.Distance(oldLoadedChunk, new Vector2(rx, ry)) < 2)
				if(/*((xp == rx-ChunkRange || xp == rx+ChunkRange) || (yp == ry-ChunkRange || yp == ry+ChunkRange))*/true) {
					//Debug.Log("Replacing Chunk at " + new Vector2(xp, yp) + " : " + Vector2.Distance(new Vector2(xp,yp), oldLoadedChunk) + " : " + oldLoadedChunk);
					info.LoadedChunk = new Vector2(rx,ry);
					info.FakePosX = Mathf.RoundToInt(xp - info.LoadedChunk.x);
					info.FakePosY = Mathf.RoundToInt(yp - info.LoadedChunk.y);

					int chunkRSize = (ChunkRange * 2 + 1);

					int cl_x= ((xp - rx) + ChunkRange);
					int cl_y = ((yp - ry) + ChunkRange);

					int a = cl_x+cl_y*chunkRSize;

					for(int x = 0; x < info.mapWidth; x++) {
						for(int y = 0; y < info.mapHeight; y++) {
							//Debug.Log((LoadedChunk.x * info.mapWidth + x).ToString() + ", " + x);
							//Debug.Log("["+x+","+y+","+a+"]");
							//Debug.Log(a);
							if(Objects[x,y,a] != null) {
								Destroy(Objects[x,y,a]);
							}
							Objects[x,y,a] = (GameObject)Instantiate(BlockPrefab, new Vector3(xp * info.mapWidth + x, DownloadHeightMap[x,y,a] * Ampl, yp * info.mapHeight + y), Quaternion.identity);
							Objects[x,y,a].GetComponent<MeshRenderer>().material = DefaultMaterial;
							Objects[x,y,a].GetComponent<MeshRenderer>().material.color = DownloadColorMap[x,y,a];
						}
					}
				}
			}
		}
		LoadedChunk = new Vector2(Mathf.RoundToInt((Generator.position.x - (info.mapWidth / 2)) / info.mapWidth), Mathf.RoundToInt((Generator.position.z - (info.mapHeight / 2)) / info.mapHeight));
		yield return new WaitForSeconds(0f);
	}

	float RoundToBound (float Value, float Bound) {
		return Bound * Mathf.Round(Value / Bound);
	}
}
