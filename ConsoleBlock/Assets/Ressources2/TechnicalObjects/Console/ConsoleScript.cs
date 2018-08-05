using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Globalization;

public class ConsoleScript : WInteractable {

	public string encscript;
	public string[] linescript;

	int IndexRead = 0;
	int LoopCommands = 0;

    int FlowControlResult = 0;
    string FlowControlOutput;

    List<Variable> LocalVariable = new List<Variable>();

	public string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";
    public string NumberChars = "0123456789.";
    public string UnothorizedChar = "+-/*%&|^!~=<>? {}()[].,\t;\n#";
    public string OperatorChars = ".+-/*%&|^!~<>?=#";
    public string DelimitatorChars = " {}()[],\t;";
    public string LineDelimitatorChars = "{};";
    public string ParametersDelimitatorChars = "()";
	public List<string> GlobalFunctionNames = new List<string>();
    public List<int> GlobalFunctionIndex = new List<int>();

    public List<object> GlobalVariableCompileValue = new List<object>();

    public string[] Keyword = new string[] {
		"void","bool","int","float","string","char",
		"if","else","for","while","break","continue","return"
	};
    public string[] KeywordFunction = new string[] {
        "void","bool","int","float","string","char"
    };
    public string[] KeywordVariable = new string[] {
        "bool","int","float","string","char",
    };
    public string[] KeywordFlow = new string[] {
        "if","else","for","while"
    };
    public string[] KeywordFlowControl = new string[] {
        "break","continue","return"
    };


    //Code encoding - Reading

    // Variable Writing/Reading
    // Function Triggered w/ Params
    // Function returning a value
    // Flow statement (if, for, break, else)

    // Operator Calculation
    // Line ending: ";", "{}", "()"

    // Global Variable are pre-declared in the code
    // Global Variable can be marker as public and allow other consoles to see them as input data

    override public void OnInteraction (Player player) {
        IsInteracting = true;
        player.OpenUI();
        player.uiManager.OpenUI("Console Script Editor", this);
    }

    void Start () {
        ComsumePPS = !InfinitePPSFilling;
        PPSComsumption = 7.0f;

        StartCoroutine(OnCompilation());
    }

    override public void Update () {
        if(CanExecuteCode()) {
            for(int i = 0; i < FunctionCall.Count; i++) {
                FunctionCaller fc = FunctionCall[0];
                FunctionCall.RemoveAt(0);

                if(GlobalFunctionNames.Contains(fc.Name)) {
                    StartCoroutine(ExecuteFunction(fc.Name, fc.parameters));
                }
            }
            StartCoroutine(OnScriptExecution());
        }
    }

    public IEnumerator OnCompilation () {
        for(int i = 0; i < GlobalVariable.Count; i++) {
            if(GlobalVariable[i].source == null) {
                if(GlobalVariable[i].variableType == VariableType.v_bool) {
                    GlobalVariable[i].source = false;
                }
                if(GlobalVariable[i].variableType == VariableType.v_int) {
                    GlobalVariable[i].source = 0;
                }
                if(GlobalVariable[i].variableType == VariableType.v_char) {
                    GlobalVariable[i].source = '\0';
                }
                if(GlobalVariable[i].variableType == VariableType.v_float) {
                    GlobalVariable[i].source = 0.0f;
                }
                if(GlobalVariable[i].variableType == VariableType.v_string) {
                    GlobalVariable[i].source = string.Empty;
                }
            }
        }

        string enc = encscript.Replace("\n", " ").Replace("\t", " ");
		while(enc.Contains("  ")) {
            enc = enc.Replace("  ", " ");
		}
		linescript = enc.Replace(";","@;@").Replace("{","@{@").Replace("}","@}@").Split('@');
		for(int i = 0; i < linescript.Length; i++) {
			if(linescript[i].EndsWith(" ")) {
                linescript[i] = linescript[i].Remove(linescript[i].Length - 1, 1);
			}
			if(linescript[i].StartsWith(" ")) {
                linescript[i] = linescript[i].Remove(0, 1);
			}
        }
        linescript = linescript.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();


        IndexRead = 0;
		GlobalFunctionNames = new List<string>();
        GlobalFunctionIndex = new List<int>();
        int bracketsCount = 0;

        for(int i = 0; i < linescript.Length; i++) {
            if(linescript[i].EndsWith("{")) {
                bracketsCount++;
            }
            if(linescript[i].EndsWith("}")) {
                bracketsCount--;
            }

            if(bracketsCount == 0) {
                int FunctionType = -1;
                if(i + 1 < linescript.Length) {
                    if(linescript[i + 1].EndsWith("{")) {
                        if(KeywordFunction.ToList().Contains(GetIndexTextSeparatedByUnothorizedChar(linescript[i], 0))) {
                            GlobalFunctionNames.Add(GetIndexTextSeparatedByUnothorizedChar(linescript[i], 1));
                            GlobalFunctionIndex.Add(i);
                        }
                    }
                }
            }
		}
		IndexRead = 0;

		StartCoroutine(ExecuteFunction("Init",new List<Variable>()));

        yield return null;

        //TODO: Make sure global functions are sent to the dictionnairy
    }

