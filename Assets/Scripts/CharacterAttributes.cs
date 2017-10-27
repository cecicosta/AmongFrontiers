using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAttributes : MonoBehaviour {

    public float defense = 0;
    public float health = 30;
    public float stamina = 10;
    public float ram = 10;

    public bool useStamina = false;
    public bool useRam = false;

    protected float totalHealth;
    protected float totalStamina;
    protected float totalRam;

    public Condition onZeroHealthTrigger;
    public UnityEngine.Events.UnityEvent onZeroHealth;

    private void Awake() {
        totalHealth = health;
        totalRam = ram;
        totalStamina = stamina;
    }

    public virtual void DoDamage(float damage) {
        if (health == 0)
            return;

        health = health - (damage - defense);
        HealthUpdate();

        if (health <= 0) {
            health = 0;
            onZeroHealth.Invoke();
            new ToolKitEventTrigger().TriggerEvent(new ToolKitEvent(onZeroHealthTrigger));
        }
    }
    public virtual void SpendStamina(float amount) {
        if (!useStamina)
            return;

        stamina = stamina - amount < 0 ? 0 : stamina - amount;
        StaminaUpdate();
    }

    public virtual void SpendRam(float amount) {
        if (!useRam)
            return;

        if (ram == 0) {
            DoDamage(amount + defense);
            return;
        }
        ram = ram - amount < 0 ? 0 : ram - amount;
        RamUpdate();
    }


    public abstract void HealthUpdate();
    public abstract void StaminaUpdate();
    public abstract void RamUpdate();

    public virtual void RecoveryHealth(float amount) {
        if (health + amount > totalHealth)
            health = totalHealth;
        else
            health += amount;
        HealthUpdate();
    }
    public virtual void RecoveryStamina(float amount) {
        if (!useStamina)
            return;

        if (stamina + amount > totalStamina)
            stamina = totalStamina;
        else
            stamina += amount;
        StaminaUpdate();
    }
    public virtual void RecoveryRam(float amount) {
        if (!useRam)
            return;

        if (ram + amount > totalRam)
            ram = totalRam;
        else
            ram += amount;
        RamUpdate();
    }

    public abstract void DoAttack();
}
