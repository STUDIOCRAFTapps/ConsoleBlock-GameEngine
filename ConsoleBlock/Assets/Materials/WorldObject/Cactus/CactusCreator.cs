using UnityEngine;
using System.Collections;

public class CactusCreator : MonoBehaviour {

	public GameObject Flower;
	public GameObject Block;

	public int Height;
	float CurrentSize = 0f;
	float Stack = 0f;

	public string Seed;

	// Use this for initialization
	void Start () {
		if(string.IsNullOrEmpty(Seed)) {
			Random.InitState(Mathf.RoundToInt(System.Environment.TickCount));
			Seed = Mathf.RoundToInt(System.Environment.TickCount).ToString();
		} else {
			Random.InitState(Seed.GetHashCode());
		}
		Height = Random.Range(4,7);
		for(int i = 0; i < Height; i++) {
			if((i%2)==1) {
				CurrentSize = Random.Range(0.8f, 0.85f);
				Stack+=CurrentSize/2;
				GameObject CObj = (GameObject)Instantiate(Block, transform.position + new Vector3(0,Stack,0), Quaternion.identity);
				CObj.transform.localScale = new Vector3(CurrentSize,CurrentSize,CurrentSize);
				CObj.transform.parent = transform;
				Stack+=CurrentSize/2;
			} else {
				CurrentSize = Random.Range(0.9f, 1.05f);
				Stack+=CurrentSize/2;
				GameObject CObj = (GameObject)Instantiate(Block, transform.position + new Vector3(0,Stack,0), Quaternion.identity);
				CObj.transform.localScale = new Vector3(CurrentSize,CurrentSize,CurrentSize);
				CObj.transform.parent = transform;
				Stack+=CurrentSize/2;

				//Flower
				if(Random.Range(0,3) > 0) { //1 of 3
					GameObject FObj = (GameObject)Instantiate(Flower, transform.position + new Vector3(0,Stack,0), Quaternion.Euler(0,Random.Range(0,5)*90,0));
					FObj.transform.GetChild(0).localPosition = new Vector3(Random.Range(-0.4f,0.4f),-(CurrentSize/2)+Random.Range(-0.4f,0.4f),CurrentSize/2+0.07f);
					FObj.transform.GetChild(0).localRotation = Quaternion.Euler(Random.Range(-7.0f,7.0f),180+Random.Range(-7.0f,7.0f),Random.Range(-45,46));
					float Size = Random.Range(0.6f,1.0f);
					FObj.transform.localScale = new Vector3(Size,Size,1);
					FObj.transform.parent = transform;
				}
			}
		}
	}
}
