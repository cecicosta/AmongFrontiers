using UnityEngine;
using System.Collections;

public class InterfaceManager : MonoBehaviour {

	public static Vector2 cursorWorldPosition;
	public static Vector2 cursorPosition;
	public static bool pressed = false;
	public static bool release = false;
	public static bool worldPressed = false;
	public static bool worldRelease = false;
	public static bool interfacePressed = false;
	public static bool interfaceRelease = false;
	// Use this for initialization
	void Start () {
	
	}
	
	void LateUpdate () {

		bool checkpressed = false;
		bool checkWorldPressed = false;
		bool checkInterfacePressed = false;

		if( pressed )
			checkpressed = true;
		if( worldPressed )
			checkWorldPressed = true;
		if( interfacePressed )
			checkInterfacePressed = true;

		release = false;
		worldRelease = false;
		interfaceRelease = false;

		pressed = false;
		worldPressed = false;
		interfacePressed = false;

		Vector3 position = Input.mousePosition;
		if( Input.GetMouseButton( 0 ) ){
			position = Input.mousePosition;
			pressed = true;
		}if( Input.touchCount > 0 ){
			position = Input.GetTouch(0).position;
			pressed = true;
		}
		
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(position), Vector2.zero, -999, 1<<8);
		cursorWorldPosition = Camera.main.ScreenToWorldPoint(position);
		cursorPosition = position;
		
		if( hit.collider != null ){
			Button button = hit.collider.GetComponent<Button>();
			button.OnFocusEvent();
			button.Focus = true;
			
			if( button.Enter == false ){
				button.OnEnterEvent();
				button.Enter = true;
			}
			interfacePressed = true;
		}else if( pressed ){
			worldPressed = true;
		}

		if( !pressed && checkpressed )
			release = true;
		if( !worldPressed && checkWorldPressed )
			worldRelease = true;
		if( !interfacePressed && checkInterfacePressed )
			interfaceRelease = true;
	}

	public static void UsePressedEvent(){
		pressed = false;
		worldPressed = false;
		interfacePressed = false;
	}

	public static void UseReleaseEvent(){
		release = false;
		worldRelease = false;
		interfaceRelease = false;
	}
}
