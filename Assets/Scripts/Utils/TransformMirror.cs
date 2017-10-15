using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TransformMirror : MonoBehaviour {

	public Transform toTrack;	
    
    public bool copyPosition = false;
    public bool copyRotation = false;
    public bool copyScale = false;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void LateUpdate() {

        if (copyPosition && toTrack.gameObject.activeInHierarchy) {
            transform.position = toTrack.position;
        }

        if (copyRotation && toTrack.gameObject.activeInHierarchy) {
            transform.rotation = toTrack.rotation;
        }

        if (copyScale && toTrack.gameObject.activeInHierarchy) {
            transform.localScale = toTrack.localScale;
        }

    }
}