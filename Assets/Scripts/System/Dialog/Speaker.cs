using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Speaker: MonoBehaviour {

	public string character;
	public string identifier;
	public UnityEngine.UI.Image face;

    public UnityEngine.Events.UnityEvent OnDialogStart;
    public UnityEngine.Events.UnityEvent OnDialogEnd;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Talk(){
	}

	public void OnDialogStartNotify( string tag ){
        OnDialogStart.Invoke();
	}

	public void OnDialogEndNotify( string tag ){
        OnDialogEnd.Invoke();
	}

    public void SetFaceActive(bool active) {
        if (face != null)
            face.gameObject.SetActive(active);
    }
}
