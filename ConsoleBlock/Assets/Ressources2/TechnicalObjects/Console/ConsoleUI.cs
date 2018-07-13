using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUI : DefaultUI {

    string Script = string.Empty;
    public TMPro.TMP_InputField scriptInputField;
    public InputField nameInputField;

    public RectTransform FunctionExecuterView;
    public RectTransform FunctionExecuterTemplate;

    public RectTransform GlobalVariableCreatorView;
    public RectTransform GlobalVariableCreatorTemplate;

    public RectTransform Autocomplete;
    public RectTransform AutocompleteOptionTemplate;
    public RectTransform AutocompleteOptionView;

    public RectTransform FunctionParameters;
    public Text FunctionParametersText;

    public GameObject[] Tabs;
    string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";

    public Text[] PPS_Values;

    bool anyKeyHold;
    int CursorPosition = -1;

    override public void OpenUI () {
        scriptInputField.text = Target.GetComponent<ConsoleScript>().encscript;
        nameInputField.text = Target.Name;

        UpdateFunctionExecutionTabs();

        PPS_Values[0].text = Target.ComsumePPS.ToString();
        PPS_Values[1].text = Target.PPSComsumption.ToString();
        if(Target.PowerSource != null) {
            PPS_Values[2].text = Target.PowerSource.Name;
        } else {
            PPS_Values[2].text = "Null";
        }
    }

    public void CreateGlobalVariableTemplate () {
        ClearGlobalVariableCreator();

        for(int i = 0; i < Target.GetComponent<ConsoleScript>().GlobalVariable.Count; i++) {
            GameObject gvt = (GameObject)Instantiate(GlobalVariableCreatorTemplate.gameObject,GlobalVariableCreatorView);
            gvt.SetActive(true);
            gvt.GetComponent<GlobalVariableUI>().EditVariableDisplay(Target.GetComponent<ConsoleScript>().GlobalVariable[i]);
            int x = i;
            gvt.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
                DeleteGlobalVariable(x);
            });
            gvt.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => {
                CreateNewGlobalVariable(x+1);
            });
        }
    }

    public void CreateNewGlobalVariable (int Index) {
        Target.GetComponent<ConsoleScript>().GlobalVariable.Insert(Index, new Variable("New_Variable" + Target.GetComponent<ConsoleScript>().GlobalVariable.Count, VariableType.v_int, null, new VariableParameters(true, VariableAccessType.v_readwrite)));
        Target.GetComponent<ConsoleScript>().GlobalVariableCompileValue.Insert(Index, null);
        int i = Index;
        if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_bool) {
            Target.GetComponent<ConsoleScript>().GlobalVariable[i].source = false;
        }
        if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_int) {
            Target.GetComponent<ConsoleScript>().GlobalVariable[i].source = 0;
        }
        if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_char) {
            Target.GetComponent<ConsoleScript>().GlobalVariable[i].source = '\0';
        }
        if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_float) {
            Target.GetComponent<ConsoleScript>().GlobalVariable[i].source = 0.0f;
        }
        if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_string) {
            Target.GetComponent<ConsoleScript>().GlobalVariable[i].source = string.Empty;
        }
        Target.GetComponent<ConsoleScript>().GlobalVariableCompileValue[i] = Target.GetComponent<ConsoleScript>().GlobalVariable[i].source;
        CreateGlobalVariableTemplate();
    }

    public void DeleteGlobalVariable (int ID) {
        Target.GetComponent<ConsoleScript>().GlobalVariable.RemoveAt(ID);
        CreateGlobalVariableTemplate();
    }

    public void UpdateGlobalVariable () {
        for(int i = 0; i < Target.GetComponent<ConsoleScript>().GlobalVariable.Count; i++) {
            Target.GetComponent<ConsoleScript>().GlobalVariable[i].Id = GlobalVariableCreatorView.GetChild(i).GetComponent<GlobalVariableUI>().Name;
            Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType = GlobalVariableCreatorView.GetChild(i).GetComponent<GlobalVariableUI>().variableType;
            if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableParameters != null) {
                Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableParameters.IsPublic = GlobalVariableCreatorView.GetChild(i).GetComponent<GlobalVariableUI>().IsPublic;
            }
            if(i >= Target.GetComponent<ConsoleScript>().GlobalVariableCompileValue.Count || i < 0) {
                object o = null;
                if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_bool) {
                    o = false;
                }
                if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_int) {
                    o = 0;
                }
                if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_float) {
                    o = 0.0f;
                }
                if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_string) {
                    o = "";
                }
                if(Target.GetComponent<ConsoleScript>().GlobalVariable[i].variableType == VariableType.v_char) {
                    o = '\0';
                }
                Target.GetComponent<ConsoleScript>().GlobalVariableCompileValue.Insert(i, o);
            }
            Target.GetComponent<ConsoleScript>().GlobalVariableCompileValue[i] = GlobalVariableCreatorView.GetChild(i).GetComponent<GlobalVariableUI>().Value;
        }
    }

    public void ClearGlobalVariableCreator () {
        for(int i = 0; i < GlobalVariableCreatorView.childCount; i++) {
            Destroy(GlobalVariableCreatorView.GetChild(i).gameObject);
        }
    }

    public void ClearFunctionExecuter () {
        for(int i = 0; i < FunctionExecuterView.childCount; i++) {
            Destroy(FunctionExecuterView.GetChild(i).gameObject);
        }
    }

    public void UpdateFunctionExecutionTabs () {
        ClearFunctionExecuter();
        for(int i = 0; i < Target.GetComponent<ConsoleScript>().GlobalFunctionNames.Count; i++) {
            GameObject button = (GameObject)Instantiate(FunctionExecuterTemplate.gameObject,FunctionExecuterView);
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<Text>().text = Target.GetComponent<ConsoleScript>().GlobalFunctionNames[i];
            int x = i;
            button.GetComponent<Button>().onClick.AddListener(() => {
                StartCoroutine(Target.GetComponent<ConsoleScript>().ExecuteFunction(Target.GetComponent<ConsoleScript>().GlobalFunctionNames[x], Target.GetComponent<ConsoleScript>().GetFunctionParametersTemplate(Target.GetComponent<ConsoleScript>().GlobalFunctionNames[x])));
            });
        }
    }

    public void Save () {
        if(!Tabs[1].activeSelf) {
            //Manual
            Script = scriptInputField.text;
            Target.GetComponent<ConsoleScript>().encscript = Script;
            StartCoroutine(Target.GetComponent<ConsoleScript>().OnCompilation());
        } else {
            //TODO: Automatic (Visual) Coding: Compile visual script
            //Automatic
        }
        Target.Name = nameInputField.text;

        if(Target.GetComponent<ConsoleScript>().GlobalVariable.Count == Target.GetComponent<ConsoleScript>().GlobalVariableCompileValue.Count) {
            for(int i = 0; i < Target.GetComponent<ConsoleScript>().GlobalVariable.Count; i++) {
                Target.GetComponent<ConsoleScript>().GlobalVariable[i].source = Target.GetComponent<ConsoleScript>().GlobalVariableCompileValue[i];
            }
        }
        UpdateFunctionExecutionTabs();
        CreateGlobalVariableTemplate();
    }

    public void ParametersNameChanged (string Name) {
        Target.Name = Name;
    }

    void Update () {
        if(Tabs[0].activeInHierarchy && scriptInputField.isFocused) {
            CursorPosition = scriptInputField.caretPosition;
        }

        if(Input.anyKey) {
            anyKeyHold = true;
        }

        //TODO: Autocomplete stuff here
        if(Tabs[0].activeInHierarchy) {
            //Function Autocomplete: Get the last ( detected. Find the last . before it. Get the Name and the Function kit attached to it and if everything is there display the helper and format some text
            string FunctionHelperText = string.Empty;
            if(scriptInputField.text.Contains("(") && scriptInputField.text.Length > 3) {
                string s = scriptInputField.text;
                int Phase = 0;
                string First = string.Empty;
                string Second = string.Empty;
                int Lenght = 1;
                for(int i = CursorPosition - 1; i >= 0; i--) {
                    if(i >= s.Length) {
                        break;
                    }
                    if(Phase == 0) {
                        if(s[i] == '(') {
                            Phase = 1;
                        } else if(!AllowedChars.Contains(s[i].ToString())) {
                            break;
                        }
                    } else if(Phase == 1) {
                        if(AllowedChars.Contains(s[i].ToString())) {
                            First = First.Insert(0, s[i].ToString());
                        } else if(s[i] == '.') {
                            Phase = 2;
                            Lenght = 2;
                        } else {
                            Lenght = 1;
                            break;
                        }
                    } else if(Phase == 2) {
                        if(AllowedChars.Contains(s[i].ToString())) {
                            Second = Second.Insert(0, s[i].ToString());
                        } else {
                            break;
                        }
                    }
                }
                if(Phase > 0) {
                    if(Lenght == 1) {
                        if(First.Length > 0) {
                            if(Target.GetComponent<ConsoleScript>().GlobalFunctionNames.Contains(First)) {
                                List<Variable> ft = Target.GetComponent<ConsoleScript>().GetFunctionParametersTemplate(First);
                                FunctionHelperText += "(";
                                foreach(Variable v in ft) {
                                    if(FunctionHelperText != "(") {
                                        FunctionHelperText += ", ";
                                    }

                                    FunctionHelperText += Variable.TypeToString(v.variableType);
                                    FunctionHelperText += " ";
                                    FunctionHelperText += v.Id;
                                }
                                FunctionHelperText += ")";
                            }
                        }
                    } else if(Lenght == 2) {
                        if(First.Length > 0 && Second.Length > 0) {
                            bool k = false;
                            foreach(WInteractable inter in Target.GetComponent<ConsoleScript>().transmitter.sources) {
                                if(inter.Name == Second) {
                                    k = true;
                                }
                            }
                            if(k) {
                                FunctionTemplate ft = Target.GetComponent<ConsoleScript>().transmitter.AccessSpecificInteractableFunction(Second, First);
                                FunctionHelperText += "(";
                                foreach(VariableTemplate v in ft.Parameters) {
                                    if(FunctionHelperText != "(") {
                                        FunctionHelperText += ", ";
                                    }

                                    FunctionHelperText += Variable.TypeToString(v.variableType);
                                    FunctionHelperText += " ";
                                    FunctionHelperText += v.Id;
                                }
                                FunctionHelperText += ")";
                            }
                        }
                    }
                }
            }
            if(FunctionHelperText.Length > 0) {
                FunctionParametersText.text = FunctionHelperText;
                if(!FunctionParameters.gameObject.activeSelf) {
                    FunctionParameters.gameObject.SetActive(true);
                }
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)FunctionParameters.transform);
            } else {
                if(FunctionParameters.gameObject.activeSelf) {
                    FunctionParameters.gameObject.SetActive(false);
                }
            }

            ConsoleScript c = Target.GetComponent<ConsoleScript>();
            string textpart = string.Empty;
            if(CursorPosition - 1 >= 0 && CursorPosition - 1 < scriptInputField.text.Length) {
                if(AllowedChars.Contains(scriptInputField.text[CursorPosition - 1].ToString())) {
                    textpart = GetTextAfterCaret(scriptInputField.text, CursorPosition);
                } else {
                    textpart = string.Empty;
                }
            } else {
                textpart = string.Empty;
            }

            if(string.IsNullOrEmpty(textpart)) {
                textpart = " ";
            }

            if(InputControl.GetInput(InputControl.InputType.MouseSecondairyPress)) {
                Autocomplete.position = Input.mousePosition;
            }

            //Autocomplete
            if((c.AllowedChars.Contains(textpart[0].ToString()) || (string.IsNullOrWhiteSpace(textpart) || string.IsNullOrEmpty(textpart))) && !char.IsDigit(textpart[0])) {

                List<string> AutocompleteResult = GetAutocompleteOptions();

                //Set the position of the autocomplete box and config it
                //Must be able to understand Arrow and Enters
                if(AutocompleteResult.Count > 0 && ((string.IsNullOrWhiteSpace(textpart)||string.IsNullOrEmpty(textpart)) == InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete)) || FunctionParameters.gameObject.activeSelf) {
                    if(!Autocomplete.gameObject.activeInHierarchy) {
                        Autocomplete.gameObject.SetActive(true);
                    }
                    if((!Input.anyKey && anyKeyHold) || InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete) && !FunctionParameters.gameObject.activeSelf) {
                        for(int i = 0; i < AutocompleteOptionView.childCount - 1; i++) {
                            Destroy(AutocompleteOptionView.GetChild(i + 1).gameObject);
                        }
                        for(int i = 0; i < AutocompleteResult.Count; i++) {
                            GameObject Option = (GameObject)Instantiate(AutocompleteOptionTemplate.gameObject, AutocompleteOptionView);
                            Option.SetActive(true);
                            int x = i;
                            Option.GetComponent<Button>().onClick.AddListener(() => {
                                LoadAutocompleteOption(x);
                            });
                            Option.transform.GetChild(0).GetComponent<Text>().text = AutocompleteResult[i];
                        }
                    }
                } else {
                    if(Autocomplete.gameObject.activeInHierarchy && !FunctionParameters.gameObject.activeSelf) {
                        Autocomplete.gameObject.SetActive(false);
                    }
                }
            }
        } else if(Autocomplete.gameObject.activeInHierarchy) {
            Autocomplete.gameObject.SetActive(false);
        }
        if(Tabs[3].activeInHierarchy) {
            UpdateGlobalVariable();
        }
    }

    public void LoadAutocompleteOption (int i) {
        List<string> option = GetAutocompleteOptions();
        int autosegment = GetIndexAfterCaret(scriptInputField.text, CursorPosition);

        scriptInputField.text = scriptInputField.text.Remove(CursorPosition-autosegment, autosegment);
        CursorPosition -= autosegment;
        scriptInputField.text = scriptInputField.text.Insert(CursorPosition, option[i]);
        CursorPosition += option[i].Length;
        scriptInputField.caretPosition = CursorPosition-autosegment;
    }

    public List<string> GetAutocompleteOptions () {
        ConsoleScript c = Target.GetComponent<ConsoleScript>();
        string textpart = GetTextAfterCaret(scriptInputField.text, CursorPosition);
        if(string.IsNullOrEmpty(textpart)) {
            textpart = " ";
        }

        bool EmptyLine = true;
        int autos = GetIndexAfterCaret(scriptInputField.text, CursorPosition) + 1;
        if(CursorPosition - 1 > 0) {
            if(CursorPosition - autos >= 0 && CursorPosition - autos < scriptInputField.text.Length) {
                if(scriptInputField.text[CursorPosition - autos] == '.') {
                    EmptyLine = false;
                }
            } else {
                EmptyLine = true;
            }
        } else {
            EmptyLine = true;
        }

        List<string> AutocompleteResult = new List<string>();
        if(EmptyLine) {
            for(int k = 0; k < c.Keyword.Length; k++) {
                if(c.Keyword[k].StartsWith(textpart) || (string.IsNullOrWhiteSpace(textpart) && InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete))) {
                    AutocompleteResult.Add(c.Keyword[k]);
                }
            }
            for(int k = 0; k < c.transmitter.sources.Count; k++) {
                if(c.transmitter.sources[k].Name.StartsWith(textpart) || (string.IsNullOrWhiteSpace(textpart) && InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete))) {
                    AutocompleteResult.Add(c.transmitter.sources[k].Name);
                }
            }
            for(int k = 0; k < c.GlobalVariable.Count; k++) {
                if(c.GlobalVariable[k].Id.StartsWith(textpart) || (string.IsNullOrWhiteSpace(textpart) && InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete))) {
                    AutocompleteResult.Add(c.GlobalVariable[k].Id);
                }
            }
            for(int k = 0; k < c.GlobalFunctionNames.Count; k++) {
                if(c.GlobalFunctionNames[k].StartsWith(textpart) || (string.IsNullOrWhiteSpace(textpart) && InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete))) {
                    AutocompleteResult.Add(c.GlobalFunctionNames[k]);
                }
            }
        }

        if(CursorPosition - 1 > 0) {
            //TODO: Transmitter variable/functions
            //Get the previous transmitter source name
            //Drain info from it
            int autosegment = GetIndexAfterCaret(scriptInputField.text, CursorPosition)+1;
            if(CursorPosition-autosegment >= 0 && CursorPosition-autosegment < scriptInputField.text.Length) {
                if(scriptInputField.text[CursorPosition-autosegment] == '.') {
                    string SourceName = GetTextAfterCaret(scriptInputField.text, CursorPosition-autosegment);

                    List<FunctionTemplate> externalFunctions = new List<FunctionTemplate>();
                    c.transmitter.AccessAllInteractableFunction(SourceName, out externalFunctions);
                    for(int k = 0; k < externalFunctions.Count; k++) {
                        if(externalFunctions[k].Name.StartsWith(textpart) || (string.IsNullOrWhiteSpace(textpart) && InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete))) {
                            AutocompleteResult.Add(externalFunctions[k].Name);
                        }
                    }

                    List<Variable> externalVariable = new List<Variable>();
                    c.transmitter.AccessAllInteractableVariables(SourceName, out externalVariable);
                    for(int k = 0; k < externalVariable.Count; k++) {
                        if(externalVariable[k].Id.StartsWith(textpart) || (string.IsNullOrWhiteSpace(textpart) && InputControl.GetInput(InputControl.InputType.CodingInputFieldShowAutocomplete))) {
                            AutocompleteResult.Add(externalVariable[k].Id);
                        }
                    }
                }
            }
        }

        return AutocompleteResult;
    }

    string GetTextAfterCaret (string TextParameters, int Caret) {
        string Result = string.Empty;
        for(int i = Caret-1; i >= 0; i--) {
            if(i >= 0 && i < TextParameters.Length) {
                if(AllowedChars.Contains(TextParameters[i].ToString())) {
                    Result = Result.Insert(0, TextParameters[i].ToString());
                } else {
                    break;
                }
            } else {
                break;
            }
        }
        return Result;
    }

    int GetIndexAfterCaret (string TextParameters, int Caret) {
        for(int i = Caret - 1; i >= 0; i--) {
            if(i >= 0 && i < TextParameters.Length) {
                if(!AllowedChars.Contains(TextParameters[i].ToString())) {
                    return Caret - i - 1;
                }
            }
        }
        return Caret;
    }
}
