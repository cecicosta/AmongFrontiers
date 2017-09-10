using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimAction : ToolKitAction {
	public string animationState;
	public int layer;

	public override void Execute( GameObject gameobjct ){
		if (animationState == null || animationState.CompareTo ("") == 0) {
			Debug.Log ( "AnimAction::Execute - Empty animation state." );		
			return;
		}

		Animator animator = gameobjct.GetComponent<Animator> ();
		if (animator != null) {
				animator.Play (animationState, layer);
		} else {
			Debug.Log ( "AnimAction::Execute - Object does not have a Animator component." );
			return;
		}
	}
	public override bool isStarted(){
		return false;
	}
	public override bool isFinished(){
		return false;
	}

}
