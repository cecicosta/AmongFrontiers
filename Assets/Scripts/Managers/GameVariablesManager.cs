using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameVariablesManager : Singleton<GameVariablesManager> {
	public List<Condition> gameVariables = new List<Condition>();
	public List<Condition> sceneVariables = new List<Condition>();
	public Dictionary<string, Condition> variables = new Dictionary<string, Condition>();
	// Use this for initialization
	void Start () {
		foreach (Condition v in sceneVariables) {
				variables.Add (v.identifier, v);
		}
		foreach (Condition v in gameVariables) {
				variables.Add (v.identifier, v);
		}
		ToolKitEventHandle.Instance.onEvent += onTKEvent;
	}

    public Condition CreateSceneCondition(string identifier, 
    Condition.VariableType type = Condition.VariableType.TRIGGER, 
    Condition.VariableCondition condition = Condition.VariableCondition.EQUAL) {
        Condition c = new Condition();
        c.identifier = identifier;
        c.type = type;
        c.variableCondition = condition;
        return c;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public Condition GetVariable(string variable){
		return variables [variable];
	}

    public List<Condition> GetSceneConditions() {
        return sceneVariables;
    }

	public List<Condition> getAllVariables(){
		List<Condition> variables = new List<Condition> (GameVariablesManager.Instance.gameVariables.ToArray());
		variables.AddRange (GameVariablesManager.Instance.sceneVariables.ToArray());
		return variables;
	}

	public void onTKEvent(ToolKitEvent tkEvent){

		if (tkEvent.type == ToolKitEvent.EventType.VARIABLE_CHANGE) {
			if( variables.ContainsKey( tkEvent.condition.identifier ) && variables[tkEvent.condition.identifier].type == tkEvent.condition.type  ){
				variables[tkEvent.condition.identifier] = tkEvent.condition;
			}
		}
	}
}
