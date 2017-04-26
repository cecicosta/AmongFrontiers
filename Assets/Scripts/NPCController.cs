using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {
		
	Animator animator;
	public float walkSpeed = 10;
	public float walkVelocity = 5;
	public float friction = 0.1f;
	public float speed = 0;
	private bool flip = false;
	public Dialog dialog;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

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

		try{
		//	animator.SetFloat("speed", rigidbody2D.velocity.x );
		}catch(System.Exception e){}
	}

	public void Flip ()
	{
		print(name);
		// Switch the way the player is labelled as facing.
		flip = !flip;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		//theScale.x = -1*theScale.x;
		transform.localScale = theScale;
	}


}
