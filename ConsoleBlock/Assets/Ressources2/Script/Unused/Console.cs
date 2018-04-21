using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

public class Console : MonoBehaviour {

	public InputConnector InputI;
	public OutputConnecter OutputO;
	public EnergyInputSaver EnergieInput;
	public string Text;
	public string[] Lines;

	public GameObject TextPrefab;
	public Transform Canvas;
	public Transform CanvasPos;

	public string[] GetArray;
	public string[] GetValues;

	public List<string> SetArray;
	public List<string> SetValues;

	public List<string> SendFunctionMailName;
	public List<string> SendFunctionMailValue;

	char[] Codes;

	List<string> FunctionNames; //Private
	List<GameObject> FunctionObjects;

	float DistanceBettweenBlocks = 50f;
	public float Consomation = 0.3f;

	public bool isOp;

	// Use this for initialization
	void Start () {
		SetArray = new List<string>();
		SetValues = new List<string>();
		FunctionNames = new List<string>();
		FunctionObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(OutputO.DataId.Length);
		SetArray.Clear();
		SetValues.Clear();
		Lines = new string[Text.Split('\n').Length];
		Lines = Text.Split('\n');
		if(!isOp && Consomation <= EnergieInput.GetComponent<EnergyInputSaver>().EnergySource) {
			ReadFunction("Update");
			if(InputI.SendFunctionMailNameI.Count > 0) {
				int SaveCount = InputI.SendFunctionMailNameI.Count;
				for(int c = 0; c < SaveCount; c++) {
					int i = 0;
					switch(InputI.SendFunctionMailNameI[i]) {
					case "ReadFunction":
						if(FunctionNames.Contains(InputI.SendFunctionMailValueI[i])) {
							ReadFunction(InputI.SendFunctionMailValueI[i]);
						}
						break;
					default:
						break;
					}
					InputI.SendFunctionMailNameI.RemoveAt(i);
					InputI.SendFunctionMailValueI.RemoveAt(i);
				}
			}
			EnergieInput.GetComponent<EnergyInputSaver>().EnergySource = EnergieInput.GetComponent<EnergyInputSaver>().EnergySource - Consomation;
		} else {
			if(Input.GetKeyDown(KeyCode.Return) && isOp) {
				UpdateFunctionsList();
			}
		}
		if(InputI != null) {
			GetArray = InputI.DataId;
			GetValues = InputI.Data;
		}
		if(OutputO != null) {
			OutputO.DataId = SetArray.ToArray();
			OutputO.Data = SetValues.ToArray();
		}
	}

	string SearchInString(string StartWith, string EndWith, string LineInsert) {
		int First = LineInsert.IndexOf(StartWith);
		int Last = LineInsert.LastIndexOf(EndWith);
		return LineInsert.Substring(First + 1, Last - First - 1);
	}

	void SendAllMail () {
		OutputO.SendFunctionMailNameI = SendFunctionMailName;
		OutputO.SendFunctionMailValueI = SendFunctionMailValue;
		SendFunctionMailName = new List<string>();
		SendFunctionMailValue = new List<string>();
	}

