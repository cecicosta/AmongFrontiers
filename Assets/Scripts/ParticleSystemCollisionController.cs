using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemCollisionController : MonoBehaviour {

    ParticleSystem particleSystem;
    public float amountOfDamage = 5;
	// Use this for initialization
	void Start () {
        particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnParticleCollision(GameObject other) {
        Debug.Log("This shit collided");

        //ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[255];

        CharacterAttributes characterAttributes = other.GetComponent<CharacterAttributes>();
        if (characterAttributes == null)
            return;

        characterAttributes.DoDamage(amountOfDamage);

        //if (particleSystem.particleCount > 0) {
        //    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();

        //    particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
        //    Debug.Log(particles.Count);
        //    if (particles.Count > 0) {
        //        foreach (ParticleSystem.Particle p in particles) {
        //            attackTarget.GetComponent<CharacterAttributes>().DoDamage(attackDamage);
        //        }
        //    }
        //}
    }
}
