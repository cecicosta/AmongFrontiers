using UnityEngine;
using System.Collections;

[System.Serializable]
public class LocateTargetAction : ToolKitAction {
    public Transform toChase;
    Transform position;
    public bool frontOnly = false;

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
        Camera main = Camera.main;
        Vector2 srcPosition = main.WorldToScreenPoint(gameObject.transform.position);
        //Enemy is in the screen?
        if (!main.pixelRect.Contains(srcPosition))
            return;

        CameraFollow cf = main.GetComponent<CameraFollow>();

        //Detected target is the current player
        if (cf.target.gameObject != colliding)
            return;

        Player player = colliding.GetComponent<Player>();

        //The player is at the locate distance
        if ((player.transform.position - gameObject.transform.position).magnitude > player.locateDistance)
            return;

        AnimatedInput animInput = gameObject.GetComponent<AnimatedInput>();
        animInput.target = player.transform.position;
        Player enemyPlayer = gameObject.GetComponent<Player>();

        //The player is at the attack distance
        if ((player.transform.position - gameObject.transform.position).magnitude > enemyPlayer.attackDistance)
            return;

        animInput.DoAttack(player.transform);
    }

    public override bool isStarted(){
		return true;
	}
	public override bool isFinished(){
		return (Mathf.Abs(position.position.x - toChase.position.x) <= 0.2);
	}

}