	int ExecuteLine (string LineToExecute, int Value) {
		string FindVariable = LineToExecute;

		//MAY CAUSE FATAL ERROR v (but also, it can break game if remove)
		bool quit = false;
		int blocker = 0;
		while(FindVariable.Contains("Input.")) {
			int ind = FindVariable.IndexOf("Input.")+6;
			//Debug.Log("ind: " + ind);
			int e = 0;
			string comp = "Input.";
			string value = "error";
			for(int d = 0; d < 256; d++) {
				//Debug.Log(FindVariable[ind+d]);
				if(char.IsLetterOrDigit(FindVariable[ind+d])) {
					comp+=FindVariable[ind+d].ToString();
				} else {
					break;
				}
			}
			e=ind-6+comp.Length;
			//Debug.Log("comp: " + comp);
			//Debug.Log("e: " + e);
			for(int inp = 0; inp < InputI.DataId.Length; inp++) {
				if(comp == InputI.DataId[inp]) {
					value = InputI.Data[inp];
				}
			}
			if(value == "error") {
				quit = true;
			}
			FindVariable = FindVariable.Remove(ind-6, e-ind+6);
			FindVariable = FindVariable.Insert(ind-6, value);
			//Debug.Log(FindVariable.Substring(ind-6, e-ind+6));
			//Debug.Log("FindVariable: " + FindVariable);
			if(quit || blocker >= 64) {
				break;
			}
			blocker++;
		}

		foreach(string FunctionRead in FunctionNames) {
			if(LineToExecute.Replace("\t", string.Empty).StartsWith(FunctionRead) || LineToExecute.Replace("\t", string.Empty).StartsWith(FunctionRead.Replace(" ", string.Empty))) {
				ReadFunction(FunctionRead.Replace("()", string.Empty).Replace(" ", string.Empty));
			}
		}
		if(LineToExecute.Replace("\t", string.Empty).StartsWith("print") || LineToExecute.Replace("\t", string.Empty).StartsWith("Debug.Log") && LineToExecute.EndsWith(";")) {
			Debug.Log(SearchInString("(\"", "\")", LineToExecute.Replace("\t", string.Empty)).Replace("\"", string.Empty));
		} else if(LineToExecute.Replace("\t", string.Empty).StartsWith("if") && LineToExecute.EndsWith("{")) {
			if(CompileClass.ReadBoolean(SearchInString("(", ")", FindVariable)) == "true") {
				int TabsCount = 0;
				foreach(char c in LineToExecute) {
					if(c == '\t') TabsCount++;
				}
				for(int l = Value + 1; l < Lines.Length; l++) {
					int CLTabsCount = 0;
					foreach(char c in Lines[l]) {
						if(c == '\t') CLTabsCount++;
					}
					if(Lines[l].EndsWith("}") && CLTabsCount == TabsCount) {
						break;
					} else if(Lines[l].EndsWith("{")) {
						for(int LineScanner = l; Lines[LineScanner].EndsWith("}"); LineScanner++) {
							l = LineScanner;
						}
						l++;
					} else {
						ExecuteLine(Lines[l], l);
					}
				} 
			} else {
				/*int SCount = 0;
				//Else/ElseIf
				int TabsCount = 0;
				foreach(char c in LineToExecute) {
					if(c == '\t') TabsCount++;
				}
				for(int l = Value + 1; l < Lines.Length; l++) {
					int CLTabsCount = 0;
					foreach(char c in Lines[l]) {
						if(c == '\t') CLTabsCount++;
					}
					if(Lines[l].EndsWith("}") && CLTabsCount == TabsCount) {
						break;
					} else if(Lines[l].EndsWith("{")) {
						for(int LineScanner = l; Lines[LineScanner].EndsWith("}"); LineScanner++) {
							l = LineScanner;
						}
						l++;
					} else {
						SCount++;
					}
				}*/
			}
		} else if(LineToExecute.Replace("\t", string.Empty).StartsWith("Output.Send") && LineToExecute.EndsWith(");")) {

			//All Output witch contains var: Var's name will be replace with value

			string IncludeVar = LineToExecute;

			//MAY CAUSE FATAL ERROR v (but also, it can break game if remove)
			bool quit2 = false;
			int blocker2 = 0;
			while(IncludeVar.Contains("Input.")) {
				int ind = IncludeVar.IndexOf("Input.")+6;
				//Debug.Log("ind: " + ind);
				int e = 0;
				string comp = "Input.";
				string value = "error";
				for(int d = 0; d < 256; d++) {
					//Debug.Log(FindVariable[ind+d]);
					if(char.IsLetterOrDigit(IncludeVar[ind+d])) {
						comp+=IncludeVar[ind+d].ToString();
					} else {
						break;
					}
				}
				e=ind-6+comp.Length;
				//Debug.Log("comp: " + comp);
				//Debug.Log("e: " + e);
				for(int inp = 0; inp < InputI.DataId.Length; inp++) {
					if(comp == InputI.DataId[inp]) {
						value = InputI.Data[inp];
					}
				}
				if(value == "error") {
					quit2 = true;
				}
				IncludeVar = IncludeVar.Remove(ind-6, e-ind+6);
				IncludeVar = IncludeVar.Insert(ind-6, value);
				//Debug.Log(FindVariable.Substring(ind-6, e-ind+6));
				//Debug.Log("FindVariable: " + FindVariable);
				if(quit2 || blocker2 >= 64) {
					break;
				}
				blocker2++;
			}



			if(IncludeVar.Replace("\t", string.Empty) == "Output.SendMail();") {
				SendAllMail();
			} else if(IncludeVar.Replace("\t", string.Empty).StartsWith("Output.SendColor") && NumberFree(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty))) {
				SendingMessage(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty), IncludeVar);
			} else if(IncludeVar.Replace("\t", string.Empty).StartsWith("Output.SendFloat") && MadeForFloat(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty))) {
				SendingMessage(CompileClass.ReadFloat(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty)).ToString(), IncludeVar);
			} else if(IncludeVar.Replace("\t", string.Empty).StartsWith("Output.SendInt") && MadeForInt(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty))) {
				SendingMessage(CompileClass.ReadIntegrer(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty)).ToString(), IncludeVar);
			} else if((IncludeVar.Replace("\t", string.Empty).StartsWith("Output.SendFunc") || IncludeVar.Replace("\t", string.Empty).StartsWith("Output.SendString"))) {
				SendingMessage(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty), IncludeVar);
			} else if(IncludeVar.Replace("\t", string.Empty).StartsWith("Output.SendBoolean")) {
				if(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty) == "true" || SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty) == "false") {
					SendingMessage(SearchInString("(", ")", IncludeVar).Split(',')[1].Replace(" ", string.Empty), IncludeVar);
				}
			} else if(IncludeVar.Replace("\t", string.Empty).StartsWith("Output.SendController")) {
				string Type; //Mail or Save
				string Port; //1 or 2 TODO: 1/2/1/2
				string Action; //Output.
				string Data; //Value
				IncludeVar = SearchInString("(", ")", IncludeVar).Replace(" ", string.Empty);
				if(IncludeVar.Split(',').Length == 4) {
					Type = IncludeVar.Split(',')[1];
					//Debug.Log(IncludeVar.Split(',')[0]);
					Port = IncludeVar.Split(',')[0];
					Action = IncludeVar.Split(',')[2];
					Data = IncludeVar.Split(',')[3];
					//Debug.Log("I'm preparing myself to send a new mail! Here's the data:("+Port+","+Type+","+Action+","+Data+")");
					SendingMessage(Data, Port + "," + Type + "," + Action, true); //"Port,Type,Action","Value"
				}
			}
		} else if(LineToExecute.Replace("\t", string.Empty).StartsWith("Input.Clear();")) {
			InputI.Data.ToList().Clear();
			InputI.DataId.ToList().Clear();
			InputI.SendFunctionMailNameI.Clear();
			InputI.SendFunctionMailValueI.Clear();
		} else if(LineToExecute.Replace("\t", string.Empty).StartsWith("Output.Clear();")) {
			OutputO.Data.ToList().Clear();
			OutputO.DataId.ToList().Clear();
			OutputO.SendFunctionMailNameI.Clear();
			OutputO.SendFunctionMailValueI.Clear();
		} else {
			if(LineToExecute.EndsWith(";") && LineToExecute.Contains(" = ")) {
				string[] parts = LineToExecute.Replace("\t", string.Empty).Replace(";", string.Empty).Replace(" = ", "\t").Split('\t');
				//OutputO.Data[OutputO.DataId.ToList().IndexOf(parts[0])] = parts[1];
				if(OutputO.DataId.ToList().Contains(parts[0])) {
					OutputO.Data[OutputO.DataId.ToList().IndexOf(parts[0])] = parts[1];
				} else {
					Debug.Log("Fail: "  + parts[0]);
				}
			}
		}
		return 0;
	}

	void SendingMessage (string Message, string LineToExecute) {
		if(!SendFunctionMailName.Contains(SearchInString("(", ")", LineToExecute).Split(',')[0])) {
			SendFunctionMailName.Add(SearchInString("(", ")", LineToExecute).Split(',')[0]);
			SendFunctionMailValue.Add(Message);
		} else {
			int index = SendFunctionMailName.IndexOf(SearchInString("(", ")", LineToExecute).Split(',')[0]);
			SendFunctionMailName.Remove(SearchInString("(", ")", LineToExecute).Split(',')[0]);
			SendFunctionMailName.Add(SearchInString("(", ")", LineToExecute).Split(',')[0]);
			SendFunctionMailValue.RemoveAt(index);
			SendFunctionMailValue.Add(Message);
		}
	}

	void SendingMessage (string Message, string LineToExecute, bool isAdaptForController) {
		if(isAdaptForController) {
			if(!SendFunctionMailName.Contains(LineToExecute)) {
				SendFunctionMailName.Add(LineToExecute);
				SendFunctionMailValue.Add(Message);
			} else {
				int index = SendFunctionMailName.IndexOf(LineToExecute);
				SendFunctionMailName.Remove(LineToExecute);
				SendFunctionMailName.Add(LineToExecute);
				SendFunctionMailValue.RemoveAt(index);
				SendFunctionMailValue.Add(Message);
			}
		} else {
			if(!SendFunctionMailName.Contains(SearchInString("(", ")", LineToExecute).Split(',')[0])) {
				SendFunctionMailName.Add(SearchInString("(", ")", LineToExecute).Split(',')[0]);
				SendFunctionMailValue.Add(Message);
			} else {
				int index = SendFunctionMailName.IndexOf(SearchInString("(", ")", LineToExecute).Split(',')[0]);
				SendFunctionMailName.Remove(SearchInString("(", ")", LineToExecute).Split(',')[0]);
				SendFunctionMailName.Add(SearchInString("(", ")", LineToExecute).Split(',')[0]);
				SendFunctionMailValue.RemoveAt(index);
				SendFunctionMailValue.Add(Message);
			}
		}
	}

	bool NumberFree (string str) {
		foreach(char c in str) {
			if(!char.IsLetter(c) || c == ' ') {
				return false;
			}
		}
		return true;
	}

	bool MadeForFloat (string str) {
		if(!NumberFree(str)) {
			foreach(char c in str) {
				if(c == '.') {
					return true;
				}
			}
			return true;
		} else {
			return false;
		}
	}

	bool MadeForInt (string str) {
		if(!NumberFree(str)) {
			foreach(char c in str) {
				if(c == '.') {
					return false;
				}
			}
			return true;
		} else {
			return false;
		}
	}

	public void ReadFunction (string FunctionName) {
		UpdateSaveSends();
		for(int i = 0; i < Lines.Length; i++) {
			if(Lines[i].Replace("\t", string.Empty).StartsWith("FuncList " + FunctionName) && Lines[i].EndsWith("() {")) {
				StartCoroutine(ReadFuncList(FunctionName, i));
				break;
			}
			if(Lines[i].Replace("\t", string.Empty).StartsWith("void " + FunctionName) || Lines[i].Replace("\t", string.Empty).StartsWith("func " + FunctionName) && Lines[i].EndsWith("() {")) {
				for(int l = i + 1; l < Lines.Length; l++) {
					if(Lines[l].EndsWith("{")) {
						ExecuteLine(Lines[l], l);
						for(int r = l; r < Lines.Length; r++) {
							if(Lines[r].EndsWith("}")) {
								l = r/* + 1*/;
								break;
							}
						}
					} else if(Lines[l] == "}") {
						break;
					} else {
						ExecuteLine(Lines[l], l);
					}
				}
			}
		}
	}

	IEnumerator ReadFuncList (string Name, int StartLine) {
		for(int l = StartLine + 1; l < Lines.Length; l++) {
			if(Lines[l].EndsWith("{")) {
				if(Lines[l].Replace("\t", string.Empty).StartsWith("Time.Wait(")) {
					yield return new WaitForSeconds(float.Parse(SearchInString("(", ")", Lines[l].Replace("\t", string.Empty))));
				} else {
					ExecuteLine(Lines[l], l);
				}
				for(int r = l; r < Lines.Length; r++) {
					if(Lines[r].EndsWith("}")) {
						l = r + 1;
						break;
					}
				}
			} else if(Lines[l] == "}") {
				break;
			} else {
				if(Lines[l].Replace("\t", string.Empty).StartsWith("Wait(")) {
					yield return new WaitForSeconds(CompileClass.ReadFloat(SearchInString("(", ")", Lines[l].Replace("\t", string.Empty))));
				} else {
					ExecuteLine(Lines[l], l);
				}
			}
		}
		yield return new WaitForSeconds(0.0f);
	}

	public void UpdateFunctionsList () {
		//Search for a list of functions
		FunctionNames.Clear();
		foreach(string ScanningLine in Lines) {
			if(ScanningLine.Replace("\t", string.Empty).StartsWith("void ") || ScanningLine.Replace("\t", string.Empty).StartsWith("func ") || ScanningLine.Replace("\t", string.Empty).StartsWith("FuncList ") && ScanningLine.EndsWith(" {")) {
				FunctionNames.Add(ScanningLine.Replace("void ", string.Empty).Replace("func ", string.Empty).Replace("FuncList ", string.Empty).Replace(" {", string.Empty).Replace("\t", string.Empty));
			}
		}

		//Delete Existing Objects
		if(FunctionObjects != null) {
			for(int o = 0; o < CanvasPos.childCount; o++) {
				Destroy(CanvasPos.GetChild(o).gameObject);
			}
		}
		FunctionObjects.Clear();

		//Creating new ones (Yay!)
		for(int i = 0; i < FunctionNames.Count; i++) {
			GameObject CObject = (GameObject)Instantiate(TextPrefab, CanvasPos.position - (Vector3.up * i * DistanceBettweenBlocks), Quaternion.identity);
			CObject.transform.parent = CanvasPos;
			CObject.transform.GetChild(0).GetComponent<Text>().text = FunctionNames.ToArray()[i];

			CObject.GetComponent<Button>().onClick.AddListener(()=>{
				ReadFunction(CObject.transform.GetChild(0).GetComponent<Text>().text);
			});

			FunctionObjects.Add(CObject);
		}
	}

	void UpdateSaveSends () {
		int Count = 0;
		List<string> names = new List<string>();
		for(int i = 0; i < Lines.Length; i++) {
			if(Lines[i].EndsWith(";") && Lines[i].Contains(" = ")) {
				//Debug.Log(Lines[i].Replace("\t", "").Replace(" = ", "\t").Split('\t')[0] + ", " + Lines[i].Replace("\t", "").Replace(" = ", "\t").Split('\t')[1]);
				names.Add(Lines[i].Replace("\t", "").Replace(" = ", "\t").Split('\t')[0]);
				Count++;
			}
		}
		OutputO.Data = new string[Count];
		OutputO.DataId = new string[Count];
		int i2 = 0;
		foreach(string curName in names) {
			//Debug.Log("Added " + curName);
			OutputO.DataId[i2] = curName;
			//Debug.Log(OutputO.DataId[0]);
			i2++;
		}
	}
}

