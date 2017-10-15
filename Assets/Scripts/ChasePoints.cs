using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePoints : MonoBehaviour {

    public AnimatedInput animatedInput;
    public Transform[] points;
    public bool loop = true;
    private int currentToChase = 0;

    private bool reverting = false;
    private Vector3 next;

    // Update is called once per frame
    void Update () {

        next = currentToChase >= 0 && currentToChase < points.Length? points[currentToChase].position: next;
        animatedInput.target = next;

        if (Mathf.Abs(transform.position.x - next.x) <= 0.2) {

            if (loop && reverting)
                reverting = currentToChase < 0 ? false : true;

            if (loop && !reverting)
                reverting = currentToChase >= points.Length? true : false;

            if (reverting) {
                currentToChase--;
            } else
                currentToChase++;
        }

	}
}
