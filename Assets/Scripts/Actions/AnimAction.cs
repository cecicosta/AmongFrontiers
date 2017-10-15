using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimAction : ToolKitAction {
	public string animationState;
	public int layer;

	public override void Execute( GameObject gameobject ){
		if (animationState == null || animationState.CompareTo ("") == 0) {
			Debug.Log ( "AnimAction::Execute - Empty animation state." );		
			return;
		}

		Animator animator = gameobject.GetComponent<Animator> ();
		if (animator != null) {
				animator.Play (animationState, layer);
		} else {
			Debug.Log ( "AnimAction::Execute - Object does not have a Animator component." );
			return;
		}
	}

    //It is a quick solution for the problem of all objects responding
    //to a condition when it is trigged by a specific one, but intended to be answer only by himself 
    public override void Execute(GameObject colliding, GameObject gameObject) {
        if (animationState == null || animationState.CompareTo("") == 0) {
            Debug.Log("AnimAction::Execute - Empty animation state.");
            return;
        }

        if(!colliding.transform.IsChildOf(gameObject.transform))
            return;

        Animator animator = colliding.GetComponent<Animator>();
        if (animator != null) {
            animator.Play(animationState, layer);
        } else {
            Debug.Log("AnimAction::Execute - Object does not have a Animator component.");
            return;
        }
    }

    public override bool isStarted(){
		return true;
	}
	public override bool isFinished(){
		return true;
	}

}
