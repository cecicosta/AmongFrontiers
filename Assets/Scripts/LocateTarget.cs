using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocateTarget : MonoBehaviour {
    [System.Serializable]
    public class OnTrackTarget : UnityEvent<Transform> { }
    
    public Player thisPlayer;

    public float attackCoolDown = 3;

    private GameObject colliding;

    public OnTrackTarget onTrackTarget;
    public UnityEvent onTargetMissed;
    public OnTrackTarget onAttackTarget;

    private ToolKitEventTrigger trigger;
    private bool found;
    private bool previous;
    private float lastAttack;

    // Use this for initialization
    void Start () {
        trigger = new ToolKitEventTrigger();
	}
	
	// Update is called once per frame
	void Update () {
        if (previous != found && !found)
            onTargetMissed.Invoke();

        if (colliding == null) {
            previous = found;
            found = false;
            return;
        }

        Camera main = Camera.main;
        Vector2 srcPosition = main.WorldToScreenPoint(thisPlayer.transform.position);
        //Enemy is in the screen?
        if (!main.pixelRect.Contains(srcPosition)) {
            previous = found;
            found = false;
            return;
        }

        Player player = colliding.GetComponent<Player>();

        //The player is at the locate distance?
        if ((player.transform.position - thisPlayer.transform.position).magnitude > player.locateDistance) {
            previous = found;
            found = false;
            return;
        }

        onTrackTarget.Invoke(player.transform);

        previous = found;
        found = true;

        //The player is at the attack distance?
        if ((player.transform.position - thisPlayer.transform.position).magnitude > thisPlayer.attackDistance) {
            return;
        }

        if (Time.time - lastAttack > attackCoolDown) {
            onAttackTarget.Invoke(player.transform);
            lastAttack = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        Camera main = Camera.main;
        CameraFollow cf = main.GetComponent<CameraFollow>();

        //Detected target is the current player?
        if (cf.target.gameObject != collision.gameObject) {
            return;
        }

        colliding = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision) {

        Camera main = Camera.main;
        CameraFollow cf = main.GetComponent<CameraFollow>();

        //Detected target is the current player?
        if (cf.target.gameObject != collision.gameObject) {
            return;
        }

        colliding = null;
    }
}
