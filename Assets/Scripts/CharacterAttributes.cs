using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttributes : MonoBehaviour {

    public float health = 30;

    public Condition onZeroHealthTrigger;
    public UnityEngine.Events.UnityEvent onZeroHealth;

    public virtual void DoDamage(float damage) {
        if (health == 0)
            return;

        health -= damage;

        if (health <= 0) {
            health = 0;
            onZeroHealth.Invoke();
            new ToolKitEventTrigger().TriggerEvent(new ToolKitEvent(onZeroHealthTrigger));
        }
    }

    public virtual void DoAttack() {

    }
}
