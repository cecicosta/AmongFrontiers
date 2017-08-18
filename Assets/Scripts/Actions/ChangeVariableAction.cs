using UnityEngine;
using System.Collections;

public class ChangeVariableAction : ToolKitAction {
	public Condition variable;
	
	public override void Execute( GameObject gameobjct ){
		ToolKitEventTrigger trigger = new ToolKitEventTrigger ();
		ToolKitEvent tkevent = new ToolKitEvent ();
		tkevent.type = ToolKitEvent.EventType.VARIABLE_CHANGE;
		tkevent.condition = (Condition)variable;
		trigger.TriggerEvent (tkevent);
	}
	public override bool isStarted(){
		return false;
	}
	public override bool isFinished(){
		return false;
	}
}