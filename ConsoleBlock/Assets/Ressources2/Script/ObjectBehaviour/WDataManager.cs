using System;
using System.Collections.Generic;
using UnityEngine;

public class WObjectTransmitter {
	public List<WInteractable> sources = new List<WInteractable>();

	public bool AccessWriteInteractableVariables (string InteractableName, out List<Variable> variables) {
		List<Variable> result = new List<Variable>();

		for(int i = 0; i < sources.Count; i++) {
			if(sources[i].Name == InteractableName) {
				for(int g = 0; g < sources[i].GlobalVariable.Count; g++) {
					if(sources[i].GlobalVariable[g].variableParameters.IsPublic) {
						if(sources[i].GlobalVariable[g].variableParameters.VariableAccess == VariableAccessType.v_readwrite) {
							result.Add(sources[i].GlobalVariable[g]);
						}
					}
				}
				variables = result;
				return true;
			}
		}
        variables = new List<Variable>();
        return false;
	}

	public bool AccessAllInteractableVariables (string InteractableName, out List<Variable> variables) {
		List<Variable> result = new List<Variable>();

		for(int i = 0; i < sources.Count; i++) {
			if(sources[i].Name == InteractableName) {
				for(int g = 0; g < sources[i].GlobalVariable.Count; g++) {
					if(sources[i].GlobalVariable[g].variableParameters.IsPublic) {
						result.Add(sources[i].GlobalVariable[g]);
					}
				}
				variables = result;
				return true;
			}
		}
		variables = new List<Variable>();
		return false;
	}

    public bool AccessAllInteractableFunction (string InteractableName, out List<FunctionTemplate> functions) {
        for(int i = 0; i < sources.Count; i++) {
            if(sources[i].Name == InteractableName) {
                functions = sources[i].GlobalFunctionsDictionnairy;
                return true;
            }
        }
        functions = new List<FunctionTemplate>();
        return false;
    }

    public FunctionTemplate AccessSpecificInteractableFunction (string InteractableName, string FunctionName) {
        for(int i = 0; i < sources.Count; i++) {
            if(sources[i].Name == InteractableName) {
                for(int x = 0; x < sources[i].GlobalFunctionsDictionnairy.Count; x++) {
                    if(sources[i].GlobalFunctionsDictionnairy[x].Name == FunctionName) {
                        return sources[i].GlobalFunctionsDictionnairy[x];
                    }
                }
                break;
            }
        }
        return null;
    }

    public void ApplyWriteInteractableVariables (string InteractableName, List<Variable> variables) {
        for(int i = 0; i < sources.Count; i++) {
            if(sources[i].Name == InteractableName) {
                for(int c = 0; c < variables.Count; c++) {
                    for(int g = 0; g < sources[i].GlobalVariable.Count; g++) {
                        if(sources[i].GlobalVariable[g].variableParameters.IsPublic && sources[i].GlobalVariable[g].variableParameters.VariableAccess == VariableAccessType.v_readwrite) {
                            if(variables[c].Id == sources[i].GlobalVariable[g].Id) {
                                sources[i].GlobalVariable[g].source = variables[c].source;
                                break;
                            }
                        }
                    }
                }
                break;
            }
        }
    }

    public void SendInteractableFunctionCall (string InteractableName, FunctionCaller caller) {
        for(int i = 0; i < sources.Count; i++) {
            if(sources[i].Name == InteractableName) {
                sources[i].FunctionCall.Add(caller);
                break;
            }
        }
    }
}

[Serializable]
public class Variable {
	public string Id;
	public VariableType variableType;
	public VariableParameters variableParameters;
	[HideInInspector]
	public object source;

	public Variable (string Id, VariableType variableType, object source) {
		this.Id = Id;
		this.source = source;
		this.variableType = variableType;
	}

	public Variable (string Id, VariableType variableType, object source, VariableParameters variableParameters) {
		this.Id = Id;
		this.source = source;
		this.variableType = variableType;
		this.variableParameters = variableParameters;
	}

	public static object StringToObject (string S, VariableType variableType) {
		switch(variableType) {
			case VariableType.v_bool:
				return (S=="true");
			case VariableType.v_int:
				return int.Parse(S);
			case VariableType.v_float:
				return float.Parse(S);
			case VariableType.v_string:
				return S.Remove(S.Length).Remove(0);
			case VariableType.v_char:
				return S[1];
		}
		return null;
	}

