using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FaceTarget: MonoBehaviour {

	public Transform toTrack;	
    
    public bool copyPosition = false;
    public bool copyRotation = false;
    public bool copyScale = false;
    private Quaternion baseRotation;

    // Use this for initialization
    void Start() {
        baseRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate() {

        if (toTrack != null && copyPosition && toTrack.gameObject.activeInHierarchy) {
            transform.position = toTrack.position;
        }

        if (toTrack != null && copyRotation && toTrack.gameObject.activeInHierarchy) {
            transform.rotation = toTrack.rotation;
        }

        if (toTrack != null && copyScale && toTrack.gameObject.activeInHierarchy) {
            transform.localScale = toTrack.localScale;
        }

        if (toTrack != null && toTrack.gameObject.activeInHierarchy)
            LookAt();
    }

    void LookAt() {
        transform.rotation = baseRotation * Quaternion.LookRotation(-transform.forward, Vector3.Cross(transform.forward, transform.position - toTrack.position));
    }

    public void LookAt(Transform target) {
        transform.rotation = baseRotation * Quaternion.LookRotation(-transform.forward, Vector3.Cross(transform.forward, transform.position - target.position));
    }
}