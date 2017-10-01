using UnityEngine;
using System.Collections;

[System.Serializable]
public class ChangeVariableAction : ToolKitAction {
    
    [SerializeField]
    public Condition changeTo;
	
	public override void Execute( GameObject gameobjct ){

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