using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsoleScript : WInteractable {

	public string encscript;
	public string[] linescript;

	int IndexRead = 0;
	int LoopCommands = 0;

	List<Variable> LocalVariable = new List<Variable>();

	string AllowedChars = "abdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";
	string OperatorChars = ".+-/*%&|^!~<>?";
	string DelimitatorChars = " {}()[],\t;";
	string LineDelimitatorChars = "{};";
	string ParametersDelimitatorChars = "()";
	List<string> GlobalFunctionNames = new List<string>();

	string[] Keyword = new string[] {
		"void","bool","int","float","string","char",
		"if","else","for","while","break","continue","return"
	};
    string[] KeywordFunction = new string[] {
        "void","bool","int","float","string","char"
    };
    string[] KeywordVariable = new string[] {
        "bool","int","float","string","char",
    };
    string[] KeywordFlow = new string[] {
        "if","else","for","while"
    };
    string[] KeywordFlowControl = new string[] {
        "break","continue","return"
    };

    //TODO: Code encoding - Reading

    // Variable Writing/Reading
    // Function Triggered w/ Params
    // Function returning a value
    // Flow statement (if, for, break, else)

    // Operator Calculation
    // Line ending: ";", "{}", "()"

    // Global Variable are pre-declared in the code
    // Global Variable can be marker as public and allow other consoles to see them as input data

    public void OnCompilation () {
		string enc = encscript.Replace("\t"," ");
		while(enc.Contains("  ")) {
			enc.Replace("  ", " ");
		}
		linescript = enc.Replace(";","@;@").Replace("{","@{@").Replace("}","@}@").Split('@');
		for(int i = 0; i < linescript.Length; i++) {
			if(linescript[i].EndsWith(" ")) {
				linescript[i].Remove(linescript[i].Length - 1);
			}
			if(linescript[i].StartsWith(" ")) {
				linescript[i].Remove(0);
			}
		}

		IndexRead = 0;
		GlobalFunctionNames = new List<string>();
		for(; IndexRead < linescript.Length; IndexRead++) {
			if(linescript[IndexRead].IndexOfAny(LineDelimitatorChars.ToCharArray()) == -1) {
				continue;
			}
			int FunctionType = -1;
			string GlobalFunctionName = FlowGetFunctionName(out FunctionType);
			if(FunctionType != -1) {
				if(GlobalFunctionName != null) {
					GlobalFunctionNames.Add(GlobalFunctionName);
				}
			}
		}
		IndexRead = 0;

		ExecuteFunction("Start",new List<Variable>());
	}

	public void OnScriptExecution () {
		IndexRead = 0;
		LocalVariable = new List<Variable>();
		ExecuteFunction("Update",new List<Variable>());
	}

	public void ExecuteCodeBlock (int StartIndex) {
		int LocalVariableCount = 0;
		int bracketsCount = 0;

		for(int i = StartIndex; i < linescript.Length; i++) {
            string FirstText = GetTextUntilUnothorizedChar(linescript[i]);
            List<Variable> externalVariables = new List<Variable>();
            transmitter.AccessAllInteractableVariables(FirstText, out externalVariables);

            if(linescript[i] == "}" && bracketsCount == 0) {
				break;
			} else if(linescript[i] == "}") {
				bracketsCount--;
			} else if(linescript[i] == "{") {
				bracketsCount++;
			} else if(bracketsCount == 0) {
				if(Keyword.ToList().Contains(FirstText)) {
                    //It's a keyword!
                    if(KeywordVariable.ToList().Contains(FirstText)) {
                        //Variable creation
                    } else if(KeywordFlow.ToList().Contains(FirstText)) {
                        //Flow control
                    } else if(KeywordFlowControl.ToList().Contains(FirstText)) {
                        //Flow actions
                    }
                } else if(GlobalFunctionNames.Contains(FirstText)) {
					//It's a function
				} else if(VariableNameID(GlobalVariable, FirstText) != -1) {
					//It's a global variable
				} else if(VariableNameID(LocalVariable, FirstText) != -1) {
					//It's a local variable
				} else if(VariableNameID(externalVariables, FirstText) != -1) {
                    //Get the connected interactable's dictonnairy of variables
                }
			}
		}
	}

    public string GetTextUntilUnothorizedChar (string Line) {
        string Result = string.Empty;
        for(int i = 0; i < Line.Length; i++) {
            if(IsAllowedNameChar(Line[i])) {
                Result += Line[i];
            } else {
                break;
            }
        }
        return Result;
    }

	public void ExecuteFunction (string Name, List<Variable> lvariables) {
		IndexRead.Add(0);
		bool MustReturnObject = false;
		VariableType ObjectType;

		int LocalVariableCount = lvariables.Count;
		LocalVariable.AddRange(lvariables);

		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			int FunctionType = -1;
			if(FlowGetFunctionName(out FunctionType) != null) {
				break;
			} else {
				FlowSkipBrackets();
			}
			if(FunctionType > 0) {
				MustReturnObject = true;
				ObjectType = (VariableType)(FunctionType - 1);
			}
        }

		if(IndexRead[CurrentIndexLayer] < encscript.Length) {
			return;
		} else {
			FlowGotoBrackets();
		}

		//read function line by line, until it's the end of the function or a return statement is called
		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			string line = FlowGetTextUntilEnding();
			string[] subline = line.Split(DelimitatorChars.ToCharArray());
			if(subline[0] == "return") {

			} else if(/*IsDone*/) {
				
			}

			ExecuteLine();
		}

		IndexRead.RemoveAt(CurrentIndexLayer);
		CurrentIndexLayer--;

		LocalVariable.RemoveRange(LocalVariable.Count-LocalVariableCount-1,LocalVariableCount);
	}

	string ExecuteLine (string l = "") {
		string line;
		if(!string.IsNullOrEmpty(l)) {
			line = l;
			FlowGotoToLine();
		} else {
			line = FlowGetTextUntilEnding();
		}
		FlowReturnToLine();
		//Remove all empty element
		string[] subline = line.Split(DelimitatorChars.ToCharArray());
		subline.ToList().Remove(string.Empty);

		//Statement as first mention:
		// IF, ELSE
		// FOR, WHILE
		// BREAK, CONTINUE, RETURN

		//Variable as first mention:
		// FUNCTIONS:
		//  FunctionName(Parameters)
		// VARIABLE:
		//  = ASSIGN
		//  += ASSIGN WITH AN OPERATOR

		//TODO:
		// Code memory; else

		if(subline.Length > 0) {
			if(subline[0] == "if") {
				FlowReturnToLine();
				string parameters = FlowGetParametersUntilEnding(line);

				bool result = (bool)SolveOperators(parameters);
			} else if(subline[0] == "else") {

			} else if(subline[0] == "for") {
				LoopCommands = 0;
				int ExecutionLayer = 0;

				FlowReturnToLine();
				string[] parameters = FlowGetParametersUntilEnding(line).Split(';');
				int LocalVariableCount = 0;
				if(VariableNames.ToList().Contains(parameters[0].Split(DelimitatorChars.ToCharArray())[0])) {
					LocalVariableCount++;
					LocalVariable.Add(new Variable(
						parameters[0].Split(DelimitatorChars.ToCharArray())[1],
						DictonairyElement.VariableNameToType(parameters[0].Split(DelimitatorChars.ToCharArray())[0]),
						Variable.StringToObject(parameters[0].Split(DelimitatorChars.ToCharArray())[3],DictonairyElement.VariableNameToType(parameters[0].Split(DelimitatorChars.ToCharArray())[0]))
					));
				} else {
					//ERROR
					return;
				}


				for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
					if(parameters[1] != string.Empty) {
						bool r = (bool)SolveOperators(parameters[1]);
						if(!r) {
							break;
						}
					} else {
						//ERROR
						return null;
					}

					string result = ExecuteLine();
					result = result.Replace(' ', '').Replace('\t','');

					if(LoopCommands == 0) {
						FlowSkipBrackets();
						break;
					}
					if(LoopCommands == 1) {
						FlowReturnToBrackets();
						continue;
					}
					if(result.StartsWith("{")) {
						ExecutionLayer++;
					}
					if(result.StartsWith("}")) {
						if(ExecutionLayer == 0) {
							FlowSkipBrackets();
							return null;
						} else {
							ExecutionLayer--;
						}
					}

					ExecuteLine(parameters[2]);
				}

				for(int i = 0; i < LocalVariableCount; i++) {
					LocalVariable.RemoveAt(LocalVariable.Count-1);
				}
			} else if(subline[0] == "while") {
				LoopCommands = 0;
				int ExecutionLayer = 0;
				for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
					while(true) {
						string parameters = FlowGetParametersUntilEnding(line);
						if(parameters != string.Empty) {
							bool r = (bool)SolveOperators(parameters);
							if(!r) {
								break;
							}
						} else {
							//ERROR
							return null;
						}

						string result = ExecuteLine();

						if(LoopCommands == 0) {
							FlowSkipBrackets();
							break;
						}
						if(LoopCommands == 1) {
							FlowReturnToBrackets();
							continue;
						}
						if(result.StartsWith("{")) {
							ExecutionLayer++;
						}
						if(result.StartsWith("}")) {
							if(ExecutionLayer == 0) {
								FlowSkipBrackets();
								return null;
							} else {
								ExecutionLayer--;
							}
						}
					}
				}
			} else if(subline[0] == "break") {
				LoopCommands = 1;
			} else if(subline[0] == "continue") {
				LoopCommands = 2;
			} else if(GlobalFunctionNames.Contains(subline[0])) {
				ExecuteFunction(subline[0],/*Parameters*/);
			} else if(VariableNameID(GlobalVariable,subline[0]) != -1) {
				int vID = VariableNameID(GlobalVariable,subline[0]);
				//Execute stuff
			} else if(VariableNameID(LocalVariable,subline[0]) != -1) {
				int vID = VariableNameID(LocalVariable,subline[0]);
				//Execute stuff
			} else if(/*OutputVariable(Assign)*/) {

			} else if(/*OutputVariable(Trigger)*/) {

			} else if(/*DirectConsoleAcces*/) {
				if(/*GlobalExternalFunction*/) {

				} else if(/*GlobalExternalVariable*/) {

				}
			}
		}

		//Goto next line (index)

		return line;
	}

	object SolveOperators (string Parameters) {
		
	}

	int VariableNameID (List<Variable> variables, string Name) {
		for(int i = 0; i < variables.Count; i++) {
			if(variables[i].Id == Name) {
				return i;
			}
		}
		return -1;
	}

	string FlowGetFunctionName (out int FunctionType) {
		int f = Array.IndexOf(FunctionNames,FlowGetText());
		FunctionType = f;
		if(f != -1) {
			return FlowGetText();
		} else {
			return string.Empty;
		}
	}

	void FlowGotoBrackets () {
		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			if(encscript[IndexRead[CurrentIndexLayer]] == '{') {
				IndexRead[CurrentIndexLayer]++;
				break;
			}
		}
	}

	void FlowReturnToBrackets () {
		for(; IndexRead[CurrentIndexLayer] >= 0; IndexRead[CurrentIndexLayer]--) {
			if(encscript[IndexRead[CurrentIndexLayer]] == '{') {
				IndexRead[CurrentIndexLayer]++;
				break;
			}
		}
	}

	void FlowReturnToLine () {
		for(; IndexRead[CurrentIndexLayer] >= 0; IndexRead[CurrentIndexLayer]--) {
			int f = Array.IndexOf(LineDelimitatorChars.ToCharArray(),encscript[IndexRead[CurrentIndexLayer]]);
			if(encscript[IndexRead[CurrentIndexLayer]] != -1) {
				IndexRead[CurrentIndexLayer]++;
				break;
			}
		}
	}

	void FlowGotoToLine () {
		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			int f = Array.IndexOf(LineDelimitatorChars.ToCharArray(),encscript[IndexRead[CurrentIndexLayer]]);
			if(encscript[IndexRead[CurrentIndexLayer]] != -1) {
				IndexRead[CurrentIndexLayer]++;
				break;
			}
		}
	}

	string FlowGetTextUntilEnding () {
		string Text = "";
		bool firstallowedchardetected = false;

		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			int f = Array.IndexOf(LineDelimitatorChars.ToCharArray(),encscript[IndexRead[CurrentIndexLayer]]);
			if(f != -1) {
				firstallowedchardetected = true;
				Text += encscript[IndexRead[CurrentIndexLayer]];
			} else {
				if(firstallowedchardetected) {
					break;
				}
			}
		}
		return Text;
	}

	string FlowGetParametersUntilEnding (string line) {
		string Text = "";
		int bracketsCount = 0;
		bool firstallowedchardetected = false;

		for(int i = 0; i < line.Length; i++) {
			if(encscript[IndexRead[CurrentIndexLayer]] == ')') {
				if(bracketsCount == 0 && firstallowedchardetected) {
					break;
				} else {
					bracketsCount--;
				}
			}
			if(encscript[IndexRead[CurrentIndexLayer]] == '(') {
				firstallowedchardetected = true;
				bracketsCount++;
			}
			if(firstallowedchardetected) {
				Text += line[i];
			}
		}

		return Text;
	}

	void FlowSkipBrackets () {
		int bracketsCount = 0;

		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			if(encscript[IndexRead[CurrentIndexLayer]] == '}') {
				if(bracketsCount == 0) {
					break;
				} else {
					bracketsCount--;
				}
			}
			if(encscript[IndexRead[CurrentIndexLayer]] == '{') {
				bracketsCount++;
			}
		}
	}

	void FlowSkipToName () {
		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			if(IsAllowedNameChar(encscript[IndexRead[CurrentIndexLayer]])) {
				break;
			}
		}
	}

	string FlowGetText () {
		string Text = "";
		bool firstallowedchardetected = false;

		for(; IndexRead[CurrentIndexLayer] < encscript.Length; IndexRead[CurrentIndexLayer]++) {
			if(IsAllowedNameChar(encscript[IndexRead[CurrentIndexLayer]])) {
				firstallowedchardetected = true;
				Text += encscript[IndexRead[CurrentIndexLayer]];
			} else {
				if(firstallowedchardetected) {
					break;
				}
			}
		}

		return Text;
	}

	bool IsAllowedNameChar (char Char) {
		return AllowedChars.Contains(Char.ToString());
	}

	override public void OnInteraction () {
		//Call the player code to start the ui-console
	}
}
