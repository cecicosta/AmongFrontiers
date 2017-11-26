using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Player))]
public partial class PlayerInput : CharacterAttributes {

    
    [SerializeField]
    public Condition runTrigger;
    public Condition moveTrigger;
    public Condition standTrigger;
    public Condition jumpTrigger;
    public Condition attackTrigger;
    public Condition attackAndWalkTrigger;
    public Condition jumpAndAttackingTrigger;
    public Condition airboneTrigger;
    public Condition recoveryRam;

    public GameObject graphics;
    public bool stopped = false;

    private ToolKitEventTrigger eventTrigger;
    private Player player;

    private float horizontalInput = 0;
    private bool flip = false;
    private bool still = false;
    private bool chasing = false;
    private Vector2 target;

    public float runSpeed;
    public float walkSpeed;
    public float attackCoolDown = 3;
    public float attackDuration = 0;
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
    private bool jump;

    void Start () {
		player = GetComponent<Player> ();
        eventTrigger = new ToolKitEventTrigger();
        onHealthChangeNotify.Invoke(health/totalHealth);
    }

    private void OnEnable() {
        onHealthChangeNotify.Invoke(health / totalHealth);
    }

    void Update () {

        jump = false;
        running = false;
        horizontalInput = 0;

        if (stopped) {
            CheckCurrentState();
            return;
        }

        if (stamina == 0) {
            player.SetDirectionalInput(Vector2.zero);

            if (Input.GetKeyUp(KeyCode.Space)) {
                player.OnJumpInputUp();
            }
            CheckCurrentState();
            return;
        }

        Vector2 position = InterfaceManager.cursorWorldPosition;

        //Get mouse or touch direction input
        if (false && InterfaceManager.worldPressed) {
            //Correct character flickering direction when the cursor moves right-left or vice-versa
            if (still) {
                if (horizontalInput > 0 && transform.position.x - 0.2 < position.x)
                    horizontalInput = 1;
                else if (transform.position.x > position.x)
                    horizontalInput = -1;
                else if (horizontalInput < 0 && transform.position.x + 0.2 > position.x)
                    horizontalInput = -1;
                else if (transform.position.x < position.x)
                    horizontalInput = 1;
            } else {
                if (transform.position.x + 0.5f < position.x) {
                    horizontalInput = 1;
                    still = true;
                } else if (transform.position.x - 0.5f > position.x) {
                    horizontalInput = -1;
                    still = true;
                }
            }
            chasing = false;
        }

        //Get key direction input
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0) {
            still = false;
            horizontalInput = Input.GetAxis("Horizontal");
            chasing = false;
        } else if (chasing) {
            if (Mathf.Abs(transform.position.x - target.x) > 0.2)
                horizontalInput = Mathf.Sign(target.x - transform.position.x);
            else
                chasing = false;
        } 

        //Flip direction
        if (horizontalInput > 0.1f) {
            if (flip) {
                Flip();
            }
        } else if (horizontalInput < -0.1f) {
            if (!flip) {
                Flip();
            }
        }

        //Get attack input
        if (Input.GetKeyDown(attackInput) && (Time.time - lastAttack > attackCoolDown)) {
            DoAttack();
            lastAttack = Time.time;
            attacking = true;
        }

        //Get jump input
        if (Input.GetKeyDown(KeyCode.Space)) {
            jump = true;
            player.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            player.OnJumpInputUp();
        }

        //Get Run input
        if (Input.GetKey(runInput)) {
            running = true;
            player.moveSpeed = runSpeed;
        } else {
            running = false;
            player.moveSpeed = walkSpeed;
        }

        player.SetDirectionalInput(new Vector2(horizontalInput, 0));

        CheckCurrentState();
    }

    private void CheckCurrentState() {
        runTrigger.BoolValue = false;
        jumpTrigger.BoolValue = false;
        jumpAndAttackingTrigger.BoolValue = false;
        moveTrigger.BoolValue = false;
        attackAndWalkTrigger.BoolValue = false;
        attackTrigger.BoolValue = false;
        standTrigger.BoolValue = false;
        airboneTrigger.BoolValue = false;

        if (jump && !attacking) {
            jumpTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(jumpTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(jumpTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (player.IsAirbone() && attacking) {
            jumpAndAttackingTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(jumpAndAttackingTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(jumpAndAttackingTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (horizontalInput != 0 && running) {
            runTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(runTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(runTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (horizontalInput != 0 && !player.IsAirbone() && !attacking) {
            moveTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(moveTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(moveTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (horizontalInput != 0 && !player.IsAirbone() && attacking) {
            attackAndWalkTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(attackAndWalkTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(attackAndWalkTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (player.IsAirbone()) {
            airboneTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(airboneTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(airboneTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else if (attacking) {
            attackTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(attackTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(attackTrigger);
            eventTrigger.TriggerEvent(tkevent);
        } else {
            standTrigger.BoolValue = true;
            GameVariablesManager.Instance.ChangeConditionValue(standTrigger);
            ToolKitEvent tkevent = new ToolKitEvent(standTrigger);
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
        yield return new WaitForSeconds(attackDuration);

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

        ToolKitEvent tkevent = new ToolKitEvent(standTrigger);
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
