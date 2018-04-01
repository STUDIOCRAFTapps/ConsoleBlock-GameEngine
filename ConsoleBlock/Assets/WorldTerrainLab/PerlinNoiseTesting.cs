using UnityEngine;
using System.Collections;

public class PerlinNoiseTesting : MonoBehaviour {

	public AnimationCurve DensityOverHeight;
	public float DensityModificator = 5f;
	float MainOctaveSize = 0.05f;
	public float requireForce = 0.4f;

	public float NoiseStartX;
	public float NoiseStartY;
	public float NoiseStartZ;

	public GameObject obj;
	public int MapX = 32;
	public int MapY = 32;
	public int MapZ = 32;

	// Use this for initialization
	void Start () {
		NoiseStartX = Random.Range(0, 1048.0f);
		NoiseStartY = Random.Range(0, 1048.0f);
		NoiseStartZ = Random.Range(0, 1048.0f);
		for(int x = 0; x < MapX; x++) {
			for(int y = 0; y < MapY; y++) {
				for(int z = 0; z < MapZ; z++) {
					if(PerlinNoise3d(NoiseStartX+(x*MainOctaveSize), NoiseStartY+(y*MainOctaveSize)*(DensityOverHeight.Evaluate(y/MapY)*DensityModificator), NoiseStartZ+(z*MainOctaveSize)) >= 0.4f) {
						Instantiate(obj, new Vector3(x,y,z), Quaternion.identity);
					}
				}
			}
		}
	}

	float PerlinNoise3d (float x, float y, float z) {
		return Mathf.PerlinNoise(x,z)*Mathf.PerlinNoise(x,y)*Mathf.PerlinNoise(z,y);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
