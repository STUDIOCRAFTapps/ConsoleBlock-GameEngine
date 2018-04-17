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

    int FlowControlResult = 0;
    string FlowControlOutput;

    List<Variable> LocalVariable = new List<Variable>();

	string AllowedChars = "abdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";
    string NumberChars = "0123456789.";
    string UnothorizedChar = "+-/*%&|^!~=<>? {}()[],\t;\n";
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

    //Code encoding - Reading

    // Variable Writing/Reading
    // Function Triggered w/ Params
    // Function returning a value
    // Flow statement (if, for, break, else)

    // Operator Calculation
    // Line ending: ";", "{}", "()"

    // Global Variable are pre-declared in the code
    // Global Variable can be marker as public and allow other consoles to see them as input data

    public IEnumerator OnCompilation () {
        string enc = encscript.Replace("\n", " ").Replace("\t", " ");
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
		for(int i = 0; i < linescript.Length; i++) {
			int FunctionType = -1;
            if(i+1 < linescript.Length) {
                if(linescript[i+1].Contains("{")) {
                    if(KeywordFunction.ToList().Contains(GetIndexTextSeparatedByUnothorizedChar(linescript[i],0))) {
                        GlobalFunctionNames.Add(GetIndexTextSeparatedByUnothorizedChar(linescript[i], 1));
                    }
                }
            }
		}
		IndexRead = 0;

		//ExecuteFunction("Init",new List<Variable>());

        yield return null;
    }

	public IEnumerator OnScriptExecution () {
		IndexRead = 0;
		LocalVariable = new List<Variable>();
		//ExecuteFunction("Update",new List<Variable>());

        yield return null;
	}

    public List<Variable> GetFunctionParametersTemplate (string FunctionName) {
        List<Variable> Result = new List<Variable>();
        int DeclarationLine = -1;

        for(int i = 0; i < linescript.Length; i++) {
            if(FunctionName == linescript[i]) {
                DeclarationLine = 1;
            }
        }

        if(DeclarationLine != -1) {
            //Get parameters, separate the stuff by ',' and space
            string[] Parameters = GetParameters(linescript[DeclarationLine]).Split(',');
            for(int i = 0; i < Parameters.Length; i++) {
                string[] SubParameters = Parameters[i].Split(' ');
                SubParameters.ToList().Remove(string.Empty);
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

        StartCoroutine(ExecuteCodeBlock(StartIndex+1));

        LocalVariable.RemoveRange(LocalVariable.Count - LocalVariableCount - 1, LocalVariableCount);
        yield return null;
    }

	public IEnumerator ExecuteCodeBlock (int StartIndex) {
		int LocalVariableCount = 0;
		int bracketsCount = 0;

        bool IfCommandFailed = false;
        bool IfCommandFailedReset = false;

        for(int i = StartIndex; i < linescript.Length; i++) {
            string FirstText = GetIndexTextSeparatedByUnothorizedChar(linescript[i],0);//GetTextUntilUnothorizedChar(linescript[i]);
            List<Variable> externalVariables = new List<Variable>();
            transmitter.AccessAllInteractableVariables(FirstText, out externalVariables);

            if(linescript[i] == "}" && bracketsCount == 0) {
                break;
            } else if(linescript[i] == "}") {
                bracketsCount--;
            } else if(linescript[i] == "{") {
                bracketsCount++;
            } else if(linescript[i] == ";") {
                continue;
            } else if(bracketsCount == 0) {
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
                            bool result = (bool)SolveOperators(GetParameters(linescript[i]));
                            if(result) {
                                ExecuteCodeBlock(i + 1);
                            } else {
                                IfCommandFailed = true;
                                IfCommandFailedReset = true;
                            }
                        }
                        if(FirstText == "else") {
                            if(GetIndexTextSeparatedByUnothorizedChar(linescript[i], 1) == "if") {
                                bool result = (bool)SolveOperators(GetParameters(linescript[i]));
                                if(result) {
                                    ExecuteCodeBlock(i + 1);
                                } else {
                                    IfCommandFailed = true;
                                    IfCommandFailedReset = true;
                                }
                            } else {
                                if(IfCommandFailed) {
                                    ExecuteCodeBlock(i + 1);
                                }
                            }
                        }
                        if(FirstText == "while") {
                            while(true) {
                                bool result = (bool)SolveOperators(GetParameters(linescript[i]));
                                if(!result) {
                                    break;
                                } else {
                                    ExecuteCodeBlock(i + 1);
                                    if(FlowControlResult != 0) {
                                        if(FlowControlResult == 1) {
                                            break;
                                        }
                                        if(FlowControlResult == 3) {
                                            //TODO: Handle return statement
                                        }
                                        FlowControlResult = 0;
                                    }
                                    yield return null;
                                }
                            }
                        }
                        if(FirstText == "for") {
                            int ForVariableCount = 0;
                            string InitScript = GetParameters(linescript[i]).Split(';')[0];
                            string RepeatScript = GetParameters(linescript[i]).Split(';')[2];
                            string CheckScript = GetParameters(linescript[i]).Split(';')[1];

                            if(KeywordVariable.ToList().Contains(GetIndexTextSeparatedByUnothorizedChar(InitScript,0))) {
                                //Variable creation
                                LocalVariable.Add(new Variable(
                                    GetIndexTextSeparatedByUnothorizedChar(InitScript, 1),
                                    Variable.StringToType(GetIndexTextSeparatedByUnothorizedChar(InitScript, 0)),
                                    Variable.StringToObject(GetIndexTextSeparatedByUnothorizedChar(InitScript, 2), Variable.StringToType(GetIndexTextSeparatedByUnothorizedChar(InitScript, 1)))
                                ));
                                ForVariableCount++;
                            }

                            while(true) {
                                bool result = (bool)SolveOperators(CheckScript);
                                if(!result) {
                                    break;
                                } else {
                                    ExecuteCodeBlock(i + 1);
                                    if(FlowControlResult != 0) {
                                        if(FlowControlResult == 1) {
                                            break;
                                        }
                                        if(FlowControlResult == 3) {
                                            //TODO: Handle return statement
                                        }
                                        FlowControlResult = 0;
                                    }
                                    yield return null;
                                }
                                BasicLineExecution(RepeatScript);
                            }

                            LocalVariable.RemoveRange(LocalVariable.Count-ForVariableCount-1,ForVariableCount);
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
                    List<Variable> FunctionParameters = new List<Variable>();

                    //Handle function parameters
                    FunctionParameters = GetFunctionParametersTemplate(FirstText);
                    for(int p = 0; p < GetParameters(linescript[i]).Split(',').Length; i++) {
                        FunctionParameters[p].source = SolveOperators(GetParameters(linescript[i]).Split(',')[p]);
                    }

                    StartCoroutine(ExecuteFunction(i,FunctionParameters));

                } else if(VariableNameID(GlobalVariable, FirstText) != -1) {
                    //It's a global variable
                    int VariableID = VariableNameID(GlobalVariable, FirstText);

                    if(GetSignsAfterUnothorizedChar(linescript[i]) == "++") {
                        GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
                    } else if(GetSignsAfterUnothorizedChar(linescript[i]) == "--") {
                        GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
                    } else if(linescript[i].Split(' ')[1].StartsWith("=")) {
                        GlobalVariable[VariableID].source = SolveOperators(GetApplyParameters(linescript[i]));
                    } else {
                        GlobalVariable[VariableID].source = SolveOperators(GlobalVariable[VariableID].Id + linescript[i].Split(' ')[1][0] + GetApplyParameters(linescript[i]));
                    }
                } else if(VariableNameID(LocalVariable, FirstText) != -1) {
                    //It's a local variable
                    int VariableID = VariableNameID(LocalVariable, FirstText);

                    if(GetSignsAfterUnothorizedChar(linescript[i]) == "++") {
                        LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source + 1);
                    } else if(GetSignsAfterUnothorizedChar(linescript[i]) == "--") {
                        LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source + 1);
                    } else if(linescript[i].Split(' ')[1].StartsWith("=")) {
                        LocalVariable[VariableID].source = SolveOperators(GetApplyParameters(linescript[i]));
                    } else {
                        LocalVariable[VariableID].source = SolveOperators(LocalVariable[VariableID].Id + linescript[i].Split(' ')[1][0] + GetApplyParameters(linescript[i]));
                    }
                } else if(VariableNameID(externalVariables, FirstText) != -1) {
                    //Get the connected interactable's dictonnairy of variables

                    int VariableID = VariableNameID(externalVariables, FirstText);

                    if(GetSignsAfterUnothorizedChar(linescript[i]) == "++") {
                        externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
                    } else if(GetSignsAfterUnothorizedChar(linescript[i]) == "--") {
                        externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
                    } else if(linescript[i].Split(' ')[1].StartsWith("=")) {
                        externalVariables[VariableID].source = SolveOperators(GetApplyParameters(linescript[i]));
                    } else {
                        externalVariables[VariableID].source = SolveOperators(externalVariables[VariableID].Id + linescript[i].Split(' ')[1][0] + GetApplyParameters(linescript[i]));
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

        LocalVariable.RemoveRange(LocalVariable.Count-LocalVariableCount-1,LocalVariableCount);

        yield return null;
    }

    public void BasicLineExecution (string Line) {
        string FirstText = GetIndexTextSeparatedByUnothorizedChar(Line, 0);
        List<Variable> externalVariables = new List<Variable>();
        transmitter.AccessAllInteractableVariables(FirstText, out externalVariables);

        if(VariableNameID(GlobalVariable, FirstText) != -1) {
            //It's a global variable
            int VariableID = VariableNameID(GlobalVariable, FirstText);

            if(GetSignsAfterUnothorizedChar(Line) == "++") {
                GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
            } else if(GetSignsAfterUnothorizedChar(Line) == "--") {
                GlobalVariable[VariableID].source = ((float)GlobalVariable[VariableID].source + 1);
            } else if(Line.Split(' ')[1].StartsWith("=")) {
                GlobalVariable[VariableID].source = SolveOperators(GetApplyParameters(Line));
            } else {
                GlobalVariable[VariableID].source = SolveOperators(GlobalVariable[VariableID].Id + Line.Split(' ')[1][0] + GetApplyParameters(Line));
            }
        } else if(VariableNameID(LocalVariable, FirstText) != -1) {
            //It's a local variable
            int VariableID = VariableNameID(LocalVariable, FirstText);

            if(GetSignsAfterUnothorizedChar(Line) == "++") {
                LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source + 1);
            } else if(GetSignsAfterUnothorizedChar(Line) == "--") {
                LocalVariable[VariableID].source = ((float)LocalVariable[VariableID].source + 1);
            } else if(Line.Split(' ')[1].StartsWith("=")) {
                LocalVariable[VariableID].source = SolveOperators(GetApplyParameters(Line));
            } else {
                LocalVariable[VariableID].source = SolveOperators(LocalVariable[VariableID].Id + Line.Split(' ')[1][0] + GetApplyParameters(Line));
            }
        } else if(VariableNameID(externalVariables, FirstText) != -1) {
            //Get the connected interactable's dictonnairy of variables

            int VariableID = VariableNameID(externalVariables, FirstText);

            if(GetSignsAfterUnothorizedChar(Line) == "++") {
                externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
            } else if(GetSignsAfterUnothorizedChar(Line) == "--") {
                externalVariables[VariableID].source = ((float)externalVariables[VariableID].source + 1);
            } else if(Line.Split(' ')[1].StartsWith("=")) {
                externalVariables[VariableID].source = SolveOperators(GetApplyParameters(Line));
            } else {
                externalVariables[VariableID].source = SolveOperators(externalVariables[VariableID].Id + Line.Split(' ')[1][0] + GetApplyParameters(Line));
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

    public string GetSignsAfterUnothorizedChar (string Line) {
        string Result = string.Empty;

        for(int i = 0; i < Line.Length; i++) {
            if(Line[i] == ' ') {
                return Result;
            }
            if(!IsAllowedNameChar(Line[i])) {
                Result += Line[i];
            }
        }
        return Result;
    }

    public string GetIndexTextSeparatedByUnothorizedChar (string Line, int Index) {
        string Result = string.Empty;
        string[] subResult = Result.Split(UnothorizedChar.ToCharArray());
        subResult.ToList().Remove(string.Empty);

        return subResult[Index];
    }

    public string GetParameters (string Line) {
        string Result = string.Empty;
        bool ParametersDetected = false;
        int BracketsCount = 0;

        for(int i = 0; i < Line.Length; i++) {
            if(ParametersDetected) {
                Result += Line[i];
            } else if(Line[i] == '(') {
                BracketsCount++;
                ParametersDetected = true;
            } else if(Line[i] == ')') {
                BracketsCount--;
                if(BracketsCount == 0) {
                    break;
                }
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

    object SolveOperators (string Parameters) {
        //TODO: Finish solve operators
        //TODO: Handle NOT operator, bitwise ~ and logical !

        //COMPILE
        List<SolveElement> SolveList = new List<SolveElement>();
        SolveList = SolveCompileLine(Parameters);

        //Find the first chunk (inside the first empty parameter group)
        //Get the first variable duo with an operator in the middle,
        //Solve the operator and replace it with variable
        //Repeat until chunk is just a simple variable.
        //Find an other chunk
        //Repeat until there's no chunk remaining

        int readingIndex = 0;
        int endIndex = SolveList.Count-1;
        List<SolveElement> tempSolveElement;

        while(SolveList.Count > 1) {
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

            SolveElement res = SolveChunk(SolveList.GetRange(readingIndex, endIndex - readingIndex));
            SolveList.RemoveRange(readingIndex, endIndex - readingIndex);
            SolveList.Insert(readingIndex, res);
        }

        if(SolveList[0].type == SolveElementType.v_variable) {
            return SolveList[0].subVariable.source;
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

        while(res.Count > 1) {
            int index = -1;

            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyNot))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_binairyNot));

                res[index+1].
                continue;
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_not))) {
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_not));

                continue;
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_multiply)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_divide)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_modulo))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_multiply));
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_divide)) < index) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_divide));
                }
                if(res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_modulo)) < index) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_modulo));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_plus)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_plus));
                if(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus) < index) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_minus));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_bitshiftRightShift)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_bitshiftLeftShift))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_bitshiftRightShift));
                if(new SolveElement(SolveElementType.v_operator, OperatorType.v_bitshiftLeftShift) < index) {
                    index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_bitshiftLeftShift));
                }
            } else
            if(res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_equal)) ||
                res.Contains(new SolveElement(SolveElementType.v_operator, OperatorType.v_notEqual))) {

                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.v_equal));
                if(new SolveElement(SolveElementType.v_operator, OperatorType.v_notEqual) < index) {
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
                index = res.IndexOf(new SolveElement(SolveElementType.v_operator, OperatorType.or));
            }
        }

        return res[0];
    }

    SolveElement SolveOperation (SubVariable s1, OperatorType o, SubVariable s2) {
        //TODO: Fix solve operation
        return new SolveElement(SolveElementType.v_variable, s1.ApplyOperator(o,s2));
    }

    List<SolveElement> SolveCompileLine (string Line) {
        List<SolveElement> Result = new List<SolveElement>();

        bool StartWordRead = true;
        string CurrentWord = "";
        bool IsNumberDecimal = false;
        int SeekingType = 0;
        for(int i = 0; i < Line.Length; i++) {
            if(StartWordRead) {
                StartWordRead = false;
                if(char.IsDigit(Line[i])) {
                    //Number
                    CurrentWord += Line[i];
                    SeekingType = 0;
                } else if(Line[i] == '(') {
                    Result.Add(new SolveElement(SolveElementType.v_openbrackets));
                    StartWordRead = true;
                } else if(Line[i] == ')') {
                    Result.Add(new SolveElement(SolveElementType.v_closebrackets));
                    StartWordRead = true;
                } else if(char.IsLetter(Line[i])) {
                    //Keyword or variable
                    CurrentWord += Line[i];
                    SeekingType = 1;
                } else if(OperatorChars.Contains(Line[i])) {
                    //Operator
                    CurrentWord += Line[i];
                    SeekingType = 2;
                }
            } else {
                if(SeekingType == 0) {
                    if(NumberChars.Contains(Line[i])) {
                        if(Line[i] == '.') {
                            IsNumberDecimal = true;
                        }
                        CurrentWord += Line[i];
                    } else {
                        Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_int, int.Parse(CurrentWord))));
                        if(IsNumberDecimal) {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_float, float.Parse(CurrentWord))));
                        } else {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_int, int.Parse(CurrentWord))));
                        }

                        i--;
                        StartWordRead = true;
                        CurrentWord = string.Empty;
                        IsNumberDecimal = false;
                    }
                } else if(SeekingType == 1) {
                    if(AllowedChars.Contains(Line[i])) {
                        CurrentWord += Line[i];
                    } else {
                        if(CurrentWord == "false") {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_bool, false)));
                        } else if(CurrentWord == "true") {
                            Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(VariableType.v_bool, true)));
                        } else {
                            //Probably a variable. Check local, global and input
                            List<Variable> externalVariables = new List<Variable>();
                            transmitter.AccessAllInteractableVariables(CurrentWord, out externalVariables);

                            if(GlobalFunctionNames.Contains(CurrentWord)) {
                                //It's a function
                                List<Variable> FunctionParameters = new List<Variable>();

                                //Handle function parameters
                                FunctionParameters = GetFunctionParametersTemplate(CurrentWord);
                                for(int p = 0; p < GetParameters(linescript[i]).Split(',').Length; i++) {
                                    FunctionParameters[p].source = SolveOperators(GetParameters(linescript[i]).Split(',')[p]);
                                }

                                StartCoroutine(ExecuteFunction(i, FunctionParameters));

                                //TODO: Handle return statement

                            } else if(VariableNameID(GlobalVariable, CurrentWord) != -1) {
                                //It's a global variable
                                int VariableID = VariableNameID(GlobalVariable, CurrentWord);

                                Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(GlobalVariable[VariableID].variableType, GlobalVariable[VariableID].source)));
                            } else if(VariableNameID(LocalVariable, CurrentWord) != -1) {
                                //It's a local variable
                                int VariableID = VariableNameID(LocalVariable, CurrentWord);

                                Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(LocalVariable[VariableID].variableType, LocalVariable[VariableID].source)));
                            } else if(VariableNameID(externalVariables, CurrentWord) != -1) {
                                //Get the connected interactable's dictonnairy of variables
                                int VariableID = VariableNameID(externalVariables, CurrentWord);

                                Result.Add(new SolveElement(SolveElementType.v_variable, new SubVariable(externalVariables[VariableID].variableType, externalVariables[VariableID].source)));
                            }
                        }

                        i--;
                        StartWordRead = true;
                        CurrentWord = string.Empty;
                        IsNumberDecimal = false;
                    }
                } else if(SeekingType == 2) {
                    Result.Add(new SolveElement(SolveElementType.v_operator,SolveElement.StringToOperator(CurrentWord)));
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

	override public void OnInteraction () {
		//Call the player code to start the ui-console?
	}
}
