using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Player))]
public partial class PlayerInput : CharacterAttributes {

    public Condition RunTrigger;
    public Condition moveAnimationTrigger;
    public Condition standAnimationTrigger;
    public Condition jumpAnimationTrigger;
    public Condition attackAnimationTrigger;
    public Condition attackAndWalkTrigger;
    public Condition jumpAndAttackingTrigger;
    public Condition recoveryRam;

    public GameObject graphics;
    public bool stopped = false;

    private ToolKitEventTrigger eventTrigger;
    private Player player;

    private float directionX = 0;
    private bool flip = false;
    private bool still = false;
    private bool chasing = false;
    private Vector2 target;

    public float runSpeed;
    public float walkSpeed;
    public float attackCoolDown = 3;
    public KeyCode attackInput;
    public KeyCode runInput;
    public UnityEvent attackStarted;
    public UnityEvent attackEnd;

    public OnValueChangesFloat onKeepAttackNotify;
    public OnValueChangesFloat onHealthChangeNotify;
    public OnValueChangesFloat onStaminaChangeNotify;
    public OnValueChangesFloat onRamChangeNotify;

    private float lastAttack = 0;
    private bool attacking = false;
    private bool running;
    
    void Start () {
		player = GetComponent<Player> ();
        eventTrigger = new ToolKitEventTrigger();
        onHealthChangeNotify.Invoke(health/totalHealth);
    }

    private void OnEnable() {
        onHealthChangeNotify.Invoke(health / totalHealth);
    }

    void Update () {

        if (stopped)
            return;

        if (stamina == 0) {
            ToolKitEvent tkevent = new ToolKitEvent(standAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
            player.SetDirectionalInput(Vector2.zero);

            if (Input.GetKeyUp(KeyCode.Space)) {
                player.OnJumpInputUp();
            }
            return;
        }

        Vector2 position = InterfaceManager.cursorWorldPosition;

        if (false && InterfaceManager.worldPressed) {
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

        if(Input.GetKeyDown(attackInput) && (Time.time - lastAttack > attackCoolDown)) {
            DoAttack();
            lastAttack = Time.time;
            attacking = true;
        }

        player.SetDirectionalInput (new Vector2(directionX, 0));

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}

        if (Input.GetKey(runInput)) {
            running = true;
            player.moveSpeed = runSpeed;
        } else {
            running = false;
            player.moveSpeed = walkSpeed;
        }


        RunTrigger.BoolValue = false;
        jumpAnimationTrigger.BoolValue = false;
        jumpAndAttackingTrigger.BoolValue = false;
        moveAnimationTrigger.BoolValue = false;
        attackAndWalkTrigger.BoolValue = false;
        attackAnimationTrigger.BoolValue = false;
        standAnimationTrigger.BoolValue = false;

        if (player.IsJumping() && !attacking) {
            jumpAnimationTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(jumpAnimationTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(jumpAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (player.IsJumping() && attacking) {
            jumpAndAttackingTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(jumpAndAttackingTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(jumpAndAttackingTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (directionX != 0 && running) {
            RunTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(RunTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(RunTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (directionX != 0 && !player.IsJumping() && !attacking) {
            moveAnimationTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(moveAnimationTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(moveAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (directionX != 0 && !player.IsJumping() && attacking) {
            attackAndWalkTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(attackAndWalkTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(attackAndWalkTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (attacking) {
            attackAnimationTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(attackAnimationTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(attackAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else {
            standAnimationTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(standAnimationTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(standAnimationTrigger);
            eventTrigger.TriggerEvent(tkevent);
        }
    }

    public override void DoAttack() {
        StartCoroutine(AttackDuringTime());
    }

    IEnumerator AttackDuringTime() {
        attackStarted.Invoke();
        attacking = true;
        float started = Time.time;
        while (Input.GetKey(attackInput) && stamina != 0) {
            onKeepAttackNotify.Invoke(Time.time - started);
            yield return null;
        }
        attackEnd.Invoke();
        attacking = false;
    }

    public void Stop() {
        stopped = true;
        GetComponent<Player>().SetDirectionalInput(new Vector2(0, 0));

        ToolKitEvent tkevent = new ToolKitEvent(standAnimationTrigger);
        new ToolKitEventTrigger().TriggerEvent(tkevent);
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

    public override void HealthUpdate() {
        onHealthChangeNotify.Invoke(health / totalHealth);
    }

    public override void StaminaUpdate() {
        onStaminaChangeNotify.Invoke(stamina / totalStamina);
    }

    public override void RamUpdate() {
        onRamChangeNotify.Invoke(ram / totalRam);
    }
}
