﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameVariablesManager : Singleton<GameVariablesManager> {
	public List<Condition> sceneVariables = new List<Condition>();
	public Dictionary<string, Condition> variables = new Dictionary<string, Condition>();
    private List<Condition> inputConditions = new List<Condition>();
    // Use this for initialization
    void Awake () {
		foreach (Condition v in sceneVariables) {
            Condition c = v;
            variables.Add(c.identifier, c);
            RetrieveCondition(c);
		}
		ToolKitEventHandle.Instance.onEvent += onTKEvent;
        
        inputConditions.AddRange(sceneVariables.FindAll(x => x.type == Condition.VariableType.Input));
    }

    public bool ChangeConditionValue(Condition c) {
        if (!variables.ContainsKey(c.identifier))
            return false;
        variables[c.identifier].Copy(c);
        return true;
    }

    public bool PersistsCondition(Condition c) {
        if (!variables.ContainsKey(c.identifier))
            return false;

        byte[] data = DataManipulationUtils.ObjectToByteArray(c);
        string stringData = Convert.ToBase64String(data);
        PlayerPrefs.SetString(c.identifier, stringData);
        variables[c.identifier].Copy(c);
        return true;
    }

    public bool PersistConsitionInSession(Condition c) {
        if (!variables.ContainsKey(c.identifier))
            return false;

        variables[c.identifier].Copy(c);
        SaveGameVariablesSession.Instance.SaveVariable(c);
        return true;
    }

    public bool RetrieveCondition(Condition c) {
        if (!variables.ContainsKey(c.identifier))
            return false;

        if (SaveGameVariablesSession.Instance.LoadGameVariable(c.identifier) != null) {
            c = SaveGameVariablesSession.Instance.LoadGameVariable(c.identifier);
            variables[c.identifier].Copy(c);
            return true;
        }

        //if (PlayerPrefs.GetString(c.identifier) != "") {
        //    string stringData = PlayerPrefs.GetString(c.identifier);
        //    byte[] result = Convert.FromBase64String(stringData);
        //    c = (Condition)DataManipulationUtils.ByteArrayToObject(result);
        //    variables[c.identifier].Copy(c);
        //    return true;
        //}

        return false;
    }

    public Condition CreateSceneCondition(string identifier, 
    Condition.VariableType type = Condition.VariableType.Trigger, 
    Condition.VariableCondition condition = Condition.VariableCondition.Equal) {
        Condition c = new Condition();
        c.identifier = identifier;
        c.type = type;
        c.comparison = condition;
        return c;
    }
	
	// Update is called once per frame
	void Update () {
        foreach(Condition c in inputConditions) {
            if (Input.GetKeyDown(c.InputValue)) {
                c.inputState = Condition.InputState.Down;
                (new ToolKitEventTrigger()).TriggerEvent(new ToolKitEvent(c));
            }else if (Input.GetKey(c.InputValue)) {
                c.inputState = Condition.InputState.Press;
                (new ToolKitEventTrigger()).TriggerEvent(new ToolKitEvent(c));
            } else if (Input.GetKeyUp(c.InputValue)) {
                c.inputState = Condition.InputState.Up;
                (new ToolKitEventTrigger()).TriggerEvent(new ToolKitEvent(c));
            }
        }
	}

	public Condition GetVariable(string variable){
		return variables.ContainsKey(variable)? variables [variable] : null;
	}

    public List<Condition> GetSceneConditions() {
        return sceneVariables;
    }

	public List<Condition> getAllVariables(){
        List<Condition> variables = new List<Condition>();
		variables.AddRange (GameVariablesManager.Instance.sceneVariables.ToArray());
		return variables;
	}

    /// <summary>
    /// If a event is trigged by the ToolKitEventHandler, this method is called
    /// it will update the condition to the condition emmited by the event, only if it match the identifier and type
    /// </summary>
    /// <param name="tkEvent"></param>
	public void onTKEvent(ToolKitEvent tkEvent){

        if (tkEvent.condition.type == Condition.VariableType.Trigger || tkEvent.condition.type == Condition.VariableType.Int ||
            tkEvent.condition.type == Condition.VariableType.Float || tkEvent.condition.type == Condition.VariableType.Bool) {

			if( variables.ContainsKey( tkEvent.condition.identifier ) && variables[tkEvent.condition.identifier].type == tkEvent.condition.type  ){
				variables[tkEvent.condition.identifier] = tkEvent.condition;
			}
		}
	}


}
