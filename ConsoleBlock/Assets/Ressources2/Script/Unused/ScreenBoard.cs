using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenBoard : MonoBehaviour {

	public Transform ScreenObject;

	public InputConnector input;
	public OutputConnecter output;

	public Material[] mats;
	
	public string[] SetArray;
	public string[] SetValues;

	public GameObject GetSourceFrom;

	List<Vector3> vertices;
	List<int> triangles;
	List<Vector2> uvs;
	List<string> ray;

	public float PixelSize = 0.05f;

	int Witdh = 30;
	int Height = 20;

	MeshFilter mf;
	Mesh mesh;
	MeshRenderer mr;

	int[,] ColorIdInt;
	int ColorInt;

	string storedColor = "Black";
	int storedX = 0;
	int storedY = 0;
	int storedCursorPosX = 0;
	int storedCursorPosY = 0;

	//30w by 20h pixelscreen
	void Start () {
		ResetScreen();
		input = input.GetComponent<InputConnector>();
		output = output.GetComponent<OutputConnecter>();
		output.Data = new string[2];
		output.DataId = new string[2] {
			"Input.ScreenGetX",
			"Input.ScreenGetY"
		};
	}

	void RemovePixelAt(int X, int Y) {
		int i = 0;
		foreach(string rays in ray) {
			if(rays.Split(' ')[0] == X.ToString() && rays.Split(' ')[1] == Y.ToString()) {
				mf.mesh.SetTriangles(new int[] {i + 0, i + 2, i + 1, i + 2, i + 3, i + 1}, ColorIdInt[X,Y]); //0,2,1 2,3,1
			}
			i++;
		}
		ColorIdInt[X,Y] = 0;
	}

	void ResetScreen() {
		ray = new List<string>();
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();

		ColorIdInt = new int[Witdh,Height];
		mf = ScreenObject.GetComponent<MeshFilter>();
		mesh = new Mesh();
		mr = ScreenObject.GetComponent<MeshRenderer>();
		mf.mesh = mesh;
		mf.mesh.Clear();
		mr.materials = mats;
		mesh.subMeshCount = 9;
	}

	void SetPixelUpdate(float X, float Y, string ColorId) {
		if(X < 0 || X >= Witdh || Y < 0 || Y >= Height) {
			return;
		}
		int SaveX = Mathf.RoundToInt(X);
		int SaveY = Mathf.RoundToInt(Y);
		X = Mathf.RoundToInt(X);
		Y = Mathf.RoundToInt(Y);
		X = X * PixelSize;
		Y = Y * PixelSize;

		Vector3[] verticles = new Vector3[4];

		verticles[0] = new Vector3(X + 0, Y + 0, -0.01f); //0
		verticles[1] = new Vector3(X + PixelSize, Y + 0, -0.01f); //1 - 1
		verticles[2] = new Vector3(X + 0, Y + PixelSize, -0.01f); //2 - 1
		verticles[3] = new Vector3(X + PixelSize, Y + PixelSize, -0.01f); //3

		int[] tri = new int[6];

		tri[0] = vertices.Count + 0; //0 //0
		tri[1] = vertices.Count + 2; //3 //2
		tri[2] = vertices.Count + 1; //1 //1

		tri[3] = vertices.Count + 2; //4 //2
		tri[4] = vertices.Count + 3; //5 //3
		tri[5] = vertices.Count + 1; //2 //1

		foreach(Vector3 V3 in verticles) {
			vertices.Add(V3);
		}
		foreach(int TS in tri) {
			triangles.Add(TS);
			ray.Add(X + " " + Y);
		}

		mesh.vertices = vertices.ToArray();
		//mesh.triangles = triangles.ToArray();

		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2(0,0);
		uv[1] = new Vector2(1,0);
		uv[2] = new Vector2(0,1);
		uv[3] = new Vector2(1,1);

		foreach(Vector2 SingleUV in uv) {
			uvs.Add(SingleUV);
		}

		mesh.uv = uvs.ToArray();

		ColorInt = 0;

		switch(ColorId) {
			case "Red":
				ColorInt = 3;
			ColorIdInt[SaveX,SaveY] = 4;
				break;
			case "Green":
				ColorInt = 2;
			ColorIdInt[SaveX,SaveY] = 3;
				break;
			case "Blue":
				ColorInt = 1;
			ColorIdInt[SaveX,SaveY] = 2;
				break;
			case "Black":
				ColorInt = 0;
			ColorIdInt[SaveX,SaveY] = 1;
				break;
			case "White":
				ColorInt = 4;
			ColorIdInt[SaveX,SaveY] = 5;
				break;
			case "Yellow":
				ColorInt = 5;
			ColorIdInt[SaveX,SaveY] = 6;
				break;
			case "Orange":
				ColorInt = 6;
			ColorIdInt[SaveX,SaveY] = 7;
				break;
			case "Magenta":
				ColorInt = 7;
			ColorIdInt[SaveX,SaveY] = 8;
				break;
			default:
				ColorInt = 0;
			ColorIdInt[SaveX,SaveY] = 1;
				break;
		}
		mf.mesh.SetTriangles(tri, ColorInt + 1);
		mesh.RecalculateNormals();
	}

	void Fill (int X1, int Y1, int X2, int Y2, string ColorId) {
		for(int x = 0; x < X2; x++) {
			for(int y = 0; y < Y2; y++) {
				SetPixelUpdate(x + X1, y + Y1, ColorId);
			}
		}
	}

	int GetColorOnPixel (int X, int Y) {
		if(ColorIdInt[X,Y] != null) {
			return ColorIdInt[X,Y];
		} else {
			return 0;
		}
	}

	void RemovePixel (int X, int Y) {
		//new Vector3(X + 0, Y + 0, -0.01f), new Vector3( X + 0.1f, Y + 0, -0.01f), new Vector3(X + 0, Y + 0.1f, -0.01f), new Vector3(X + 0.1f, Y + 0.1f, -0.01f)
		vertices.Remove(new Vector3(X + 0, Y + 0, -0.01f));
		vertices.Remove(new Vector3(X + 0.1f, Y + 0, -0.01f));
		vertices.Remove(new Vector3(X + 0, Y + 0.1f, -0.01f));
		vertices.Remove(new Vector3(X + 0.1f, Y + 0.1f, -0.01f));
		mesh.vertices = vertices.ToArray();

		ColorIdInt[X,Y] = 0;
	}

	// Update is called once per frame
	void Update () {
		output.Data[0] = storedX.ToString();
		output.Data[1] = storedY.ToString();
		if(input.SendFunctionMailNameI.Count > 0) {
			int SaveCount = input.SendFunctionMailNameI.Count;
			for(int c = 0; c < SaveCount; c++) {
				int i = 0;
				switch(input.SendFunctionMailNameI[i]) {
				case "ScreenSetColor":
					storedColor = input.SendFunctionMailValueI[i];
					break;
				case "ScreenSetX":
					storedX = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[i]), 0, Witdh);
					break;
				case "ScreenSetY":
					storedY = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[i]), 0, Height);
					break;
				case "ScreenSetPixel":
					SetPixelUpdate(storedX, storedY, storedColor);
					break;
				case "ScreenClear":
					ResetScreen();
					break;
				case "ScreenReset":
					ResetScreen();
					break;
				case "ScreenSetCurX":
					storedCursorPosX = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[i]), 0, Witdh);
					break;
				case "ScreenSetCurY":
					storedCursorPosY = Mathf.Clamp(int.Parse(input.SendFunctionMailValueI[i]), 0, Height);
					break;
				case "ScreenShapeRectangle":
					Fill(storedX, storedY, storedCursorPosX, storedCursorPosY, storedColor);
					break;
				}
				input.SendFunctionMailNameI.RemoveAt(i);
				input.SendFunctionMailValueI.RemoveAt(i);
			}
		}
	}
}
