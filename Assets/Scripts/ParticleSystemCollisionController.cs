using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemCollisionController : MonoBehaviour {

    ParticleSystem particleSystem;
    public float amountOfDamage = 5;
    public float force = 2;
    public float damageInterval = 1;
    private float lastDamage = 0;

    // Use this for initialization
    void Start () {
        particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnParticleCollision(GameObject other) {
        if (Time.time - lastDamage < damageInterval)
            return;

        CharacterAttributes characterAttributes = other.GetComponent<CharacterAttributes>();
        if (characterAttributes == null)
            return;


        Player player = other.GetComponent<Player>();
        if (player == null)
            return;

        characterAttributes.DoDamage(amountOfDamage);

        lastDamage = Time.time;

        List<ParticleCollisionEvent> particles = new List<ParticleCollisionEvent>();

        particleSystem.GetCollisionEvents(other, particles);
        Debug.Log(particles.Count);
        if (particles.Count > 0) {
            foreach (ParticleCollisionEvent p in particles) {
                player.SetDirectionalInput(Vector2.zero);
                player.velocity = p.velocity.normalized * force;
                //attackTarget.GetComponent<CharacterAttributes>().DoDamage(attackDamage);
            }
        }
    }
}
