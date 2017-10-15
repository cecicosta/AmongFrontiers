using UnityEngine;
using System.Collections;

[System.Serializable]
public class ChaseAction : ToolKitAction {
    public Transform toChase;
    Transform position;
	public override void Execute( GameObject gameobject ){

		AnimatedInput animator = gameobject.GetComponent<AnimatedInput> ();
		if (animator != null) {
            position = animator.transform;
            animator.target = toChase.position;
		} else {
			Debug.Log ("ChaseAction::Execute - Object does not have a AnimatedInput component.");
			return;
		}
	}
    
    public override void Execute(GameObject colliding, GameObject gameObject) {

        if(!gameObject.transform.IsChildOf(colliding.transform))
            return;

        AnimatedInput animator = colliding.GetComponent<AnimatedInput>();
        if (animator != null) {
            position = animator.transform;
            animator.target = toChase.position;
        } else {
            Debug.Log("ChaseAction::Execute - Object does not have a AnimatedInput component.");
            return;
        }
    }

    public override bool isStarted(){
		return true;
	}
	public override bool isFinished(){
        if (position == null || toChase == null)
            return false;

		return (Mathf.Abs(position.position.x - toChase.position.x) <= 0.2);
	}

}
