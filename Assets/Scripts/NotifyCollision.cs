using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NotifyCollision : MonoBehaviour {
    [System.Serializable]
    public class OnValueChangeBool : UnityEngine.Events.UnityEvent<bool> { }

    public List<Collider2D> collideWith;
    public LayerMask layers;

    public OnValueChangeBool onIsCollidingNotify;
    public UnityEngine.Events.UnityEvent onCollisionEnterNotify;
    public UnityEngine.Events.UnityEvent onCollisionStayNotify;
    public UnityEngine.Events.UnityEvent onCollisionExitNotify;

    private int collidingState;
	
	// Update is called once per frame
	void Update () {
        if (collidingState == 0)
            onIsCollidingNotify.Invoke(false);
        if (collidingState == 1)
            onIsCollidingNotify.Invoke(true);
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collideWith.Count > 0 && !collideWith.Contains(collision))
            return;
        if ((layers.value & (1 << collision.gameObject.layer)) == 0)
            return;

        collidingState = 1;
        onCollisionEnterNotify.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collideWith.Count > 0 && !collideWith.Contains(collision))
            return;
        if ((layers.value & (1 << collision.gameObject.layer)) == 0)
            return;
        collidingState = 1;
        onCollisionStayNotify.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collideWith.Count > 0 && !collideWith.Contains(collision))
            return;
        if ((layers.value & (1 << collision.gameObject.layer)) == 0)
            return;
        collidingState = 0;
        onCollisionExitNotify.Invoke();
    }
}
