using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AttackCharging : MonoBehaviour {

    public ParticleSystem particles;
    public ParticleSystemCollisionController collController;

    public float timeToApex;
    public float maxParticleSizeMultiplier;
    public float maxDamageMultplier;
    public float maxForceMultiplier;
    private float baseParticleSize;
    private float baseDamage;
    private float baseForce;

    private bool started;

    public UnityEvent onChargeStarted;
    public UnityEvent onChargeFinished;
    private ParticleSystem.MainModule main;

    private void Start() {
        main = particles.main;
        baseParticleSize = particles.main.startSizeMultiplier;
        baseDamage = collController.amountOfDamage;
        baseForce = collController.force;
    }

    public void Charge(float time) {
        if (!started) {    
            main.startSizeMultiplier = baseParticleSize;
            collController.amountOfDamage = baseDamage;
            collController.force = baseForce;

            onChargeStarted.Invoke();
            started = true;
        }
        main = particles.main;
        main.startSizeMultiplier = Mathf.Lerp(baseParticleSize, baseParticleSize*maxParticleSizeMultiplier, time < timeToApex? time/timeToApex: timeToApex);
        collController.amountOfDamage = Mathf.Lerp(baseDamage, baseDamage*maxDamageMultplier, time < timeToApex ? time / timeToApex : timeToApex);
        collController.force = Mathf.Lerp(baseForce, baseForce* maxForceMultiplier, time < timeToApex ? time / timeToApex : timeToApex);
    }


    public void Zero() {
        onChargeFinished.Invoke();
        started = false;
    }
}
