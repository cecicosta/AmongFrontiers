using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameVariablesManager : Singleton<GameVariablesManager> {
	public List<Condition> gameVariables = new List<Condition>();
	public List<Condition> sceneVariables = new List<Condition>();
	public Dictionary<string, Condition> variables = new Dictionary<string, Condition>();
    private List<Condition> inputConditions = new List<Condition>();
    // Use this for initialization
    void Start () {
		foreach (Condition v in sceneVariables) {
				variables.Add (v.identifier, v);
		}
		foreach (Condition v in gameVariables) {
				variables.Add (v.identifier, v);
		}
		ToolKitEventHandle.Instance.onEvent += onTKEvent;
        
        inputConditions.AddRange(sceneVariables.FindAll(x => x.type == Condition.VariableType.Input));
        inputConditions.AddRange(gameVariables.FindAll(x => x.type == Condition.VariableType.Input));
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
            if(Input.GetKeyDown(c.InputValue)) {
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
		List<Condition> variables = new List<Condition> (GameVariablesManager.Instance.gameVariables.ToArray());
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
