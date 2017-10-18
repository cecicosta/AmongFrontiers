using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullToDirection : MonoBehaviour {
    
    public float force = 5;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter2D(Collider2D collision) {
        CharacterAttributes characterAttributes = collision.GetComponent<CharacterAttributes>();
        if (characterAttributes == null)
            return;

        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;


        Controller2D controller = collision.GetComponent<Controller2D>();

        Collider2D mine = GetComponent<Collider2D>();

        if (controller == null)
            return;

        if (mine.bounds.min.x < collision.bounds.min.x && collision.bounds.min.x < mine.bounds.max.x  && collision.bounds.min.y > mine.bounds.center.y) {
            player.SetDirectionalInput(Vector2.zero);
            player.velocity.y = force;
        }
        if (mine.bounds.min.x < collision.bounds.max.x && collision.bounds.max.x < mine.bounds.max.x && collision.bounds.min.y > mine.bounds.center.y) {
            player.SetDirectionalInput(Vector2.zero);
            player.velocity.y = force;
        }
        if (mine.bounds.min.x > collision.bounds.min.x && collision.bounds.max.x > mine.bounds.max.x && collision.bounds.min.y > mine.bounds.center.y) {
            player.SetDirectionalInput(Vector2.zero);
            player.velocity.y = force;
        }
        
    }
}