	public IEnumerator OnScriptExecution () {
        IndexRead = 0;
		LocalVariable = new List<Variable>();
		StartCoroutine(ExecuteFunction("Update",new List<Variable>()));

        yield return null;
	}

    public List<Variable> GetFunctionParametersTemplate (string FunctionName) {
        List<Variable> Result = new List<Variable>();
        int DeclarationLine = -1;

        for(int i = 0; i < linescript.Length; i++) {
            if(FunctionName == GetIndexTextSeparatedByUnothorizedChar(linescript[i],1)) {
                DeclarationLine = i;
            }
        }

        if(DeclarationLine != -1) {
            //Get parameters, separate the stuff by ',' and space
            string[] Parameters = GetParameters(linescript[DeclarationLine]).Split(',');
            for(int i = 0; i < Parameters.Length; i++) {
                if(string.IsNullOrEmpty(GetParameters(linescript[DeclarationLine]))) {
                    break;
                }
                string[] SubParameters = Parameters[i].Split(' ');
                Result.Add(new Variable(SubParameters[1],Variable.StringToType(SubParameters[0]),null,new VariableParameters(false,VariableAccessType.v_readonly)));
            }

            return Result;
        } else {
            return null;
        }
    }

    public IEnumerator ExecuteFunction (int StartIndex, List<Variable> variableParameters) {
        int LocalVariableCount = variableParameters.Count;
        LocalVariable.AddRange(variableParameters);

        StartCoroutine(ExecuteCodeBlock(StartIndex+2, variableParameters));

        LocalVariable.RemoveRange(LocalVariable.Count - LocalVariableCount - 1, LocalVariableCount);
        yield return null;
    }

    public IEnumerator ExecuteFunction (string Name, List<Variable> variableParameters) {
        if(GlobalFunctionIndex.Count <= 0) {
            yield break;
        }
        if(!GlobalFunctionNames.Contains(Name)) {
            yield break;
        }
        int LocalVariableCount = 0;
        
        StartCoroutine(ExecuteCodeBlock(GlobalFunctionIndex[GlobalFunctionNames.IndexOf(Name)] + 2, variableParameters));
        FlowControlResult = 0;

        //TODO: Code block should delete local var too -> for, if, wshile
        yield return null;
    }

