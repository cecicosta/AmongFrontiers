using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChangeVariableAction : ToolKitAction {
    
    [SerializeField]
    public Condition changeTo;
    public bool persists = false;
	public override void Execute( GameObject gameobjct ){
        if (persists)
            GameVariablesManager.Instance.PersistsCondition(changeTo);
        else {
            List<Condition> conditions = GameVariablesManager.Instance.getAllVariables();
            int index = conditions.FindIndex(x => x.identifier == changeTo.identifier);
            conditions[index].Copy(changeTo);
        }

		ToolKitEventTrigger trigger = new ToolKitEventTrigger ();
		ToolKitEvent tkevent = new ToolKitEvent (changeTo);
		trigger.TriggerEvent (tkevent);
	}
	public override bool isStarted(){
		return false;
	}
	public override bool isFinished(){
		return false;
	}
}