using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventAction : ToolKitAction {
    public UnityEvent onInteractionTrigged;

	public override void Execute( GameObject dialogController ){
        onInteractionTrigged.Invoke();
    }

	public override bool isStarted(){
		return false;
	}
	public override bool isFinished(){
		return false;
	}
}
