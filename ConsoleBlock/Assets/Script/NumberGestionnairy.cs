using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NumberGestionnairy : MonoBehaviour {

	public ScrollRect ArrowBlocksBoard;
	public List<GameObject> ABBoardObjects;

	public GameObject[] ArrowBlocks;

	[HideInInspector]
	public Vector3[] ArrowBlockPos;

	public string[] ProperEncoding;

	public string DataValue = "";
	public string Encoding = "";

	GameObject Host;

	// Use this for initialization
	void Start () {
		ABBoardObjects = new List<GameObject>();
		ArrowBlockPos = new Vector3[ArrowBlocks.Length];

		for(int i = 0; i < ArrowBlocks.Length; i++) {
			ArrowBlockPos[i] = ArrowBlocks[i].transform.position;
		}
	}

	public void SetPos () {
		for(int i = 0; i < ArrowBlocks.Length; i++) {
			ArrowBlockPos[i] = ArrowBlocks[i].transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < ArrowBlocks.Length; i++) {
			if(ArrowBlocks[i].GetComponent<ArrowDrag>().InDragging && ArrowBlocks[i].GetComponent<ArrowDrag>().IsFragile) {
				//Debug.Log("Sumone gut fragile!");
				GameObject Clone = (GameObject)Instantiate(ArrowBlocks[i].gameObject, ArrowBlockPos[i], ArrowBlocks[i].transform.rotation, ArrowBlocks[i].transform.parent);
				ArrowBlocks[i].GetComponent<ArrowDrag>().IsFragile = true;
				Clone.GetComponent<ArrowDrag>().InDragging = false;
				Clone.name = ArrowBlocks[i].name;
				ArrowBlocks[i] = Clone; //We've got a probl. here!
				Clone.GetComponent<ArrowDrag>().IsFragile = false;

				//Debug.Log(ArrowBlocks[i].name);
			}
		}
		//Create a copy if original arrow block is dragged, delete the copy if dropped when is under is true
	}

	GameObject GetClone (GameObject Original) {
		GameObject Clone = (GameObject)Instantiate(Original.gameObject, Original.transform.position, Original.transform.rotation, Original.transform.parent);
		Original.GetComponent<ArrowDrag>().IsFragile = true;
		Clone.GetComponent<ArrowDrag>().InDragging = false;
		Clone.name = Original.name;
		Clone.GetComponent<ArrowDrag>().IsFragile = false;

		//Clone.transform.SetParent(transform.GetChild(9).GetChild(0).GetChild(0));
		return Clone;
	}

	public void AddABBoardObject (GameObject AB, bool ResetDV, bool BySteps) {
		if(ABBoardObjects.Count == 0 || BySteps) {
			ABBoardObjects.Add(AB);
		} else {
			int index = 0;
			float Position = AB.transform.localPosition.x;
			float CurrentPos;

			for(int i = 0; i < ABBoardObjects.Count; i++) {
				CurrentPos = ABBoardObjects.ToArray()[i].transform.localPosition.x;
				if(Position>CurrentPos) {
					index = i+1;
				}
			}

			ABBoardObjects.Insert(index, AB);
		}
		UpdateABBoard(ResetDV);
	}

	public void RemoveABBoardObject (GameObject AB) {
		ABBoardObjects.Remove(AB);
		UpdateABBoard(true);
	}

	public void UpdateABBoard (bool ResetDV) {
		if(ResetDV) {
			DataValue = "";
		}

		float Height = transform.GetChild(9).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition.y;
		float Width = 33.2f;
		if(ABBoardObjects.Count != 0) {
			Width += (ABBoardObjects.ToArray()[0].GetComponent<RectTransform>().sizeDelta.x+7.4f*2f)/2-2.2f;
		}

		for(int i = 0; i < ABBoardObjects.Count; i++) {
			/*if(i != 0) {
				Width += (ABBoardObjects.ToArray()[i-1].GetComponent<RectTransform>().sizeDelta.x+(7.4f*2f)/2)-2.2f;
			}*/
			ABBoardObjects.ToArray()[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(Width, Height);

			Width += (ABBoardObjects.ToArray()[i].GetComponent<RectTransform>().sizeDelta.x+7.4f*2f)/2-2.2f;
			if(i+1 < ABBoardObjects.Count) {
				Width += (ABBoardObjects.ToArray()[i+1].GetComponent<RectTransform>().sizeDelta.x+7.4f*2f)/2;
			}
				
			if(ResetDV) {
				int BlockId = ABBoardObjects.ToArray()[i].GetComponent<ArrowDrag>().BlockId;
				string BlockDataValue = ABBoardObjects.ToArray()[i].GetComponent<ArrowDrag>().DataValues;
				DataValue += ProperEncoding[BlockId].Replace("@",BlockDataValue.Split(',')[0]);
			}
		}


		ArrowBlocksBoard.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp((Width-33.2f), 30.5f, Mathf.Infinity), 0.0f);
	}

	public void PrepareForOpening (GameObject CurrentHost) {
		Host = CurrentHost;
		//Get the "Encoding Value"
		//Get all the section, *BlockId*&*DataValueOfBlock*/... give array of string
		//Clone a block and put it, in order, in the ABBoardObjects list.
		//Apply the DataValue to each block (Check the sources in arrow block)
		string BlockEncoding = Host.GetComponent<CallNumberEditor>().Encoding;
		if(!string.IsNullOrEmpty(BlockEncoding)) {
			for(int i = 0; i < BlockEncoding.Split('/').Length; i++) {
				int BlockIds = int.Parse(BlockEncoding.Split('/')[i].Split('&')[0]);
				string DataValues = BlockEncoding.Split('/')[i].Split('&')[1];
				if(DataValues.StartsWith("Input.")) {
					DataValue = DataValue.Remove(0,6);
				}
				GameObject CreateClone = GetClone(ArrowBlocks[BlockIds]);
				ArrowBlocks[BlockIds].GetComponent<ArrowDrag>().SetDataValue(DataValues);
				ArrowBlocks[BlockIds].transform.SetParent(transform.GetChild(9).GetChild(0).GetChild(0));
				ArrowBlocks[BlockIds].GetComponent<ArrowDrag>().IsUnder = false;

				AddABBoardObject(ArrowBlocks[BlockIds], false, true);
				ArrowBlocks[BlockIds] = CreateClone;

			}
		}
	}

	public void PrepareForClosing () {
		DataValue = "";
		Encoding = "";
		for(int i = 0; i < ABBoardObjects.Count; i++) {
			if(i != 0) {
				Encoding += "/";
			}
			int BlockId = ABBoardObjects.ToArray()[i].GetComponent<ArrowDrag>().BlockId;
			string BlockDataValue = ABBoardObjects.ToArray()[i].GetComponent<ArrowDrag>().DataValues;
			DataValue += ProperEncoding[BlockId].Replace("@",BlockDataValue.Split(',')[0]);
			Encoding += BlockId+"&"+BlockDataValue;
		}
		foreach (GameObject Victim in ABBoardObjects) {
			Destroy(Victim);
		}
		ABBoardObjects.Clear();

		//HOST IS MISSING THE WOL GAME CRASH
		Host.GetComponent<CallNumberEditor>().DataValue = DataValue;
		Host.GetComponent<CallNumberEditor>().Encoding = Encoding;
		Host = null;
	}

	public static string FixMinusNumbers (string Line) {
		string FixingLine = Line;
		FixingLine = FixingLine.Replace(" /  - ", " / -");
		FixingLine = FixingLine.Replace(" *  - ", " * -");
		FixingLine = FixingLine.Replace(" +  - ", " + -");
		FixingLine = FixingLine.Replace(" -  - ", " - -");
		FixingLine = FixingLine.Replace(" >  - ", " > -");
		FixingLine = FixingLine.Replace(" <  - ", " < -");
		FixingLine = FixingLine.Replace(" >=  - ", " >= -");
		FixingLine = FixingLine.Replace(" <=  - ", " <= -");
		FixingLine = FixingLine.Replace(" ==  - ", " == -");
		FixingLine = FixingLine.Replace(" !=  - ", " != -");
		if(FixingLine.StartsWith(" - ")) {
			FixingLine = FixingLine.Remove(0, 3);
			FixingLine = "-" + FixingLine;
		}
		return FixingLine;
	}

	public string GetEncodingByDataValue (string DV) {
		string ReturnedEncoding = "";
		string EditedDV = DV;
		if(EditedDV.StartsWith(" ")) {
			EditedDV = EditedDV.Remove(0,1);
		}
		if(EditedDV.EndsWith(" ")) {
			EditedDV = EditedDV.Remove(EditedDV.Length-1,1);
		}

		string Line = EditedDV;

		Line = Line.Replace("True", "true").Replace("False", "false");

		Line = Line.Replace("!true", "false");
		Line = Line.Replace("!false", "true");
		Line = Line.Replace(" ", string.Empty);
		Line = Line.Replace("+", " + ");
		Line = Line.Replace("-", " - ");
		Line = Line.Replace(">", " > ");
		Line = Line.Replace("<", " < ");
		Line = Line.Replace(">=", " >= ");
		Line = Line.Replace("<=", " <= ");
		Line = Line.Replace("==", " == ");
		Line = Line.Replace("!=", " != ");
		Line = Line.Replace("*", " * ");
		Line = Line.Replace("/", " / ");
		Line = Line.Replace("^", " ^ ");
		Line = Line.Replace("(", "( ");
		Line = Line.Replace(")", " )");
		Line = Line.Replace("&&", " && ");
		Line = Line.Replace("||", " || ");

		Line = FixMinusNumbers(Line);

		EditedDV = Line;

		EditedDV = EditedDV.Replace("false","False");
		EditedDV = EditedDV.Replace("true","True");
		EditedDV = EditedDV.Replace("!","! ");
		EditedDV = EditedDV.Replace("  "," ");

		bool Error = false;

		for(int i = 0; i < EditedDV.Split(' ').Length; i++) {
			int ArrowId = -1;
			string ArrowDataValue = "";
			for(int ab = 0; ab < ArrowBlocks.Length; ab++) {
				if(ArrowBlocks[ab].GetComponent<ArrowDrag>().Syntax.StartsWith("StartsWith:")) {
					if(EditedDV.Split(' ')[i].StartsWith(ArrowBlocks[ab].GetComponent<ArrowDrag>().Syntax.Replace("StartsWith:",""))) {
						ArrowId = ArrowBlocks[ab].GetComponent<ArrowDrag>().BlockId;
						ArrowDataValue = EditedDV.Split(' ')[i].Replace(ArrowBlocks[ab].GetComponent<ArrowDrag>().Syntax, "");
					}
				} else if(ArrowBlocks[ab].GetComponent<ArrowDrag>().Syntax.StartsWith("Type:")) {
					if(ArrowBlocks[ab].GetComponent<ArrowDrag>().Syntax.Replace("Type:","") == "Number") {
						int n;
						bool isNumeric = int.TryParse(EditedDV.Split(' ')[i], out n);
						if(isNumeric) {
							ArrowId = ArrowBlocks[ab].GetComponent<ArrowDrag>().BlockId;
							ArrowDataValue = n.ToString();
						}
					}
				} else if(ArrowBlocks[ab].GetComponent<ArrowDrag>().Syntax == EditedDV.Split(' ')[i]) {
					ArrowId = ArrowBlocks[ab].GetComponent<ArrowDrag>().BlockId;
					ArrowDataValue = "";
				}
			}

			if(i != 0) {
				ReturnedEncoding += "/";
			}
			if(ArrowId != -1) {
				ReturnedEncoding += ArrowId+"&"+ArrowDataValue;
			} else {
				Error = true;
			}
		}

		if(Error) {
			ReturnedEncoding = "18&"+EditedDV;
		}

		return ReturnedEncoding;
	}


}
