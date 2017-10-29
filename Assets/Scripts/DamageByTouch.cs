using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageByTouch : MonoBehaviour {

    public float amountOfDamage = 5;
    public float force = 5;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //private void OnTriggerEnter2D(Collider2D collision) {
    //    Player player = collision.GetComponent<Player>();
    //    if (player == null)
    //        return;
    //    enterVelocity = player.velocity; 
    //}

    private void OnTriggerEnter2D(Collider2D collision) {
        CharacterAttributes characterAttributes = collision.GetComponent<CharacterAttributes>();
        if (characterAttributes == null)
            return;

        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;

        Controller2D controller = collision.GetComponent<Controller2D>();

        if (controller == null)
            return;

        player.SetDirectionalInput(Vector2.zero);
        Vector2 velocity = -player.velocity.normalized;
        velocity.y = 0;
        player.velocity = velocity.normalized*force;

        characterAttributes.DoDamage(amountOfDamage);
    }
}
