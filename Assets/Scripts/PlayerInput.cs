using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

    public Condition moveAnimationTrigger;
    public Condition standAnimationTrigger;
    public Condition jumpAnimationTrigger;
    public GameObject graphics;
    public bool stopped = false;

    private ToolKitEventTrigger eventTrigger;
    private Player player;

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

        if (stopped)
            return;

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


        if (player.IsJumping()) {
            ToolKitEvent tkevent = new ToolKitEvent(jumpAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
        }else if (directionX != 0) {
            ToolKitEvent tkevent = new ToolKitEvent(moveAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else {
            ToolKitEvent tkevent = new ToolKitEvent(standAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
        }

        
    }

    public void Stop() {
        stopped = true;
        player.SetDirectionalInput(new Vector2(0, 0));

        ToolKitEvent tkevent = new ToolKitEvent(standAnimationTrigger);
        eventTrigger.TriggerEvent(tkevent);
    }

    public void Resume() {
        stopped = false;
    }

    void Flip() {
        // Switch the way the player is labelled as facing.
        flip = !flip;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = graphics.transform.localScale;
        theScale.x *= -1;
        graphics.transform.localScale = theScale;
    }
}
