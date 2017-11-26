using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChangeVariableAction : ToolKitAction {
    
    [SerializeField]
    public Condition changeTo;
    public bool persistInSession = false;
	public override void Execute( GameObject gameobjct ){
        if (persistInSession) {
            GameVariablesManager.Instance.PersistConsitionInSession(changeTo);
        }else {
            GameVariablesManager.Instance.ChangeConditionValue(changeTo);
        }

        Condition copy = new Condition();
        copy.Copy(changeTo);
		ToolKitEventTrigger trigger = new ToolKitEventTrigger ();
		ToolKitEvent tkevent = new ToolKitEvent (copy);
		trigger.TriggerEvent (tkevent);
	}


    public override void Execute(GameObject colliding, GameObject gameobjct) {
        if (persistInSession) {
            GameVariablesManager.Instance.PersistConsitionInSession(changeTo);
        }else {
            GameVariablesManager.Instance.ChangeConditionValue(changeTo);
        }

        Condition copy = new Condition();
        copy.Copy(changeTo);
        ToolKitEventTrigger trigger = new ToolKitEventTrigger();
        ToolKitEvent tkevent = new ToolKitEvent(copy);
        trigger.TriggerEvent(tkevent);
    }

    public override bool isStarted(){
		return true;
	}
	public override bool isFinished(){
		return true;
	}
}