    public IEnumerator ExecuteCodeBlock (int StartIndex, List<Variable> temporaryLocalVar) {
        if(temporaryLocalVar != null) {
            LocalVariable.AddRange(temporaryLocalVar);
        }
		int LocalVariableCount = 0;
		int bracketsCount = 0;

        bool IfCommandFailed = false;
        bool IfCommandFailedReset = false;

        for(int i = StartIndex; i < linescript.Length; i++) {
            if(string.IsNullOrEmpty(linescript[i])) {
                continue;
            }
            if(linescript[i] == "}" && bracketsCount == 0) {
                break;
            } else if(linescript[i] == "}") {
                bracketsCount--;
                continue;
            } else if(linescript[i] == "{") {
                bracketsCount++;
                continue;
            } else if(linescript[i] == ";") {
                continue;
            }
            string FirstText = GetIndexTextSeparatedByUnothorizedChar(linescript[i], 0);
            string SecondText = GetIndexTextSeparatedByUnothorizedChar(linescript[i], 1);
            List<Variable> externalVariables = new List<Variable>();
            List<FunctionTemplate> externalFunctions = new List<FunctionTemplate>();
            transmitter.AccessWriteInteractableVariables(FirstText, out externalVariables);
            transmitter.AccessAllInteractableFunction(FirstText, out externalFunctions);
            //TODO: External Functions Calls

            if(bracketsCount == 0) {
                if(Keyword.ToList().Contains(FirstText)) {
                    //It's a keyword!
                    if(KeywordVariable.ToList().Contains(FirstText)) {
                        //Variable creation
                        LocalVariable.Add(new Variable(
                            GetIndexTextSeparatedByUnothorizedChar(linescript[i], 1),
                            Variable.StringToType(FirstText),
                            Variable.StringToObject(GetIndexTextSeparatedByUnothorizedChar(linescript[i], 2), Variable.StringToType(FirstText))
                        ));
                        LocalVariableCount++;
                    } else if(KeywordFlow.ToList().Contains(FirstText)) {
                        //Flow control
                        if(FirstText == "if") {
                            if(SolveOperators(GetParameters(linescript[i])) != null) {
                                bool result = (bool)SolveOperators(GetParameters(linescript[i])).source;
                                if(result) {
                                    StartCoroutine(ExecuteCodeBlock(i + 2, null));
                                } else {
                                    IfCommandFailed = true;
                                    IfCommandFailedReset = true;
                                }
                            }
                        }
                        if(FirstText == "else") {
                            if(GetIndexTextSeparatedByUnothorizedChar(linescript[i], 1) == "if") {
                                bool result = (bool)SolveOperators(GetParameters(linescript[i])).source;
                                if(result) {
                                    StartCoroutine(ExecuteCodeBlock(i + 2, null));
                                } else {
                                    IfCommandFailed = true;
                                    IfCommandFailedReset = true;
                                }
                            } else {
                                if(IfCommandFailed) {
                                    StartCoroutine(ExecuteCodeBlock(i + 2, null));
                                }
                            }
                        }
                        if(FirstText == "while") {
                            while(true) {
                                bool result = (bool)SolveOperators(GetParameters(linescript[i])).source;
                                if(!result) {
                                    break;
                                } else {
                                    StartCoroutine(ExecuteCodeBlock(i + 2, null));
                                    if(FlowControlResult != 0) {
                                        if(FlowControlResult == 1) {
                                            break;
                                        }
                                        if(FlowControlResult == 3) {
                                            yield break;
                                        }
                                        FlowControlResult = 0;
                                    }
                                    yield return null;
                                }
                            }
                        }
                        if(FirstText == "for") {
                            int ForVariableCount = 0;
                            string Parameters = GetParameters(linescript[i]);
                            int firstCutIndex = -1;
                            int secondCutIndex = 0;
                            int BracketsCountLocal = 0;
                            for(int p = 0; p < Parameters.Length; p++) {
                                if(Parameters[p] == '(') {
                                    BracketsCountLocal++;
                                }
                                if(Parameters[p] == ')') {
                                    BracketsCountLocal--;
                                }
                                if(Parameters[p] == ',' && BracketsCountLocal == 0) {
                                    if(firstCutIndex == -1) {
                                        firstCutIndex = p;
                                    } else {
                                        secondCutIndex = p;
                                    }
                                }
                            }
                            string InitScript = GetParameters(linescript[i]).Substring(0, firstCutIndex);
                            string CheckScript = GetParameters(linescript[i]).Substring(firstCutIndex + 1, secondCutIndex - firstCutIndex - 1);
                            string RepeatScript = GetParameters(linescript[i]).Substring(secondCutIndex + 1, Parameters.Length - secondCutIndex - 1);

                            if(KeywordVariable.ToList().Contains(GetIndexTextSeparatedByUnothorizedChar(InitScript,0))) {
                                //Variable creation
                                LocalVariable.Add(new Variable(
                                    GetIndexTextSeparatedByUnothorizedChar(InitScript, 1),
                                    Variable.StringToType(GetIndexTextSeparatedByUnothorizedChar(InitScript, 0)),
                                    Variable.StringToObject(GetIndexTextSeparatedByUnothorizedChar(InitScript, 2), Variable.StringToType(GetIndexTextSeparatedByUnothorizedChar(InitScript, 0)))
                                ));
                                ForVariableCount++;
                            }

                            int ErrorCheck = 999;
                            while(true) {
                                if(ErrorCheck > 0) {
                                    ErrorCheck--;
                                } else {
                                    break;
                                }

                                bool result = (bool)SolveOperators(CheckScript).source;
                                if(!result) {
                                    break;
                                } else {
                                    StartCoroutine(ExecuteCodeBlock(i + 2, null));
                                    if(FlowControlResult != 0) {
                                        if(FlowControlResult == 1) {
                                            break;
                                        }
                                        if(FlowControlResult == 3) {
                                            yield break;
                                        }
                                        FlowControlResult = 0;
                                    }
                                    yield return null;
                                }
                                BasicLineExecution(RepeatScript);
                            }

                            LocalVariable.RemoveRange(LocalVariable.Count-ForVariableCount,ForVariableCount);
                        }
                    } else if(KeywordFlowControl.ToList().Contains(FirstText)) {
                        //Flow actions
                        if(FirstText == "break") {
                            FlowControlResult = 1;
                            yield break;
                        }
                        if(FirstText == "continue") {
                            FlowControlResult = 2;
                            yield break;
                        }
                        if(FirstText == "return") {
                            FlowControlResult = 3;
                            FlowControlOutput = linescript[i].Split(' ')[1];
                            yield break;
                        }
                    }
                } else if(GlobalFunctionNames.Contains(FirstText)) {
                    //It's a function
                    StartCoroutine(ExecuteFunction(FirstText, GetFunctionPreparedParameters(linescript[i], FirstText, GetFunctionParametersTemplate(FirstText))));
                    FlowControlResult = 0;

                } else if(VariableNameID(GlobalVariable, FirstText) != -1) {
                    //It's a global variable
                    int VariableID = VariableNameID(GlobalVariable, FirstText);

                    if(GetSignsAfterUnothorizedChar(linescript[i]) == "++") {
                        GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
                    } else if(GetSignsAfterUnothorizedChar(linescript[i]) == "--") {
                        GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
                    } else if(linescript[i].Split(' ')[1].StartsWith("=")) {
                        GlobalVariable[VariableID].source = SolveOperators(GetApplyParameters(linescript[i])).source;
                    } else {
                        GlobalVariable[VariableID].source = SolveOperators(GlobalVariable[VariableID].Id + linescript[i].Split(' ')[1][0] + GetApplyParameters(linescript[i])).source;
                    }
                } else if(VariableNameID(LocalVariable, FirstText) != -1) {
                    //It's a local variable
                    int VariableID = VariableNameID(LocalVariable, FirstText);

                    if(GetSignsAfterUnothorizedChar(linescript[i]) == "++") {
                        LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source + 1);
                    } else if(GetSignsAfterUnothorizedChar(linescript[i]) == "--") {
                        LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source + 1);
                    } else if(linescript[i].Split(' ')[1].StartsWith("=")) {
                        LocalVariable[VariableID].source = SolveOperators(GetApplyParameters(linescript[i])).source;
                    } else {
                        LocalVariable[VariableID].source = SolveOperators(LocalVariable[VariableID].Id + linescript[i].Split(' ')[1][0] + GetApplyParameters(linescript[i])).source;
                    }
                } else if(VariableNameID(externalVariables, SecondText) != -1) {
                    int VariableID = VariableNameID(externalVariables, SecondText);

                    if(GetSignsAfterUnothorizedChar(linescript[i]) == "++") {
                        externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
                    } else if(GetSignsAfterUnothorizedChar(linescript[i]) == "--") {
                        externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
                    } else if(linescript[i].Split(' ')[1].StartsWith("=")) {
                        externalVariables[VariableID].source = SolveOperators(GetApplyParameters(linescript[i])).source;
                    } else {
                        externalVariables[VariableID].source = SolveOperators(externalVariables[VariableID].Id + linescript[i].Split(' ')[1][0] + GetApplyParameters(linescript[i])).source;
                    }
                    transmitter.ApplyWriteInteractableVariables(SecondText, externalVariables);
                } else if(FunctionTemplatesContains(externalFunctions, SecondText) != -1) {
                    //It's a function
                    List<Variable> FunctionParameters = new List<Variable>();

                    FunctionParameters = GetFunctionPreparedParameters(linescript[i], SecondText, transmitter.AccessSpecificInteractableFunction(FirstText, SecondText).Parameters);
                    if(FunctionParameters != null) {
                        transmitter.SendInteractableFunctionCall(FirstText, new FunctionCaller(SecondText, FunctionParameters));
                        FlowControlResult = 0;
                    }
                }

                //If function;
                // Execute with parameters
                //If Variable;
                // Followed by ++/--
                //  If Number Type, increment/decrement
                // Followed by an operator next to =
                //   Set the variable to: itself (operator) value after the = sign
                // Followed by =
                //   Set the variable to the value after the = sign

                if(IfCommandFailedReset) {
                    IfCommandFailedReset = false;
                } else {
                    IfCommandFailed = false;
                }
            }
		}

        LocalVariable.RemoveRange(LocalVariable.Count-LocalVariableCount,LocalVariableCount);
        if(temporaryLocalVar != null) {
            LocalVariable.RemoveRange(LocalVariable.Count- temporaryLocalVar.Count, temporaryLocalVar.Count);
        }

        yield return null;
    }

    public void BasicLineExecution (string Line) {
        if(string.IsNullOrEmpty(Line)) {
            return;
        }
        string FirstText = GetIndexTextSeparatedByUnothorizedChar(Line, 0);
        string SecondText = GetIndexTextSeparatedByUnothorizedChar(Line, 1);
        List<Variable> externalVariables = new List<Variable>();
        List<FunctionTemplate> externalFunctions = new List<FunctionTemplate>();
        transmitter.AccessWriteInteractableVariables(FirstText, out externalVariables);
        transmitter.AccessAllInteractableFunction(FirstText, out externalFunctions);

        if(VariableNameID(GlobalVariable, FirstText) != -1) {
            //It's a global variable
            int VariableID = VariableNameID(GlobalVariable, FirstText);

            if(GetSignsAfterUnothorizedChar(Line) == "++") {
                GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
            } else if(GetSignsAfterUnothorizedChar(Line) == "--") {
                GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
            } else if(Line.Split(' ')[1].StartsWith("=")) {
                GlobalVariable[VariableID].source = SolveOperators(GetApplyParameters(Line)).source;
            } else {
                GlobalVariable[VariableID].source = SolveOperators(GlobalVariable[VariableID].Id + Line.Split(' ')[1][0] + GetApplyParameters(Line)).source;
            }
        } else if(VariableNameID(LocalVariable, FirstText) != -1) {
            //It's a local variable
            int VariableID = VariableNameID(LocalVariable, FirstText);

            if(GetSignsAfterUnothorizedChar(Line) == "++") {
                if(LocalVariable[VariableID].variableType == VariableType.v_int) {
                    LocalVariable[VariableID].source = ((int)LocalVariable[VariableID].source + 1);
                } else if(LocalVariable[VariableID].variableType == VariableType.v_float) {
                    LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source + 1);
                }
            } else if(GetSignsAfterUnothorizedChar(Line) == "--") {
                if(LocalVariable[VariableID].variableType == VariableType.v_int) {
                    LocalVariable[VariableID].source = ((int)LocalVariable[VariableID].source - 1);
                } else if(LocalVariable[VariableID].variableType == VariableType.v_float) {
                    LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source - 1);
                }
            } else if(Line.Split(' ')[1].StartsWith("=")) {
                LocalVariable[VariableID].source = SolveOperators(GetApplyParameters(Line)).source;
            } else {
                LocalVariable[VariableID].source = SolveOperators(LocalVariable[VariableID].Id + Line.Split(' ')[1][0] + GetApplyParameters(Line)).source;
            }
        } else if(VariableNameID(externalVariables, FirstText) != -1) {
            //Get the connected interactable's dictonnairy of variables

            int VariableID = VariableNameID(externalVariables, FirstText);

            if(GetSignsAfterUnothorizedChar(Line) == "++") {
                externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
            } else if(GetSignsAfterUnothorizedChar(Line) == "--") {
                externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
            } else if(Line.Split(' ')[1].StartsWith("=")) {
                externalVariables[VariableID].source = SolveOperators(GetApplyParameters(Line)).source;
            } else {
                externalVariables[VariableID].source = SolveOperators(externalVariables[VariableID].Id + Line.Split(' ')[1][0] + GetApplyParameters(Line)).source;
            }
            transmitter.ApplyWriteInteractableVariables(FirstText, externalVariables);
        } else if(FunctionTemplatesContains(externalFunctions, SecondText) != -1) {
            List<Variable> FunctionParameters = new List<Variable>();

            FunctionParameters = GetFunctionPreparedParameters(Line, SecondText, transmitter.AccessSpecificInteractableFunction(FirstText, SecondText).Parameters);
            transmitter.SendInteractableFunctionCall(FirstText, new FunctionCaller(SecondText, FunctionParameters));
        }
    }

    public int FunctionTemplatesContains (List<FunctionTemplate> functionTemplates, string Name) {
        for(int i = 0; i < functionTemplates.Count; i++) {
            if(functionTemplates[i].Name == Name) {
                return i;
            }
        }
        return -1;
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

    public string GetSignsAfterUnothorizedChar (string Line) {
        string Result = string.Empty;
        bool HasDetectedOthorizedPart = false;

        for(int i = 0; i < Line.Length; i++) {
            if(!HasDetectedOthorizedPart) {
                if(IsAllowedNameChar(Line[i])) {
                    HasDetectedOthorizedPart = true;
                }
            } else {
                if(Line[i] == ' ') {
                    return Result;
                }
                if(!IsAllowedNameChar(Line[i])) {
                    Result += Line[i];
                }
            }
        }
        return Result;
    }

    public string GetIndexTextSeparatedByUnothorizedChar (string Line, int Index) {
        string[] subResult = Line.Split(UnothorizedChar.ToCharArray());
        subResult = subResult.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        if(Index < subResult.Length) {
            return subResult[Index];
        } else {
            return null;
        }
    }

    public string GetParameters (string Line) {
        string Result = string.Empty;
        bool ParametersDetected = false;
        int BracketsCount = 0;

        for(int i = 0; i < Line.Length; i++) {
            if(Line[i] == '(') {
                BracketsCount++;
                ParametersDetected = true;
                if(BracketsCount == 1) {
                    continue;
                }
            } else if(Line[i] == ')') {
                BracketsCount--;
                if(BracketsCount == 0) {
                    break;
                }
            }
            if(ParametersDetected) {
                Result += Line[i];
            }
        }
        return Result;
    }

    public string GetApplyParameters (string Line) {
        string Result = string.Empty;
        bool ParametersDetected = false;

        for(int i = 0; i < Line.Length; i++) {
            if(ParametersDetected) {
                Result += Line[i];
            } else if(Line[i] == '=') {
                ParametersDetected = true;
            }
        }
        return Result;
    }

    SubVariable SolveOperators (string Parameters) {

        //COMPILE
        List<SolveElement> SolveList = new List<SolveElement>();
        SolveList = SolveCompileLine(Parameters);

        //Find the first chunk (inside the first empty parameter group)
        //Get the first variable duo with an operator in the middle,
        //Solve the operator and replace it with variable
        //Repeat until chunk is just a simple variable.
        //Find an other chunk
        //Repeat until there's no chunk remaining

        List<SolveElement> tempSolveElement;

        int ErrorCheck = 999;
        while(SolveList.Count > 1) {
            int readingIndex = 0;
            int endIndex = SolveList.Count - 1;

            if(ErrorCheck > 0) {
                ErrorCheck--;
            } else {
                break;
            }
            //Searching chunk
            if(SolveList.Contains(new SolveElement(SolveElementType.v_closebrackets)) || SolveList.Contains(new SolveElement(SolveElementType.v_openbrackets))) {
                //Mulitple chunks found, find the first one
                for(int i = 0; i < SolveList.Count; i++) {
                    endIndex = i - 1;
                    if(SolveList[i] == new SolveElement(SolveElementType.v_closebrackets)) {
                        for(int x = i; x >= 0; x--) {
                            readingIndex = x + 1;
                            if(SolveList[i] == new SolveElement(SolveElementType.v_openbrackets)) {
                                break;
                            }
                        }
                        break;
                    }
                }

                tempSolveElement = SolveList.GetRange(readingIndex, endIndex-readingIndex);
            }
            SolveElement res = SolveChunk(SolveList.GetRange(readingIndex, endIndex - readingIndex + 1));
            SolveList.RemoveRange(readingIndex, endIndex - readingIndex + 1);
            SolveList.Insert(readingIndex, res);
        }

        if(SolveList.Count > 0) {
            if(SolveList[0].type == SolveElementType.v_variable) { //TODO: Retunrs empty solve list
                return SolveList[0].subVariable;
            } else {
                return null;
            }
        } else {
            return null;
        }
	}

    SolveElement SolveChunk (List<SolveElement> solveElements) {
        // Not and nots
        // Multi, Divi, Modulo
        // Plus, minus
        // Bitshift
        // Greater Smaller
        // Equal NotEqual
        // Bitwise And
        // Bitwise Nor
        // Bitwise Or
        // Logical And
        // Logical Or

        List<SolveElement> res = solveElements;

        int ErrorCheck = 999;
        while(res.Count > 1) {
            if(ErrorCheck > 0) {
                ErrorCheck--;
            } else {
                break;
            }

            int index = -1;

            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus))) {
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus)) - 1 < 0) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus));
                    SolveElement solveElement = SolveOperation(
                        res[index].operatorType,
                        res[index + 1].subVariable
                    );

                    res[index + 1] = solveElement;
                    res.RemoveAt(index);
                    continue;
                } else {
                    if(res[res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus)) - 1].type != SolveElementType.v_variable) {
                        index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus));
                        SolveElement solveElement = SolveOperation(
                            res[index].operatorType,
                            res[index + 1].subVariable
                        );

                        res[index + 1] = solveElement;
                        res.RemoveAt(index);
                        continue;
                    }
                }
            }

            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_round))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_round));
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyNot))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyNot));
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_not))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_not));
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_multiply)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_divide)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_modulo))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_multiply));
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_divide)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_divide)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_divide));
                }
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_modulo)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_modulo)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_modulo));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_plus)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_plus));
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyRightShift)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyLeftShift))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyRightShift));
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyLeftShift)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyLeftShift)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyLeftShift));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_smaller)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_smallerEqual)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_greater)) || 
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_greaterEqual))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_smaller));
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_smallerEqual)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_smallerEqual)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_smallerEqual));
                }
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_greater)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_greater)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_greater));
                }
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_greaterEqual)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_greaterEqual)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_greaterEqual));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_equal)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_notEqual))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_equal));
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_notEqual)) < index && res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_notEqual)) != -1 || index == -1) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_notEqual));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyAnd))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyAnd));
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyNor))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyNor));
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyOr))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyOr));
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_and))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_and));
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_or))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_or));
            }

            if(index != -1 && res.Count > 1) {

                bool SingularOperation = false;
                if(index - 1 < 0) {
                    SingularOperation = true;
                } else {
                    if(res[index - 1].type != SolveElementType.v_variable) {
                        SingularOperation = true;
                    }
                }

                if(!SingularOperation) {
                    SolveElement solveElement = SolveOperation(
                        res[index - 1].subVariable,
                        res[index].operatorType,
                        res[index + 1].subVariable
                    );

                    res.Insert(index + 2, solveElement);
                    res.RemoveAt(index + 1);
                    res.RemoveAt(index);
                    res.RemoveAt(index - 1);
                } else {
                    SolveElement solveElement = SolveOperation(
                        res[index].operatorType,
                        res[index + 1].subVariable
                    );

                    res[index + 1] = solveElement;
                    res.RemoveAt(index);
                }
            }
        }

        return res[0];
    }

    SolveElement SolveOperation (SubVariable s1, OperatorType o, SubVariable s2) {
        return new SolveElement(SolveElementType.v_variable, s1.ApplyOperator(o,s2));
    }

    SolveElement SolveOperation (OperatorType o, SubVariable s1) {
        return new SolveElement(SolveElementType.v_variable, s1.ApplyOperator(o));
    }

    List<SolveElement> SolveCompileLine (string Line) {
        List<SolveElement> Result = new List<SolveElement>();
        bool StartWordRead = true;
        string CurrentWord = "";
        bool IsNumberDecimal = false;
        int SeekingType = -1;

        for(int i = 0; i <= Line.Length; i++) {
            if(StartWordRead && i < Line.Length) {
                SeekingType = -1;
                StartWordRead = false;
                if(Line[i] == ' ') {
                    StartWordRead = true;
                } else if(NumberChars.Contains(Line[i]) && Line[i] != '.') {
                    //Number
                    CurrentWord += Line[i];
                    SeekingType = 0;
                } else if(Line[i] == '(') {
                    Result.Add(new SolveElement(SolveElementType.v_openbrackets));
                    StartWordRead = true;
                } else if(Line[i] == ')') {
                    Result.Add(new SolveElement(SolveElementType.v_closebrackets));
                    StartWordRead = true;
                } else if(AllowedChars.Contains(Line[i])) {
                    //Keyword or variable
                    CurrentWord += Line[i];
                    SeekingType = 1;
                } else if(OperatorChars.Contains(Line[i])) {
                    //Operator
                    CurrentWord += Line[i];
                    SeekingType = 2;
                } else {
                    StartWordRead = true;
                }
            } else if(i < Line.Length) {
                if(SeekingType == 0) {
                    if(NumberChars.Contains(Line[i])) {
                        if(Line[i] == '.') {
                            IsNumberDecimal = true;
                        }
                        CurrentWord += Line[i];
                    } else {
                        if(IsNumberDecimal) {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_float, float.Parse(CurrentWord, CultureInfo.InvariantCulture))));
                        } else {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_int, int.Parse(CurrentWord))));
                        }

                        if(Line[i] != ' ') {
                            i--;
                        } 
                        StartWordRead = true;
                        CurrentWord = string.Empty;
                        IsNumberDecimal = false;
                    }
                } else if(SeekingType == 1) {
                    if(AllowedChars.Contains(Line[i]) || Line[i] == '.') {
                        CurrentWord += Line[i];
                    } else {
                        if(CurrentWord == "false") {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_bool, false)));
                        } else if(CurrentWord == "true") {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_bool, true)));
                        } else {
                            //Probably a variable. Check local, global and input
                            List<Variable> externalVariables = new List<Variable>();
                            if(CurrentWord.Contains(".")) {
                                transmitter.AccessAllInteractableVariables(CurrentWord.Split('.')[0], out externalVariables);
                            }

                            if(GlobalFunctionNames.Contains(CurrentWord)) {
                                //It's a function
                                List<Variable> FunctionParameters = new List<Variable>();

                                StartCoroutine(ExecuteFunction(i, GetFunctionPreparedParameters(linescript[i], CurrentWord, GetFunctionParametersTemplate(CurrentWord))));

                                SubVariable subVariable = SolveOperators(FlowControlOutput);
                                Result.Add(new SolveElement(SolveElementType.v_variable, subVariable));
                                FlowControlResult = 0;

                            } else if(VariableNameID(GlobalVariable, CurrentWord) != -1) {
                                //It's a global variable
                                int VariableID = VariableNameID(GlobalVariable, CurrentWord);

                                Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(GlobalVariable[VariableID].variableType, GlobalVariable[VariableID].source)));
                            } else if(VariableNameID(LocalVariable, CurrentWord) != -1) {
                                //It's a local variable
                                int VariableID = VariableNameID(LocalVariable, CurrentWord);

                                Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(LocalVariable[VariableID].variableType, LocalVariable[VariableID].source)));
                            } else if(VariableNameID(externalVariables, CurrentWord.Split('.')[1]) != -1) {
                                //Get the connected interactable's dictonnairy of variables
                                int VariableID = VariableNameID(externalVariables, CurrentWord.Split('.')[1]);

                                Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(externalVariables[VariableID].variableType, externalVariables[VariableID].source)));
                            }
                        }

                        if(Line[i] != ' ') {
                            i--;
                        }
                        StartWordRead = true;
                        CurrentWord = string.Empty;
                        IsNumberDecimal = false;
                    }
                } else if(SeekingType == 2) {
                    if(OperatorChars.Contains(Line[i])) {
                        CurrentWord += Line[i];
                    } else {
                        Result.Add(new SolveElement(SolveElementType.v_operator, SolveElement.StringToOperator(CurrentWord)));
                        if(Line[i] != ' ') {
                            i--;
                        }
                        StartWordRead = true;
                        CurrentWord = string.Empty;
                        IsNumberDecimal = false;
                    }
                }
            } else {
                if(SeekingType == 0) {
                    if(IsNumberDecimal) {
                        Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_float, float.Parse(CurrentWord, CultureInfo.InvariantCulture))));
                    } else {
                        Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_int, int.Parse(CurrentWord))));
                    }
                } else if(SeekingType == 1) {
                    if(CurrentWord == "false") {
                        Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_bool, false)));
                    } else if(CurrentWord == "true") {
                        Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_bool, true)));
                    } else {
                        //Probably a variable. Check local, global and input
                        List<Variable> externalVariables = new List<Variable>();
                        if(CurrentWord.Contains(".")) {
                            transmitter.AccessAllInteractableVariables(CurrentWord.Split('.')[0], out externalVariables);
                        }

                        if(GlobalFunctionNames.Contains(CurrentWord)) {
                            //It's a function
                            List<Variable> FunctionParameters = new List<Variable>();

                            //Handle function parameters
                            FunctionParameters = GetFunctionParametersTemplate(CurrentWord);
                            for(int p = 0; p < GetParameters(linescript[i]).Split(',').Length; i++) {
                                FunctionParameters[p].source = SolveOperators(GetParameters(linescript[i]).Split(',')[p]);
                            }

                            StartCoroutine(ExecuteFunction(i, FunctionParameters));

                            SubVariable subVariable = SolveOperators(FlowControlOutput);
                            Result.Add(new SolveElement(SolveElementType.v_variable, subVariable));
                            FlowControlResult = 0;

                        } else if(VariableNameID(GlobalVariable, CurrentWord) != -1) {
                            //It's a global variable
                            int VariableID = VariableNameID(GlobalVariable, CurrentWord);

                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(GlobalVariable[VariableID].variableType, GlobalVariable[VariableID].source)));
                        } else if(VariableNameID(LocalVariable, CurrentWord) != -1) {
                            //It's a local variable
                            int VariableID = VariableNameID(LocalVariable, CurrentWord);

                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(LocalVariable[VariableID].variableType, LocalVariable[VariableID].source)));
                        } else if(VariableNameID(externalVariables, CurrentWord.Split('.')[1]) != -1) {
                            //Get the connected interactable's dictonnairy of variables
                            int VariableID = VariableNameID(externalVariables, CurrentWord.Split('.')[1]);

                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(externalVariables[VariableID].variableType, externalVariables[VariableID].source)));
                        }/* else if(FunctionTemplatesContains(externalFunctions, CurrentWord.Split('.')[1]) != -1) {
                            //It's a function
                            List<Variable> FunctionParameters = new List<Variable>();

                            FunctionParameters = GetFunctionPreparedParameters(linescript[i], CurrentWord.Split('.')[1], transmitter.AccessSpecificInteractableFunction(CurrentWord.Split('.')[0], CurrentWord.Split('.')[1]).Parameters);
                            transmitter.SendInteractableFunctionCall(CurrentWord.Split('.')[0], new FunctionCaller(CurrentWord.Split('.')[1], FunctionParameters));
                        }*/
                    }
                } else if(SeekingType == 2) {
                    Result.Add(new SolveElement(SolveElementType.v_operator, SolveElement.StringToOperator(CurrentWord)));
                }
            }
        }
        return Result;
    }

    int VariableNameID (List<Variable> variables, string Name) {
        for(int i = 0; i < variables.Count; i++) {
            if(variables[i].Id == Name) {
                return i;
            }
        }
        return -1;
    }

    int VariableNameID (List<string> variables, string Name) {
        for(int i = 0; i < variables.Count; i++) {
            if(variables[i] == Name) {
                return i;
            }
        }
        return -1;
    }

    bool IsAllowedNameChar (char Char) {
		return AllowedChars.Contains(Char.ToString());
	}

    List<Variable> GetFunctionPreparedParameters (string Line, string FunctionName, List<Variable> template) {
        List<Variable> FunctionParameters = new List<Variable>();
        List<string> Parameters = new List<string>();
        string ParamsLine = GetParameters(Line);
        string StoringLine = string.Empty;
        int ParamsBCount = 0;

        //Handle function parameters
        FunctionParameters = template; //GetFunctionParametersTemplate(FunctionName)
        for(int p = 0; p < ParamsLine.Length; p++) {
            if(ParamsLine[p] == '(') {
                ParamsBCount++;
            } else if(ParamsLine[p] == ')') {
                ParamsBCount++;
            } else if(ParamsLine[p] == ',') {
                if(ParamsBCount == 0) {
                    Parameters.Add(StoringLine);
                    StoringLine = string.Empty;
                } else {
                    StoringLine += ParamsLine[p];
                }
            } else {
                StoringLine += ParamsLine[p];
            }
        }
        if(Parameters.Count <= 0) {
            if(FunctionParameters.Count > 0) {
                FunctionParameters[0].source = SolveOperators(ParamsLine).source;
            }
        } else {
            for(int p = 0; p < Parameters.Count; p++) {
                FunctionParameters[p].source = SolveOperators(Parameters[p]).source;
            }
        }

        return FunctionParameters;
    }

    List<Variable> GetFunctionPreparedParameters (string Line, string FunctionName, List<VariableTemplate> template) {
        List<Variable> FunctionParameters = new List<Variable>();
        List<string> Parameters = new List<string>();
        string ParamsLine = GetParameters(Line);
        string StoringLine = string.Empty;
        int ParamsBCount = 0;

        //Handle function parameters
        for(int p = 0; p < ParamsLine.Length; p++) {
            if(ParamsLine[p] == '(') {
                ParamsBCount++;
            } else if(ParamsLine[p] == ')') {
                ParamsBCount--;
            } else if(ParamsLine[p] == ',') {
                if(ParamsBCount == 0) {
                    Parameters.Add(StoringLine);
                    StoringLine = string.Empty;
                } else {
                    StoringLine += ParamsLine[p];
                }
            } else {
                StoringLine += ParamsLine[p];
            }
        }
        Parameters.Add(StoringLine);
        StoringLine = string.Empty;
        for(int p = 0; p < Parameters.Count; p++) {
            if(SolveOperators(Parameters[p]) != null) {
                FunctionParameters.Add(new Variable(
                    template[p].Id,
                    template[p].variableType,
                    SolveOperators(Parameters[p]).source
                ));
            } else {
                return null;
            }
        }

        return FunctionParameters;
    }
}