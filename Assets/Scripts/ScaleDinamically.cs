using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleDinamically : MonoBehaviour {
    public bool invert = false;

    public void ScaleNormalizedX(float amount) {
        transform.localScale = new Vector3((invert ? -1 : 1) * amount, transform.localScale.y, transform.localScale.z);
    }
    public void ScaleNormalizedY(float amount) {
        transform.localScale = new Vector3(transform.localScale.x, (invert ? -1 : 1) * amount, transform.localScale.z);
    }
    public void ScaleNormalizedZ(float amount) {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (invert ? -1 : 1) * amount);
    }

}
