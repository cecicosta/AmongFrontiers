﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (Player))]
public class AnimatedInput : MonoBehaviour {

    public Condition moveAnimationTrigger;
    public Condition standAnimationTrigger;
    public Condition jumpAnimationTrigger;
    public GameObject graphics;
    public GameObject attackParticleGraphic;

    public Vector2 target;
    public float attackDuration = 1;
    public bool jumpStart = false;
    public bool jumpEnd;
    public bool stopped = false;

    private ToolKitEventTrigger eventTrigger;
    private Player player;

    private float directionX = 0;
    private bool flip = false;
    private bool still = false;
    public bool chasing = false;

    public float health = 30;

    public UnityEngine.Events.UnityEvent onZeroHealth;

    void Start () {
		player = GetComponent<Player> ();
        eventTrigger = new ToolKitEventTrigger();
    }

	void Update () {

        if (stopped)
            return;

       if (chasing) {

            if (Mathf.Abs(transform.position.x - target.x) > 0.2)
                directionX = Mathf.Sign(target.x - transform.position.x);
            else
                directionX = 0;
        } 

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

		if (jumpStart) {
			player.OnJumpInputDown ();
            jumpStart = false;
		}
		if (jumpEnd) {
			player.OnJumpInputUp ();
            jumpEnd = false;
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

    public void DoAttack(Transform attackTarget) {
        StartCoroutine(AttackDuringTime(attackTarget));
    }

    IEnumerator AttackDuringTime(Transform attackTarget) {
        attackParticleGraphic.SetActive(true);
        float started = Time.time;
        yield return new WaitUntil(() => { return Time.time - started > attackDuration; });
        attackParticleGraphic.SetActive(false);
    }
    
    public void SetTarget(Transform targetTransform) {
        target = targetTransform.position;
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

    void DoDamage(float damage) {
        health -= damage;

        if (health <= 0) {
            health = 0;
            onZeroHealth.Invoke();
        }
    }
}