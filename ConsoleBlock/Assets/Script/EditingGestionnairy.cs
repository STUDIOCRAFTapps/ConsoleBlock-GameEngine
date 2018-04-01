using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditingGestionnairy : MonoBehaviour {

	bool AfterUpdating = false;

	public List<GameObject> obj;
	public List<GameObject> temporary;
	int SavingCooldown = 340;

	public string CompiledScript = "";
	public string[] CompScriptLines;

	public Spawner spawner;

	public string LoadValue;
	public bool PrepareLoading = false;
	int CountdownBeforeLoad = 1;
	public bool Initialized = false;

	bool Ask = false;
	string AskString = "";

	RectTransform rt;
	int count = 0;
	int spacing = 0;

	void Start () {
		rt = GetComponent<RectTransform>();
		bool Check = false;
	}

	public void Load (string LV) {
		if(!Ask) {
			Ask = true;
			AskString = LV;
			return;
		}
		
		int OriginalCC = transform.childCount;
		for(int i = 0; i < OriginalCC; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}
		obj = new List<GameObject>();


		//Replace();

		int Decale = 0;

		string[] LVLines = LV.Split('\n');
		//Debug.Log("==Staring the decripting==");
		for(int i = 0; i < LVLines.Length; i++) {
			//Debug.Log("Executing Line #" + i);
			string CurrentLine = LVLines[i];
			int TabCount = CurrentLine.Split('\t').Length-1;
			CurrentLine = CurrentLine.Replace("\t","").Replace("\n","");
			//Debug.Log("=Current Line: " + CurrentLine+"=");
			bool IsResizable = CurrentLine.EndsWith("{");
			if(CurrentLine.EndsWith(";")) {
				CurrentLine = CurrentLine.Remove(CurrentLine.Length-1);
			}
			if(CurrentLine.EndsWith(" {")) {
				CurrentLine = CurrentLine.Remove(CurrentLine.Length-2);
			} else if(CurrentLine.EndsWith("{")) {
				CurrentLine = CurrentLine.Remove(CurrentLine.Length-1);
			} else if(CurrentLine.EndsWith("}")) {
				Decale++;
			}


			string[] Parts;
			int[] LinesSteps;

			int LineSystem = 0;

			//Debug.Log("=Current Line Information:=");
			//Debug.Log("Value: " + CurrentLine);
			//Debug.Log("IsResizable: " + IsResizable);
			//Debug.Log("==Starting Analizing Process==");
			for(int o = 0; o < spawner.objects.Length; o++) {
				//Debug.Log("Object #" + o);
				bool OkyLine = false;
				Parts = new string[2];
				Parts[0] = spawner.objects[o].GetComponent<Drag>().StartSyntax;
				Parts[1] = spawner.objects[o].GetComponent<Drag>().EndSyntax;
				//Parts = spawner.transform.GetChild(o).GetComponent<Drag>().DataSyntax.Split('@'); //Switch
				LinesSteps = new int[Parts.Length];
				//Test if the current line countains a similar structure, but @ are replace with something (In the good order). If yes, stop the loop and keep the parts to with what is where the @ are in the syntax
				//If nothing is found, skip to the next line, mmmmkay?

				bool SkipOLoop = false;
				//Also test if CurrentLine does comport strings that the object doesn't want to.
				//Debug.Log("Name of the Object: " + spawner.objects[o].name);
				if(IsResizable.ToString() == spawner.objects[o].GetComponent<Drag>().IsResizableBlock.ToString()) {
					//Debug.Log("=TESTING: The object has the same block type=");
					for(int n = 0; n < Parts.Length; n++) {
						//if(CurrentLine.Contains(Parts[n])) { //Switch
						//	LinesSteps[n] = CurrentLine.LastIndexOf(Parts[n]);
						//} else {
						//	//if it's not there, skip the 'o' loop because it require all the parts.
						//	OkyLine = true;
						//	SkipOLoop = true;
						//	break;
						//}
						if(n == 0) {
							if(CurrentLine.StartsWith(Parts[0])) {
								//Debug.Log("The first step has been achive");
								LinesSteps[n] = CurrentLine.LastIndexOf(Parts[n]);
							} else {
								//Debug.Log("The first step has been rejected");
								SkipOLoop = true;
								break;
							}
						}
						if(n == 1) {
							if(CurrentLine.EndsWith(Parts[1])) {
								//Debug.Log("The second step has been achive");
								LinesSteps[n] = CurrentLine.LastIndexOf(Parts[n]);
							} else {
								//Debug.Log("The second step has been rejected");
								SkipOLoop = true;
								break;
							}
						}
					}
				} else {
					//Debug.Log("=TESTING: The object does not comport the same block type=");
					SkipOLoop = true;
				}
				if(!SkipOLoop) {
					//Debug.Log("The object comparing system approve this object");
					//All value in LinesSteps must be in an assending order.
					int Verify = -1;
					bool EverythingIsOky = true;
					for(int ls = 0; ls < LinesSteps.Length; ls++) {
						if(LinesSteps[ls] <= Verify) {
							Verify = LinesSteps[ls];
							EverythingIsOky = false;
						}
					}
					for(int c = 0; c < spawner.objects[o].GetComponent<Drag>().DoesNotContains.Length; c++) {
						if(CurrentLine.Contains(spawner.objects[o].GetComponent<Drag>().DoesNotContains[c])) {
							EverythingIsOky = false;
						}
					}
					if(EverythingIsOky) {
						//Debug.Log("Object Fully Approved.");
						//We have a winner! Do the normal thing and change line oky?
						List<string> DataTags = new List<string>();

						int P1,P2,P3 = 0;
						string[] SyntaxParts = spawner.objects[o].GetComponent<Drag>().DataSyntax.Split('@');

						for(int dtc = 0; dtc < SyntaxParts.Length-1; dtc++) {
							P1 = CurrentLine.IndexOf(SyntaxParts[dtc]);
							P2 = P1+SyntaxParts[dtc].Length;
							//Debug.Log("No"+dtc+" : " + CurrentLine + " in " + SyntaxParts[dtc+1]);
							if(dtc == SyntaxParts.Length-2) {
								P3 = CurrentLine.LastIndexOf(SyntaxParts[dtc+1]);
							} else {
								P3 = CurrentLine.IndexOf(SyntaxParts[dtc+1]);
							}
							//Debug.Log("Testing if \"" + SyntaxParts[dtc+1] + "\" is in \"" + CurrentLine + "\" returns " + CurrentLine.Contains(SyntaxParts[dtc+1]));
							DataTags.Add(CurrentLine.Substring(P2, P3-P2));
							CurrentLine = CurrentLine.Remove(P1, P3-P1);
						}

						OkyLine = true;
						CreateObjectAt(o,i+Decale,DataTags.ToArray());

						//Debug.Log(CurrentLine + " looks like the syntax " + spawner.objects[o].GetComponent<Drag>().DataSyntax);
					}
				}
				if(OkyLine == true) {
					//Change Line
					OkyLine = false;
					break;
				}
				if(i+1<LVLines.Length) {
					LineSystem++;
				}
				LineSystem++;
			}

			//Debug.Log("*Changing Line*");
		}
		Ask = false;
	}

	public GameObject CreateObjectAt (int BlockIndex, int LineIndex, string[] DataTags) {
		//SetDataValue
		GameObject Clone = spawner.GetClone(BlockIndex, DataTags);
		InsertAt(LineIndex, Clone);
		RedistributeReplaceAction();

		return Clone;
		//Clone.GetComponent<Drag>().SearchParent();
	}

	public void Save () {
		CompiledScript = "";
		int steps = 0;

		for(int i = 0; i < obj.Count; i++) {
			if(obj.ToArray()[i].GetComponent<Resize>()) {
				string DS = obj.ToArray()[i].GetComponent<Drag>().DataSyntax;
				int c = 0;
				while(DS.Contains("@")) {
					DS = StringExtension.ReplaceFirstOccurrance(DS, "@", obj.ToArray()[i].GetComponent<Drag>().strsources[c]);
					c++;
				}
				CompiledScript+=DS+" {\n";
				steps++;
				if(obj.ToArray()[i].GetComponent<Drag>().childs.Count > 0) {
					SearchAndEncode(steps, obj.ToArray()[i].GetComponent<Drag>().childs);
				} else {
					CompiledScript+=GetStringWith("\t", steps)+"\n";
				}
				steps--;
				CompiledScript+="}";
			} else {
				string DS = obj.ToArray()[i].GetComponent<Drag>().DataSyntax;
				int c = 0;
				while(DS.Contains("@")) {
					DS = StringExtension.ReplaceFirstOccurrance(DS, "@", obj.ToArray()[i].GetComponent<Drag>().strsources[c]);
					c++;
				}
				CompiledScript+=DS+";";
			}
			if(i+1 < obj.Count) {
				CompiledScript+="\n";
			}
		}
		//CompiledScript = CompiledScript.Substring(0, CompiledScript.Length-3); //Remove the \n at the end
		CompScriptLines = CompiledScript.Split('\n');
	}

	void SearchAndEncode (int stepsV, List<GameObject> childsV) {
		for(int i = 0; i < childsV.Count; i++) {
			if(childsV.ToArray()[i].GetComponent<Resize>()) {
				string DS = childsV.ToArray()[i].GetComponent<Drag>().DataSyntax;
				int c = 0;
				while(DS.Contains("@")) {
					DS = StringExtension.ReplaceFirstOccurrance(DS, "@", childsV.ToArray()[i].GetComponent<Drag>().strsources[c]);
					c++;
				}
				CompiledScript+=GetStringWith("\t", stepsV)+DS+" {\n";
				stepsV++;
				if(childsV.ToArray()[i].GetComponent<Drag>().childs.Count > 0) {
					SearchAndEncode(stepsV, childsV.ToArray()[i].GetComponent<Drag>().childs);
				} else {
					CompiledScript+=GetStringWith("\t", stepsV)+"\n";
				}
				stepsV--;
				CompiledScript+=GetStringWith("\t", stepsV)+"}";
			} else {
				string DS = childsV.ToArray()[i].GetComponent<Drag>().DataSyntax;
				int c = 0;
				while(DS.Contains("@")) {
					DS = StringExtension.ReplaceFirstOccurrance(DS, "@", childsV.ToArray()[i].GetComponent<Drag>().strsources[c]);
					c++;
				}
				CompiledScript+=GetStringWith("\t", stepsV)+DS+";";
			}
			if(i < childsV.Count) {
				CompiledScript+="\n";
			}
		}
	}

	string GetStringWith (string value, int count) {
		string V = "";
		for(int i = 0; i < count; i++) {
			V+=value;
		}
		return V;
	}

	void Update () {
		if(Ask) {
			Load(AskString);
		}

		if(PrepareLoading && CountdownBeforeLoad == 0 && !Initialized) {
			PrepareLoading = false;
			Initialized = true;
			Load(LoadValue);
		}
		if(PrepareLoading && CountdownBeforeLoad != 0 && !Initialized) {
			CountdownBeforeLoad--;
		}

		if(SavingCooldown == 0) {
			SavingCooldown = 340;
			Save();
		} else {
			SavingCooldown--;
		}

		count = 1;
		spacing = 2;

		for(int i = 0; i < obj.Count; i++) {
			if(obj[i] != null) {
				if(obj[i].GetComponent<Resize>()) {
					count += (obj[i].GetComponent<Resize>().Steps+1);
					spacing += obj[i].GetComponent<Resize>().Steps+1;
				} else {
					count++;
					spacing++;
				}
			} else {
				count++;
				spacing++;
			}
		}
		rt.sizeDelta = new Vector2(75, count*22+(spacing+1)*5);
		/*if(AfterUpdating) {
			RedistributeReplaceAction();
			AfterUpdating = false;
		}*/
	}

	public float RoundByBounds (float Value, float Bound) {
		return Mathf.RoundToInt(Value / Bound) * Bound;
	}

	public void InsertAt (int Order, GameObject GameObj) {
		GameObj.GetComponent<Drag>().order = Order;
		//Debug.Log("Inserting " + GameObj + " at " + Order);
		//TODO: When inserting/removing a block inside a resizable block, the block after the resizable block won't change pos most of the time
		int SkipsPoint = 0;
		temporary = obj;
		int StartPoint = 0;
		int V = 0;
		Drag res = null;
		bool Skip = false;
		bool Reset = false;

		int AdvencementPoints = 0;

		if(temporary.Count == 0) {
			Skip = true;
		}
		for(int i = 0; !Skip; i++) {
			//Debug.Log("I is " + i);
			if(Reset && 0 < temporary.Count) {
				//Debug.Log("I is 0");
				i = 0;
				Reset = false;
			} else if(Reset && 0 >= temporary.Count) {
				//Debug.Log("Out of sequence");
				V = 0;
				break;
			}
			//First, check to see if the block is inside a resizable block (Or inside a resizable that is inside a resizable block...)
			//Debug.Log("Is " + temporary.ToArray()[i].name + " a resizable block? " + temporary.ToArray()[i].GetComponent<Resize>());
			if(temporary.ToArray()[i].GetComponent<Resize>()) { //order was order-startpoint
				//Debug.Log("Checking if " + (Order-StartPoint) + " is > than " + (AdvencementPoints-1) + " and < than " + (AdvencementPoints+temporary.ToArray()[i].GetComponent<Resize>().Steps));
				if(Order > AdvencementPoints-1/* + i*/ && Order < AdvencementPoints+temporary.ToArray()[i].GetComponent<Resize>().Steps) {
					AdvencementPoints++;
					//Debug.Log("What is temporairy?");
					//Debug.Log("res: " + temporary.ToArray()[i].GetComponent<Drag>().strsources[0] + ", temporary: " + temporary.ToArray()[i].GetComponent<Drag>().childs.Count);
					res = temporary.ToArray()[i].GetComponent<Drag>();
					temporary = temporary.ToArray()[i].GetComponent<Drag>().childs;
					V = i;
					//Debug.Log("Adding " + (i+1) + " to the StartPoint");
					StartPoint += i/*+1*/;
					Reset = true;
					//Debug.Log("Iteration: " + i);
				} else {
					AdvencementPoints += temporary.ToArray()[i].GetComponent<Resize>().Steps+1;
				}
			} else {
				AdvencementPoints++;
			}
			//StartPoint += i+1;
			if((i+1 >= temporary.Count) && !Reset) {
				//Debug.Log("Breaking");
				break;
			}
		}
		if(res != null) {
			res.AddChild(GameObj, Mathf.Clamp((Order-(StartPoint)),0,res.GetComponent<Resize>().Steps-2));

			//res is GOOD!

			//res.Replace();
			return;
		}

		obj.Insert(Mathf.Clamp(Order, 0, obj.Count), GameObj);
		for(int i = 0; i < obj.Count; i++) {
			obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-((i+SkipsPoint)*27),0);
			if(obj.ToArray()[i].GetComponent<Resize>()) {
				SkipsPoint+=obj.ToArray()[i].GetComponent<Resize>().Steps;
			}
		}
		RedistributeReplaceAction();
	}

	public void Remove (GameObject GameObj) {
		int SkipsPoint = 0;
		obj.Remove(GameObj);
		for(int i = 0; i < obj.Count; i++) {
			obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-((i+SkipsPoint)*27),0);
			if(obj.ToArray()[i].GetComponent<Resize>()) {
				SkipsPoint+=obj.ToArray()[i].GetComponent<Resize>().Steps;
			}
		}
		RedistributeReplaceAction();
	}

	public void Replace () {
		int SkipsPoint = 0;
		for(int i = 0; i < obj.Count; i++) {
			obj.ToArray()[i].GetComponent<RectTransform>().localPosition = new Vector3(GetComponent<RectTransform>().sizeDelta.x/2+6+(24*obj.ToArray()[i].GetComponent<Drag>().Aligment),-20-((i+SkipsPoint)*27),0);
			obj.ToArray()[i].GetComponent<Drag>().order = Mathf.RoundToInt(1-RoundByBounds((obj.ToArray()[i].GetComponent<RectTransform>().localPosition.y)/27,1)-2);
			if(obj.ToArray()[i].GetComponent<Resize>()) {
				SkipsPoint+=obj.ToArray()[i].GetComponent<Resize>().Steps;
			}
		}
	}

	public void RedistributeReplaceAction () {
		AfterUpdating = true;
		Replace();
		foreach(GameObject gameObj in obj) {
			gameObj.GetComponent<Drag>().RedistributeReplaceAction();
		}
	}
}

public static class StringExtension
{

	public static string ReplaceFirstOccurrance(this string original, string oldValue, string newValue)
	{
		if (string.IsNullOrEmpty(original))
			return string.Empty;
		if (string.IsNullOrEmpty(oldValue))
			return original;
		if (string.IsNullOrEmpty(newValue))
			newValue = string.Empty;
		int loc = original.IndexOf(oldValue);
		return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
	}
}
