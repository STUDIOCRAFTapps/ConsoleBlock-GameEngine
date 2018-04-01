using UnityEngine;
using System.Collections;

public class TreeCreator : MonoBehaviour {

	public GameObject Log;
	public GameObject Leaves;

	public int MinTreeHeight = 5;
	public int MaxTreeHeight = 18;

	public int MinBranch = 3;
	public int MaxBranch = 7;

	public int MinBranchHeight = 5;
	public int MaxBranchHeight = 18;

	public float TreeBase = 1.5f;
	public AnimationCurve TreeBaseReduction;
	public AnimationCurve TreeDirection;
	public float CurveMultiplier = 8f;
	public bool DivideBeforeTrow;

	//More Setting
	public bool RandomizeDirection = true;
	public float DirectionDegree;
	public int BranchIterations = 3;
	public float BranchIterationSizeReduction = 0.65f;
	public float MaxDivisions = 3;

	int TreeHeight;
	int TreeBranch;
	float DirectionD;
	GameObject LogP;
	GameObject LogP2;
	float CH;
	int Branch;
	int AlreadyDivide;
	float GivenRot;

	public string Seed;

	// Use this for initialization
	void Start () {
		/*Testing*/
			//Debug.Log(MathClass.Convert.RadiusAngleToVector3_XZ(Vector3.zero, 2, 36)); [Ok]

		if(string.IsNullOrEmpty(Seed)) {
			Random.InitState(Mathf.RoundToInt(System.Environment.TickCount));
			Seed = Mathf.RoundToInt(System.Environment.TickCount).ToString();
		} else {
			Random.InitState(Seed.GetHashCode());
		}
		//RandomizeValue
		TreeHeight = Random.Range(MinTreeHeight, MaxTreeHeight + 1);
		TreeBranch = Random.Range(MinBranch, MaxBranch + 1);
		if(RandomizeDirection) {
			DirectionD = Random.Range(0f, 360f);
		} else {
			DirectionD = DirectionDegree;
		}

		//Repate
		CH = TreeBase / 2;
		AlreadyDivide = 0;
		for(int i = 0; i < TreeHeight; i++) {
			float CurrentSize = (2 * TreeBaseReduction.Evaluate((TreeBaseReduction.keys[TreeBaseReduction.length - 1].time / TreeHeight) * i));
			Vector3 CurrentPos = MathClass.Convert.RadiusAngleToVector3_XZ(Vector3.zero, TreeDirection.Evaluate((TreeDirection.keys[TreeDirection.length - 1].time / TreeHeight) * i) * CurveMultiplier, DirectionD);
			LogP = (GameObject)Instantiate(Log, transform.position + new Vector3(CurrentPos.x, CH, CurrentPos.z), Quaternion.identity);
			LogP.transform.parent = transform;
			LogP.transform.localScale = Vector3.one * CurrentSize;
			if(TreeBranch > 0 && i > MinBranchHeight && i < MaxBranchHeight) {
				if(DivideBeforeTrow && AlreadyDivide < MaxDivisions) {
					//Include BranchDiviser
					AlreadyDivide++;	//Block the log to divise again
					float DirectionModifier = Random.Range(-0.50f * CurveMultiplier, 0.50f * CurveMultiplier);
												//Edit the direction of the old log
					TreeDirection.MoveKey(TreeDirection.length - 1, new Keyframe(TreeDirection.keys[TreeDirection.length - 1].time, TreeDirection.keys[TreeDirection.length - 1].value - DirectionModifier));
					float CH2 = CH;
					GivenRot = Random.Range(0f, 360f);
					for(int b = i; b < TreeHeight; b++) {
						float CurrentSize2 = (2 * TreeBaseReduction.Evaluate((TreeBaseReduction.keys[TreeBaseReduction.length - 1].time / TreeHeight) * b));
						Vector3 CurrentPos2;
						if(AlreadyDivide == 1) {
							CurrentPos2 = MathClass.Convert.RadiusAngleToVector3_XZ(Vector3.zero, TreeDirection.Evaluate((TreeDirection.keys[TreeDirection.length - 1].time / TreeHeight) * b) * (CurveMultiplier * -1), DirectionD);
						} else {
							CurrentPos2 = MathClass.Convert.RadiusAngleToVector3_XZ(Vector3.zero, TreeDirection.Evaluate((TreeDirection.keys[TreeDirection.length - 1].time / TreeHeight) * b) * (CurveMultiplier * -1), GivenRot);
						}
						LogP2 = (GameObject)Instantiate(Log, transform.position + new Vector3(CurrentPos2.x, CH2, CurrentPos2.z), Quaternion.identity);
						LogP2.transform.localScale = Vector3.one * CurrentSize2;
						LogP2.transform.parent = transform;
						CH2 += CurrentSize2;
					}
				} else {

				}
			}
			CH += CurrentSize;
		}
		Vector3 BasePos = new Vector3(transform.position.x, transform.position.y + (TreeHeight - (TreeHeight / 4 * 1)), transform.position.z);
		Vector3 MainLeaveScale = new Vector3(TreeHeight / 5 * 2, TreeHeight / 5 * 2, TreeHeight / 5 * 2);
		GameObject MainLeaveObject = (GameObject)Instantiate(Leaves, BasePos, Quaternion.identity);
		MainLeaveObject.transform.localScale = MainLeaveScale;
		MainLeaveObject.transform.parent = transform;
		for(int i = 0; i < Random.Range(MinBranch, MaxBranch); i++) {
			float ScaleNm = Random.Range(MinBranch, TreeHeight / 5 * 1);
			GameObject CO = (GameObject)Instantiate(Leaves, BasePos + new Vector3(Random.Range(MainLeaveScale.x / 3 * 2, -MainLeaveScale.x / 3 * 2), Random.Range(MainLeaveScale.y / 3 * 2, -MainLeaveScale.y / 3 * 2), Random.Range(MainLeaveScale.z / 3 * 2, -MainLeaveScale.z / 3 * 2)), Quaternion.identity);
			CO.transform.localScale = new Vector3(ScaleNm, ScaleNm, ScaleNm);
			CO.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}

public sealed class MathClass {
	public sealed class Convert {
		public static Vector3 RadiusAngleToVector3_XZ (Vector3 Position, float Radius, float Angle) {
			return Position + new Vector3(Mathf.Sin(Mathf.Deg2Rad * Angle) * Radius, 0, Mathf.Cos(Mathf.Deg2Rad * Angle) * Radius);
		}
	}
	public sealed class RoundValue {
		public static float RoundAtBound (float Value, float Bound) {
			return Bound * Mathf.Round(Value / Bound);
		}
	}
}