    public static VariableType StringToType (string S) {
        switch(S) {
            case "bool":
                return VariableType.v_bool;
            case "int":
                return VariableType.v_int;
            case "float":
                return VariableType.v_float;
            case "string":
                return VariableType.v_string;
            case "char":
                return VariableType.v_char;
        }
        return VariableType.v_bool;
    }

    public static string TypeToString (VariableType S) {
        switch(S) {
            case VariableType.v_bool:
            return "bool";
            case VariableType.v_int:
            return "int";
            case VariableType.v_float:
            return "float";
            case VariableType.v_string:
            return "string";
            case VariableType.v_char:
            return "char";

        }
        return "error";
    }
}

[Serializable]
public class SubVariable {
    public VariableType variableType;
    [HideInInspector]
    public object source;

    public SubVariable (VariableType variableType, object source) {
        this.source = source;
        this.variableType = variableType;
    }

    public SubVariable ApplyOperator (OperatorType operatorType, SubVariable SecondVariable) {
        SubVariable res = this;
        dynamic t1 = source;
        dynamic t2 = SecondVariable.source;

        if(operatorType == OperatorType.v_plus) {
            //float int becomes float
            if(this.variableType == VariableType.v_float || SecondVariable.variableType == VariableType.v_float) {
                res.variableType = VariableType.v_float;
            }
            res.source = t1 + t2;
        } else if(operatorType == OperatorType.v_minus) {
            if(this.variableType == VariableType.v_float || SecondVariable.variableType == VariableType.v_float) {
                res.variableType = VariableType.v_float;
            }
            res.source = t1 - t2;
        } else if(operatorType == OperatorType.v_multiply) {
            if(this.variableType == VariableType.v_float || SecondVariable.variableType == VariableType.v_float) {
                res.variableType = VariableType.v_float;
            }
            res.source = t1 * t2;
        } else if(operatorType == OperatorType.v_divide) {
            if(this.variableType == VariableType.v_float || SecondVariable.variableType == VariableType.v_float) {
                res.variableType = VariableType.v_float;
            }
            res.source = t1 / t2;
        } else if(operatorType == OperatorType.v_modulo) {
            if(this.variableType == VariableType.v_float || SecondVariable.variableType == VariableType.v_float) {
                res.variableType = VariableType.v_float;
            }
            res.source = t1 % t2;
        } else if(operatorType == OperatorType.v_greater) {
            res.variableType = VariableType.v_bool;
            res.source = t1 > t2;
        } else if(operatorType == OperatorType.v_smaller) {
            res.variableType = VariableType.v_bool;
            res.source = t1 < t2;
        } else if(operatorType == OperatorType.v_greaterEqual) {
            res.variableType = VariableType.v_bool;
            res.source = t1 >= t2;
        } else if(operatorType == OperatorType.v_smallerEqual) {
            res.variableType = VariableType.v_bool;
            res.source = t1 <= t2;
        } else if(operatorType == OperatorType.v_equal) {
            res.variableType = VariableType.v_bool;
            res.source = t1 == t2;
        } else if(operatorType == OperatorType.v_notEqual) {
            res.variableType = VariableType.v_bool;
            res.source = t1 != t2;
        } else if(operatorType == OperatorType.v_binairyAnd) {
            res.source = t1 & t2;
        } else if(operatorType == OperatorType.v_binairyOr) {
            res.source = t1 | t2;
        } else if(operatorType == OperatorType.v_binairyNor) {
            res.source = t1 ^ t2;
        } else if(operatorType == OperatorType.v_and) {
            res.variableType = VariableType.v_bool;
            res.source = t1 && t2;
        } else if(operatorType == OperatorType.v_or) {
            res.variableType = VariableType.v_bool;
            res.source = t1 || t2;
        } else if(operatorType == OperatorType.v_binairyLeftShift) {
            res.source = t1 << t2;
        } else if(operatorType == OperatorType.v_binairyRightShift) {
            res.source = t1 >> t2;
        }

        return res;
    }

