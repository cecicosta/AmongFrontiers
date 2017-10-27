using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDash : MonoBehaviour {

    public float jumpForce = 20;
    public float pushForce = 15;
    private int currentJump;
    public int numberOfJumps;

    // Use this for initialization
    void Start () {
		
	}

    private void Update() {
        Controller2D controller = GetComponent<Controller2D>();

        if (controller == null)
            return;

        if (controller.collisions.below) {
            currentJump = 0;
            return;
        }
    }

    public void DoDoubleJump() {
        Player player = GetComponent<Player>();
        if (player == null)
            return;

        if (currentJump > 0 && currentJump < numberOfJumps) {
            player.velocity = Vector2.up.normalized * jumpForce;
            
        }
        currentJump++;
    }
    
	// Update is called once per frame
	public void DoDash () {
        
        Player player = GetComponent<Player>();
        if (player == null)
            return;

        Controller2D controller = GetComponent<Controller2D>();

        if (controller == null)
            return;

        //player.SetDirectionalInput(Vector2.zero);
        player.velocity = player.GetDirection().normalized * pushForce;

        //characterAttributes.DoDamage(amountOfDamage);
    }
}
