using UnityEngine;
using System.Collections;

public class PalmTreeCreator : MonoBehaviour {

	public float Height;
	public AnimationCurve growingCurve;
	public float Direction;

	public GameObject Log;
	public GameObject Leaf;

	public string Seed;

	// Use this for initialization
	void Start () {
		if(string.IsNullOrEmpty(Seed)) {
			Random.InitState(Mathf.RoundToInt(System.Environment.TickCount));
			Seed = Mathf.RoundToInt(System.Environment.TickCount).ToString();
		} else {
			Random.InitState(Seed.GetHashCode());
		}
		Direction = Random.Range(0.0f,360.0f);
		Height = Random.Range(7,15);
		growingCurve.AddKey(0,0);
		growingCurve.AddKey(1,Random.Range(0.1f,0.5f));
		growingCurve.keys[1].inTangent = Random.Range(0.0f,0.51f);
		growingCurve.keys[1].outTangent = Random.Range(0.0f,0.3f);
		for(float i = 0; i < Height; i+=1f) {
			GameObject logObj = (GameObject)Instantiate(Log, transform.position + new Vector3(TreeMath.NextLog(growingCurve.Evaluate(i/Height)*5,Direction).x, i, TreeMath.NextLog(growingCurve.Evaluate(i/Height)*5,Direction).y), Quaternion.identity);
			logObj.transform.parent = transform;
			if(i >= Height-1) {
				for(int l = 0; l < 6; l++) {
					GameObject leafObj = (GameObject)Instantiate(Leaf, transform.position + new Vector3(TreeMath.NextLog(growingCurve.Evaluate(i/Height)*5,Direction).x, i, TreeMath.NextLog(growingCurve.Evaluate(i/Height)*5,Direction).y)+Vector3.up*0.5f,Quaternion.Euler(Random.Range(-45,45),l*60,Random.Range(-95,-122)));
					leafObj.transform.parent = transform;
				}
			}
		}
	}
}

public class TreeMath {
	static public Vector2 NextLog(float Raduis, float Angle) {
		return new Vector2(Mathf.Cos(Angle*Mathf.Deg2Rad)*Raduis, Mathf.Sin(Angle*Mathf.Deg2Rad)*Raduis);
	}
}
