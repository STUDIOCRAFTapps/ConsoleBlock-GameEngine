using UnityEngine;
using System.Collections;

public class Player1AI : MonoBehaviour {

	Rigidbody PlayerRigidbody;
	public GameObject Projectile;
	public bool CreateNewGeneration;
	bool GetHit;
	int[] IDs;

	float RangeSize = 4f;
	float TurnDirectionLeft = 50f;
	float MoveDirectionLeft = 50f;
	float TurnSpeed;
	float MoveSpeed;
	float HeightSize;
	float WaitTime;

	// Use this for initialization
	void Start () {
		PlayerRigidbody = GetComponent<Rigidbody>();
		StartCoroutine("Play");
	}

	IEnumerator Play () {
		IDs = new int[3];
		IDs[0] = 0;
		IDs[1] = 0;
		IDs[2] = 0;
		TurnDirectionLeft = Random.Range(0.0f, RangeSize * 25.0f) + TurnDirectionLeft;
		MoveDirectionLeft = Random.Range(0.0f, RangeSize * 25.0f) + TurnDirectionLeft;
		TurnSpeed = Random.Range(1.0f, RangeSize * 1.25f) + TurnSpeed;
		MoveSpeed = Random.Range(1.0f, RangeSize * 1.25f) + MoveSpeed;
		HeightSize = Random.Range(1.0f, RangeSize * 1.25f) + HeightSize;
		WaitTime = (Random.Range(1.0f, RangeSize * 1.25f) + WaitTime) * 0.1f;
		while (true) {
			if(Random.Range(1, RangeSize * 2.5f) < 4) {
				foreach(int ID in IDs) {
					string TurnDirection;
					if(Random.Range(0, TurnDirectionLeft) > 50) {
						TurnDirection = "Left";
					} else {
						TurnDirection = "Right";
					}
					string MoveDirection;
					if(Random.Range(0, MoveDirectionLeft) > 50) {
						MoveDirection = "Left";
					} else {
						MoveDirection = "Right";
					}
					if(ID == 0) {
						int RandomN = Random.Range(1, 5 + 1);
						if(RandomN == 1) {
							StartCoroutine(SpiralJump(TurnSpeed * 5, HeightSize * 5, WaitTime, TurnDirection));
						}
						if(RandomN == 2) {
							StartCoroutine(Turn(TurnDirection, TurnSpeed * 5));
						}
						if(RandomN == 3) {
							StartCoroutine(Jump(HeightSize * 2));
						}
						if(RandomN == 4) {
							StartCoroutine(Shoot());
						}
						if(RandomN == 5) {
							StartCoroutine(Move(MoveDirection, MoveSpeed * 2));
						}
					} else {
						if(ID == 1) {
							StartCoroutine(SpiralJump(TurnSpeed * 5, HeightSize * 5, WaitTime, TurnDirection));
						} else if(ID == 2) {
							StartCoroutine(Turn(TurnDirection, TurnSpeed * 5));
						} else if(ID == 3) {
							StartCoroutine(Jump(HeightSize * 5));
						} else if(ID == 4) {
							StartCoroutine(Shoot());
						} else if(ID == 5) {
							StartCoroutine(Move(MoveDirection, MoveSpeed * 2));
						}
					}
					yield return new WaitForSeconds(Random.Range(0.1f, 2f));
				}
			}
			yield return new WaitForSeconds(WaitTime);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) {
			StartCoroutine(SpiralJump(5, 10, 0.1f, "Left"));
			StartCoroutine(Shoot());
		}
		if(CreateNewGeneration) {
			if(RangeSize > 0.1f) {
				RangeSize -= 0.1f;
			}
			TurnSpeed = Random.Range(1.0f, RangeSize * 1.25f) + TurnSpeed;
			MoveSpeed = Random.Range(1.0f, RangeSize * 1.25f) + MoveSpeed;
			HeightSize = Random.Range(1.0f, RangeSize * 1.25f) + HeightSize;
			WaitTime = Random.Range(1.0f, RangeSize * 1.25f) + WaitTime;
			CreateNewGeneration = false;
		}
	}

	// Moving Functions
	IEnumerator SpiralJump (float Speed, float Height, float WaitTime, string Direction) {
		PlayerRigidbody.velocity = new Vector3(0, Height, 0);
		yield return new WaitForSeconds(WaitTime);
		if(Direction == "Left") {
			PlayerRigidbody.angularVelocity = new Vector3(0, 0, Speed * -1);
		} else {
			PlayerRigidbody.angularVelocity = new Vector3(0, 0, Speed);
		}
		Debug.Log("SpiralJump");
	}

	IEnumerator Turn (string Direction, float Speed) {
		if(Direction == "Left") {
			PlayerRigidbody.angularVelocity = new Vector3(0, 0, Speed * -1);
		} else {
			PlayerRigidbody.angularVelocity = new Vector3(0, 0, Speed);
		}
		yield return new WaitForSeconds(0.1f);
		Debug.Log("Turn");
	}

	IEnumerator Jump (float Height) {
		PlayerRigidbody.velocity = new Vector3(0, PlayerRigidbody.velocity.x - Height, 0);
		yield return new WaitForSeconds(0.1f);
		Debug.Log("Jump");
	}

	IEnumerator Shoot () {
		yield return new WaitForSeconds(0.1f);
		GameObject TargetProjectil = (GameObject)Instantiate(Projectile, new Vector3(gameObject.transform.position.x + 1,gameObject.transform.position.y, 0), gameObject.transform.rotation);
		TargetProjectil.GetComponent<Projectile1Script>().Operator = gameObject.GetComponent<Player1AI>();
		TargetProjectil.GetComponent<Rigidbody>().angularVelocity = new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z);
		Debug.Log("Shoot");
	}

	IEnumerator Move (string Direction, float Speed) {
		yield return new WaitForSeconds(0.1f);
		if(Direction == "Left") {
			PlayerRigidbody.velocity = new Vector3(-Speed, 0.1f, 0);
		} else {
			PlayerRigidbody.velocity = new Vector3(Speed, 0.1f, 0);
		}
		Debug.Log("Move");
	}

	void OnCollisionEnter (Collision col) {
		if(col.gameObject.name == "Projectile2") {
			GetHit = true;
		}
	}
}
