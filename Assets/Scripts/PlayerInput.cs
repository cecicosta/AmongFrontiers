using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

    public string moveAnimationTrigger = "";
    public string standAnimationTrigger = "";

    private ToolKitEventTrigger eventTrigger;

    Player player;

    private float directionX = 0;
    private bool flip = false;
    private bool still = false;
    private bool chasing = false;
    private Vector2 target;
    void Start () {
		player = GetComponent<Player> ();
        eventTrigger = new ToolKitEventTrigger();
    }

	void Update () {

        Vector2 position = InterfaceManager.cursorWorldPosition;

        if (InterfaceManager.worldPressed) {
            //Correct character flickering direction when the cursor moves right-left or vice-versa
            if (still) {
                if (directionX > 0 && transform.position.x - 0.2 < position.x)
                    directionX = 1;
                else if (transform.position.x > position.x)
                    directionX = -1;
                else if (directionX < 0 && transform.position.x + 0.2 > position.x)
                    directionX = -1;
                else if (transform.position.x < position.x)
                    directionX = 1;
            } else {
                if (transform.position.x + 0.5f < position.x) {
                    directionX = 1;
                    still = true;
                } else if (transform.position.x - 0.5f > position.x) {
                    directionX = -1;
                    still = true;
                }
            }
            chasing = false;
        } else if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0) {
            still = false;
            directionX = Input.GetAxis("Horizontal");
            chasing = false;
        } else if (chasing) {
            if (Mathf.Abs(transform.position.x - target.x) > 0.2)
                directionX = Mathf.Sign(target.x - transform.position.x);
            else
                chasing = false;
        } else
            directionX = 0;


        if (directionX > 0.1f) {
            if (flip) {
                Flip();
            }
        } else if (directionX < -0.1f) {
            if (!flip) {
                Flip();
            }
        }


		player.SetDirectionalInput (new Vector2(directionX, 0));

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}

        if (directionX != 0) {
            ToolKitEvent tkevent = new ToolKitEvent();
            Condition condition = new Condition();
            condition.type = Condition.VariableType.TRIGGER;
            condition.identifier = moveAnimationTrigger;
            tkevent.condition = condition;
            tkevent.type = ToolKitEvent.EventType.VARIABLE_CHANGE;
            eventTrigger.TriggerEvent(tkevent);
        } else {
            ToolKitEvent tkevent = new ToolKitEvent();
            Condition condition = new Condition();
            condition.type = Condition.VariableType.TRIGGER;
            condition.identifier = standAnimationTrigger;
            tkevent.condition = condition;
            tkevent.type = ToolKitEvent.EventType.VARIABLE_CHANGE;
            eventTrigger.TriggerEvent(tkevent);
        }

    }


    void Flip() {
        // Switch the way the player is labelled as facing.
        flip = !flip;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
