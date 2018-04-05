using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WObjectTransmitter {
	public List<WInteractable> sources;

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
		variables = null;
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
		variables = null;
		return false;
	}

	public void SendVariableData (string InteractableName, List<Variable> variables) {
		
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