using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class DataFusioScript : MonoBehaviour {

	public InputConnector input1;
	public InputConnector input2;
	public OutputConnecter output;

	public List<string> ErrorList1;
	public List<string> ErrorList2;

	string[] LastSave;

	public Player currentPlayer;
	public Transform Canvas;

	int CurrentDataOperator = 0;
	int ErrorCount = 0;

	bool FirstTime = true;
	bool IsUpd = false;
	bool IsUpdS = false;

	string oldInput1;
	string oldInput2;

	bool Allow;

	// Use this for initialization
	void Start () {
		Canvas = GameObject.Find("Canvas").transform;
		input1 = input1.GetComponent<InputConnector>();
		input2 = input2.GetComponent<InputConnector>();
		output = output.GetComponent<OutputConnecter>();
		ErrorList1 = new List<string>();
		ErrorList2 = new List<string>();
		oldInput1 = input1.name;
		oldInput2 = input2.name;
	}

	void UpdateErrorCount () {
		ErrorCount = 0;
		foreach(string CData in ErrorList1) {
			foreach(string CData2 in ErrorList2) {
				if(CData == CData2) {
					ErrorCount++;
				}
			}
		}
	}

	bool GlobalErrorIsHigh () {
		int E = 0;
		foreach(string CData in input1.DataId) {
			foreach(string CData2 in input2.DataId) {
				if(CData == CData2) {
					E++;
				}
			}
		}
		return (E>0);
	}
	
	// Update is called once per frame
	void Update () {
		//*** IS UPDATED SYSTEM ***//
		if(input1.name != oldInput1 || input2.name != oldInput2) {
			IsUpdS = true;
		}
		oldInput1 = input1.name;
		oldInput2 = input2.name;

		//*** FIXING SYSTEM ***//
		if(Input.GetKeyDown(KeyCode.Return) && currentPlayer != null) {
			if(CurrentDataOperator < ErrorList2.Count) {
				ErrorList2[CurrentDataOperator] = ("Input." + Canvas.Find("Gestionnairy_UI").Find("InputField").GetComponent<InputField>().text);
			}
			UpdateErrorCount();
			Canvas.Find("Gestionnairy_UI").Find("IconSlider").Find("Pos").Find("ErrorCount").GetComponent<Text>().text = ErrorCount.ToString();
		}


		//Debug.Log(ErrorCount + ", " + IsUpdS + ", " + input1.DataId.Length + ", " +  input2.DataId.Length);
		if((ErrorList1.Count>0 && ErrorList2.Count>0 && ErrorCount == 0) || !GlobalErrorIsHigh()) {
			if(!GlobalErrorIsHigh()) {
				output.DataId = input1.DataId.Concat(input2.DataId.ToArray()).ToArray();
				output.Data = input1.Data.Concat(input2.Data.ToArray()).ToArray();
			} else {
				List<string> PreLoadedI1DataId = new List<string>();
				List<string> PreLoadedI2DataId = new List<string>();
				PreLoadedI1DataId = input1.DataId.ToList();
				bool ind = false;
				int n = 0;
				foreach(string CData in input2.DataId) {
					ind = false;
					n = 0;
					foreach(string CData2 in ErrorList1) {
						//Debug.Log(n + " / " + CData + ", " + CData2);
						if(CData == CData2) {
							PreLoadedI2DataId.Add(ErrorList2.ToArray()[n]);
							ind = true;
							break;
						}
						n++;
					}
					if(!ind) {
						PreLoadedI2DataId.Add(CData);
					}
				}

				output.DataId = PreLoadedI1DataId.Concat(PreLoadedI2DataId.ToArray()).ToArray();
				output.Data = input1.Data.Concat(input2.Data.ToArray()).ToArray();
			}
		}
	}
		
	public void UpdateErrorFix () {
		//*** AJUST VALUE BEFORE QUITTING ***//
		CurrentDataOperator = 0;
		ErrorCount = 0;
	}

	public void Next () {
		if(ErrorList1.Count == 0 && ErrorList2.Count == 0) {
			Allow = false;
		} else {
			Allow = true;
		}
		if(CurrentDataOperator + 1 < ErrorList1.Count) {
			CurrentDataOperator++;
			if(Canvas.Find("Gestionnairy_UI").gameObject.activeInHierarchy && Allow) {
				Canvas.Find("Gestionnairy_UI").Find("Input1Id").GetComponent<Text>().text = ErrorList1.ToArray()[CurrentDataOperator];
				Canvas.Find("Gestionnairy_UI").Find("InputField").GetComponent<InputField>().text = ErrorList2.ToArray()[CurrentDataOperator].Replace("Input.", string.Empty);
			} else {
				Canvas.Find("Gestionnairy_UI").Find("Input1Id").GetComponent<Text>().text = "Input.Null";
				Canvas.Find("Gestionnairy_UI").Find("InputField").GetComponent<InputField>().text = string.Empty;
			}
		}
	}

	public void Previous () {
		if(ErrorList1.Count == 0 && ErrorList2.Count == 0) {
			Allow = false;
		} else {
			Allow = true;
		}
		if(CurrentDataOperator - 1 >= 0) {
			CurrentDataOperator--;
			if(Canvas.Find("Gestionnairy_UI").gameObject.activeInHierarchy && Allow) {
				Canvas.Find("Gestionnairy_UI").Find("Input1Id").GetComponent<Text>().text = ErrorList1.ToArray()[CurrentDataOperator];
				Canvas.Find("Gestionnairy_UI").Find("InputField").GetComponent<InputField>().text = ErrorList2.ToArray()[CurrentDataOperator].Replace("Input.", string.Empty);
			} else {
				Canvas.Find("Gestionnairy_UI").Find("Input1Id").GetComponent<Text>().text = "Input.Null";
				Canvas.Find("Gestionnairy_UI").Find("InputField").GetComponent<InputField>().text = string.Empty;
			}
		}
	}

	// Opening
	public void CheckError () {
		//*** ANALIZING ERROR SYSTEM ***//
		if(FirstTime || IsUpdS) {
			ErrorList1.Clear();
			ErrorList2.Clear();

			CurrentDataOperator = 0;
			foreach(string CData in input1.DataId) {
				foreach(string CData2 in input2.DataId) {
					if(CData == CData2) {
						ErrorList1.Add(CData);
						ErrorList2.Add(CData2);
					}
				}
			}
			UpdateErrorCount();
			IsUpdS = false;
		}

		//*** BUTTON CONFIG ***//
		//Debug.Log(Canvas.FindChild("Gestionnairy_UI").GetChild(0).GetChild(1).childCount);
		Canvas.Find("Gestionnairy_UI").GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = ErrorCount.ToString();
		Canvas.Find("Gestionnairy_UI").Find("<Arrow").GetComponent<Button>().onClick.AddListener(()=>{
			Previous();
		});
		Canvas.Find("Gestionnairy_UI").Find(">Arrow").GetComponent<Button>().onClick.AddListener(()=>{
			Next();
		});

		//*** PREPARING THE FIRST PAGE ***//
		if(ErrorCount == 0) {
			Allow = false;
		} else {
			Allow = true;
		}
		if(Canvas.Find("Gestionnairy_UI").gameObject.activeInHierarchy && Allow) {
			Canvas.Find("Gestionnairy_UI").Find("Input1Id").GetComponent<Text>().text = ErrorList1.ToArray()[CurrentDataOperator];
			Canvas.Find("Gestionnairy_UI").Find("InputField").GetComponent<InputField>().text = ErrorList2.ToArray()[CurrentDataOperator].Replace("Input.", string.Empty);
		} else {
			Canvas.Find("Gestionnairy_UI").Find("Input1Id").GetComponent<Text>().text = "Input.Null";
			Canvas.Find("Gestionnairy_UI").Find("InputField").GetComponent<InputField>().text = string.Empty;
		}

		//*** RESETING FIRST-TIME ***//
		FirstTime = false;
	}

	public static T[] ConcatArrays<T>(params T[][] list)
	{
		var result = new T[list.Sum(a => a.Length)];
		int offset = 0;
		for (int x = 0; x < list.Length; x++)
		{
			list[x].CopyTo(result, offset);
			offset += list[x].Length;
		}
		return result;
	}
}