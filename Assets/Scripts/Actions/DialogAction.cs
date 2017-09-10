using UnityEngine;
using System.Collections;

[System.Serializable]
public class DialogAction : ToolKitAction {
	public string dialogFlag = "";
	public override void Execute( GameObject dialogController ){
		Debug.Log ( "DialogAction::Execute - START" );
		DialogController dc = dialogController.GetComponent<DialogController> ();
		if (dc != null) {
			dc.TriggerDialog (dialogFlag);
			Debug.Log ( "DialogAction::Execute - Object does not have a DialogController component." );
		}
		Debug.Log ( "DialogAction::Execute - STOP" );
	}
	public override bool isStarted(){
		return false;
	}
	public override bool isFinished(){
		return false;
	}
}
