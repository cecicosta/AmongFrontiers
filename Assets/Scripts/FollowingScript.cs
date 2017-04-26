using UnityEngine;
using System.Collections;

public class FollowingScript : MonoBehaviour {

	public bool constraintX = false;
	public bool constraintY = false;
	public bool constraintZ = false;
	public Transform target;


	private Vector3 targetLastPosition;
	// Use this for initialization
	void Start () {
		targetLastPosition = target.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {


		Vector3 deltaPosition = (target.transform.position - targetLastPosition);
		Vector3 translate = new Vector3(0,0,0);
		if(!constraintX)
			translate.x = deltaPosition.x;
		if(!constraintY)
			translate.y = deltaPosition.y;
		if(!constraintZ)
			translate.z = deltaPosition.z;

		transform.Translate(translate);
	
		targetLastPosition = target.transform.position;

	}

}
