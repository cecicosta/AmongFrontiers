using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public string identifier = "buttom";
	public TextMesh text;


	bool focus = false;
	public bool Focus{
		set{ focus = value; }
		get{ return focus; }
	}
	bool enter = false;
	public bool Enter{
		set{ enter = value; }
		get{ return enter; }
	}

	void OnValidate(){
		if( text != null ){
			text.GetComponent<Renderer>().sortingLayerID = GetComponent<Renderer>().sortingLayerID;
			text.GetComponent<Renderer>().sortingOrder = GetComponent<Renderer>().sortingOrder+1;
			text.GetComponent<Renderer>().sortingLayerName = GetComponent<Renderer>().sortingLayerName;
		}
	}

	private bool pressed = false;
	private bool mouseOver = false;

	// Event Handler
	public delegate void OnClick(string identifier);
	public event OnClick OnClickEvent;
	
	Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();

		if( text != null ){
			text.GetComponent<Renderer>().sortingLayerID = GetComponent<Renderer>().sortingLayerID;
			text.GetComponent<Renderer>().sortingOrder = GetComponent<Renderer>().sortingOrder+1;
			text.GetComponent<Renderer>().sortingLayerName = GetComponent<Renderer>().sortingLayerName;
		}

	}
	
	// Update is called once per frame
	void Update(){
		if( focus == false ){
			if(enter == true)
				OnLeaveEvent();
			enter = false;
		}
		focus = false;

		if(  Mathf.Abs( Input.GetAxis("Fire1") ) > 0.1f || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began ){

			pressed = true;
		}
		if( Mathf.Abs( Input.GetAxis("Fire1") ) <= 0.1f && pressed || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended ){

			pressed = false;
			if( mouseOver ){
				OnClickEvent(identifier);
			}
		}
	}
	

	//Called once when the controller is in focus
	public void OnEnterEvent (){
		animator.SetBool("focus", true );
		mouseOver = true;
	}
	//Called each frame when the controller is in focus
	public void OnFocusEvent (){

	}
	//Called once when the controller is no more in focus
	public void OnLeaveEvent () {
		animator.SetBool("focus", false );
		mouseOver = false;
	}

}