    public SubVariable ApplyOperator (OperatorType operatorType) {
        SubVariable res = this;
        dynamic t1 = source;

        if(operatorType == OperatorType.v_minus) {
            res.source = -t1;
        } else if(operatorType == OperatorType.v_not) {
            res.source = !t1;
        } else if(operatorType == OperatorType.v_binairyNot) {
            res.source = ~t1;
        } else if(operatorType == OperatorType.v_round) {
            res.variableType = VariableType.v_int;
            res.source = Mathf.RoundToInt(t1);
        }

        return res;
    }
}

[Serializable]
public class SolveElement : IEquatable<SolveElement> {
    public SolveElementType type;
    public SubVariable subVariable;
    public OperatorType operatorType;

    public SolveElement (SolveElementType type) {
        this.type = type;
    }

    public SolveElement (SolveElementType type, SubVariable subVariable) {
        this.type = type;
        this.subVariable = subVariable;
    }

    public SolveElement (SolveElementType type, OperatorType operatorType) {
        this.type = type;
        this.operatorType = operatorType;
    }

    public static OperatorType StringToOperator (string Operator) {
        switch(Operator) {
            case "+":
            return OperatorType.v_plus;
            case "-":
            return OperatorType.v_minus;
            case "*":
            return OperatorType.v_multiply;
            case "/":
            return OperatorType.v_divide;
            case "%":
            return OperatorType.v_modulo;
            case ">":
            return OperatorType.v_greater;
            case "<":
            return OperatorType.v_smaller;
            case "==":
            return OperatorType.v_equal;
            case ">=":
            return OperatorType.v_greaterEqual;
            case "<=":
            return OperatorType.v_smallerEqual;
            case "!=":
            return OperatorType.v_notEqual;
            case "&":
            return OperatorType.v_binairyAnd;
            case "|":
            return OperatorType.v_binairyOr;
            case "^":
            return OperatorType.v_binairyNor;
            case "&&":
            return OperatorType.v_and;
            case "||":
            return OperatorType.v_or;
            case "<<":
            return OperatorType.v_binairyRightShift;
            case ">>":
            return OperatorType.v_binairyLeftShift;
            case "!":
            return OperatorType.v_not;
            case "~":
            return OperatorType.v_binairyNot;
            case "#":
            return OperatorType.v_round;
        }
        return OperatorType.v_multiply;
    }

    public int CompareTo (SolveElement obj) {
        if(this.type == SolveElementType.v_operator) {
            if(this.operatorType == obj.operatorType) {
                return 0;
            } else {
                return -1;
            }
        } else {
            return -1;
        }
    }

    public bool Equals (SolveElement other) {
        if(this.type == SolveElementType.v_operator && other.type == SolveElementType.v_operator) {
            if(this.operatorType == other.operatorType) {
                return true;
            } else {
                return false;
            }
        } else if(this.type == SolveElementType.v_closebrackets && other.type == SolveElementType.v_closebrackets) {
            return true;
        } else if(this.type == SolveElementType.v_openbrackets && other.type == SolveElementType.v_openbrackets) {
            return true;
        } else {
            return false;
        }
    }
}

public class VariableParameters {
	public bool IsPublic = false;
	public VariableAccessType VariableAccess = VariableAccessType.v_readwrite;

	public VariableParameters (bool IsPublic, VariableAccessType VariableAccess) {
		this.IsPublic = IsPublic;
		this.VariableAccess = VariableAccess;
	}
}

public enum VariableAccessType {
	v_readonly,
	v_readwrite
}

public enum VariableType {
	v_bool,
	v_int,
	v_float,
	v_string,
	v_char
}

public enum SolveElementType {
    v_operator,
    v_openbrackets,
    v_closebrackets,
    v_variable
}

public enum OperatorType {
    v_plus,
    v_minus,
    v_multiply,
    v_divide,
    v_modulo,
    v_greater,
    v_smaller,
    v_greaterEqual,
    v_smallerEqual,
    v_equal,
    v_notEqual,
    v_binairyAnd,
    v_binairyOr,
    v_binairyNor,
    v_binairyNot, 
    v_and,
    v_or,
    v_not,
    v_binairyRightShift,
    v_binairyLeftShift,
    v_round
}

public class FunctionCaller {
    public List<Variable> parameters;
    public string Name;

    public FunctionCaller (string Name, List<Variable> parameters) {
        this.parameters = parameters;
        this.Name = Name;
    }
}