public sealed class CompileClass {
	public static string ReadBoolean (string Operation) {
		string Line = Operation;
		Line = Line.Replace("!true", "false");
		Line = Line.Replace("!false", "true");
		Line = Line.Replace(" ", string.Empty);
		//Line = Line.Replace("f", string.Empty);
		//Line = Line.Replace("alse", "false");
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
		Line = Line.Replace("True", "true").Replace("False", "false");
		int PairStarting = Line.Split('(').Length - 1;
		int PairEnding = Line.Split(')').Length - 1;
		Line = CompileClass.FixMinusNumbers(Line);
		if(PairStarting == PairEnding) {
			if(Line.Contains("(") && Line.Contains(")")) {
				int RepeatCount = PairStarting;
				for(int i = 0; i < RepeatCount; i++) {
					Line = Line.Replace(CompileClass.ParenthesesReader(Line, true), CompileClass.ReadBoolean(CompileClass.ParenthesesReader(Line, false)).ToString());
				}
			}
		} else {
			return "false";
		}
		string[] LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "^") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " ^ " + LineParts[i + 1]).ToString(), Mathf.Pow(float.Parse(LineParts[i - 1]), float.Parse(LineParts[i + 1])).ToString());
				}
			}
		}
		Line = CompileClass.FixMinusNumbers(Line);
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "/") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " / " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])/float.Parse(LineParts[i + 1])).ToString());
				}
			}
			if(LineParts[i] == "*") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " * " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])*float.Parse(LineParts[i + 1])).ToString());
				}
			}
		}
		Line = CompileClass.FixMinusNumbers(Line);
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "+") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " + " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])+float.Parse(LineParts[i + 1])).ToString());
				}
			}
			if(LineParts[i] == "-") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " - " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])-float.Parse(LineParts[i + 1])).ToString());
				}
			}
		}
		Line = CompileClass.FixMinusNumbers(Line);
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == ">") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " > " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])>float.Parse(LineParts[i + 1])).ToString());
				}
			}
			if(LineParts[i] == "<") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " < " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])<float.Parse(LineParts[i + 1])).ToString());
				}
			}
			if(LineParts[i] == ">=") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " >= " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])>=float.Parse(LineParts[i + 1])).ToString());
				}
			}
			if(LineParts[i] == "<=") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " <= " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])<=float.Parse(LineParts[i + 1])).ToString());
				}
			}
		}
		Line = Line.Replace("True", "true").Replace("False", "false");
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "==") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " == " + LineParts[i + 1]).ToString(), (LineParts[i - 1] == LineParts[i + 1]).ToString());
				}
			}
			if(LineParts[i] == "!=") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " != " + LineParts[i + 1]).ToString(), (LineParts[i - 1] != LineParts[i + 1]).ToString());
				}
			}
		}
		Line = Line.Replace("True", "true").Replace("False", "false");
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "&&") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1].ToString().ToLower() + " && " + LineParts[i + 1].ToString().ToLower()), ((LineParts[i - 1].ToString() == "true") && (LineParts[i + 1].ToString() == "true")).ToString().ToLower());
				}
			}
		}
		Line = Line.Replace("True", "true").Replace("False", "false");
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "||") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1].ToString().ToLower() + " || " + LineParts[i + 1].ToString().ToLower()), ((LineParts[i - 1].ToString() == "true") || (LineParts[i + 1].ToString() == "true")).ToString().ToLower());
				}
			}
		}
		Line = Line.Replace("True", "true").Replace("False", "false");
		return Line;
	}

	public static string ParenthesesReader (string Line, bool KeepSigns) {
		if(Line.Contains("(") && Line.Contains(")")) {
			string Expression = Line;
			int match = 0;
			int EndIndex = 0;
			for(int i = Expression.IndexOf('(') + 1; i < Expression.Length; i++) {
				//Debug.Log("Current Count: " + i + ", matchCount:" + match);
				if (match == 0 && Expression.ToCharArray()[i] == ')') {
					EndIndex = i;
					break;
				}
				if(Expression.ToCharArray()[i] == '(') {
					match++;
				}
				if(Expression.ToCharArray()[i] == ')') {
					match--;
				}
			}
			if(KeepSigns) {
				if(EndIndex + 1 != Expression.IndexOf('(')) {
					return Line.Substring(Expression.IndexOf('('), EndIndex - Expression.IndexOf('(') + 1);
				} else {
					return "()";
				}
			} else {
				if(EndIndex + 1 != Expression.IndexOf('(')) {
					return Line.Substring(Expression.IndexOf('(') + 1, EndIndex - Expression.IndexOf('(') - 1);
				} else {
					return "";
				}
			}
			//Debug.Log(Line.Substring(Expression.IndexOf('('), EndIndex - Expression.IndexOf('(') + 1));
		} else {
			return Line;
		}
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

	public static float ReadFloat (string Operation) {
		string Line = Operation;
		if(Line.StartsWith(" ")) {
			Line.Remove(0);
		}
		Line = Line.Replace(" ", "");
		//Line = Line.Replace("f", "");
		Line = Line.Replace("+", " + ");
		Line = Line.Replace("-", " - ");
		Line = Line.Replace("*", " * ");
		Line = Line.Replace("/", " / ");
		Line = Line.Replace("^", " ^ ");
		Line = Line.Replace("(", "( ");
		Line = Line.Replace(")", " )");
		int PairStarting = Line.Split('(').Length - 1;
		int PairEnding = Line.Split(')').Length - 1;
		if(PairStarting == PairEnding) {
			if(Line.Contains("(") && Line.Contains(")")) {
				int RepeatCount = PairStarting;
				for(int i = 0; i < RepeatCount; i++) {
					Line = Line.Replace(CompileClass.ParenthesesReader(Line, true), CompileClass.ReadFloat(CompileClass.ParenthesesReader(Line, false)).ToString());
				}
			}
		} else {
			return 0.0f;
		}
		Line = CompileClass.FixMinusNumbers(Line);
		string[] LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "^") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " ^ " + LineParts[i + 1]).ToString(), (Mathf.Pow(float.Parse(LineParts[i - 1]), float.Parse(LineParts[i + 1]))).ToString());
				}
			}
		}
		Line = CompileClass.FixMinusNumbers(Line);
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "/") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " / " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])/float.Parse(LineParts[i + 1])).ToString());
				}
			}
			if(LineParts[i] == "*") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " * " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])*float.Parse(LineParts[i + 1])).ToString());
				}
			}
		}
		Line = CompileClass.FixMinusNumbers(Line);
		LineParts = Line.Split(' ');
		for(int i = 0; i < LineParts.Length; i++) {
			if(LineParts[i] == "+") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					//BUG ICI
					//Debug.Log("Parse1: " + LineParts[i - 1]);
					//Debug.Log("Parse2: " + LineParts[i + 1]);
					Line = Line.Replace((LineParts[i - 1] + " + " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])+float.Parse(LineParts[i + 1])).ToString());
				}
			}
			if(LineParts[i] == "-") {
				if(i - 1 >= 0 && i + 1 < LineParts.Length) {
					Line = Line.Replace((LineParts[i - 1] + " - " + LineParts[i + 1]).ToString(), (float.Parse(LineParts[i - 1])-float.Parse(LineParts[i + 1])).ToString());
				}
			}
		}
		Line = CompileClass.FixMinusNumbers(Line);
		return float.Parse(Line);
	}

	public static int ReadIntegrer (string Operation) {
		return Mathf.RoundToInt(CompileClass.ReadFloat(Operation));
	}
}
