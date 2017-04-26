using UnityEngine;
using System.Collections;

public class CharacterPlatformerController : MonoBehaviour {

	Animator animator;
	public float walkSpeed = 10;
	public float walkVelocity = 5;
	public float friction = 0.1f;
	[HideInInspector]
	public float speed = 0;
	private bool flip = false;
	private bool still = false;
	private bool chasing = false;
	private Vector2 target;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 position = InterfaceManager.cursorWorldPosition;
		//Control character direction using mouse or touth position
		if( InterfaceManager.worldPressed){
			//Correct character flickering direction when the cursor moves right-left or vice-versa
			if( still ){
				if( speed > 0 && transform.position.x - 0.2 < position.x )
					speed = 1;
				else if( transform.position.x > position.x )
					speed = -1;
				else if( speed < 0 && transform.position.x + 0.2 > position.x )
					speed = -1;
				else if( transform.position.x < position.x )
					speed = 1;

			}else{
				if( transform.position.x + 0.5f < position.x ){
					speed = 1;
					still = true;
				}
				else if( transform.position.x - 0.5f > position.x ){
					speed = -1;
					still = true;
				}
			}
			chasing = false;

		}else if( Mathf.Abs( Input.GetAxis("Horizontal") ) > 0 ) {
			still = false;
			speed = Input.GetAxis("Horizontal");
			chasing = false;
		}else if( chasing ){
			if( Mathf.Abs( transform.position.x - target.x ) > 0.2 )
				speed = Mathf.Sign(target.x - transform.position.x );
			else
				chasing = false;
				
		}else 
			speed = 0;

	
		if( speed > 0.1f ){
			GetComponent<Rigidbody2D>().AddForce( (new Vector2(1,0))*walkSpeed*GetComponent<Rigidbody2D>().mass  );
			if( flip ){
				Flip();
			}

		}else if( speed < -0.1f ){
			GetComponent<Rigidbody2D>().AddForce( (new Vector2(-1,0))*walkSpeed*GetComponent<Rigidbody2D>().mass );
			if( !flip ){
				Flip();
			}
		}

		if( Mathf.Abs( GetComponent<Rigidbody2D>().velocity.x ) > walkVelocity  ){
			GetComponent<Rigidbody2D>().AddForce( (new Vector2(-1,0))*walkSpeed*GetComponent<Rigidbody2D>().mass*Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) );
		}

		if( Mathf.Abs( GetComponent<Rigidbody2D>().velocity.x ) > 0.1f ){
			GetComponent<Rigidbody2D>().AddForce( new Vector2( -GetComponent<Rigidbody2D>().mass*GetComponent<Rigidbody2D>().gravityScale*friction*Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x), 0) );
		}

		animator.SetFloat("speed", GetComponent<Rigidbody2D>().velocity.x );

	}

	public void MoveTo( Vector2 position ){
		chasing = true;
		target = position;
	}

	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		flip = !flip;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
