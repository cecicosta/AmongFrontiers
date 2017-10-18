using UnityEngine;
using System.Collections;



public class DelayableSetActive : MonoBehaviour {

    private Coroutine activateCoroutine;
    private Coroutine deactivateCoroutine;
    public float defaultDelayTime = 0.5f;

    /// <summary>
    /// Sets the active.
    /// </summary>
    /// <param name="value">If set to <c>true</c> value.</param>
    /// <param name="delayTime">Delay time.</param>
    public void SetActive(bool value, float delayTime = 0.0f) {

        if (value) {
            if (deactivateCoroutine != null) {
                StopCoroutine(deactivateCoroutine);
                deactivateCoroutine = null;
            }

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(value);
        } else {

            if (gameObject.activeInHierarchy && delayTime == 0.0f) {
                gameObject.SetActive(value);
                return;
            }

            if (gameObject.activeInHierarchy && deactivateCoroutine == null)
                deactivateCoroutine = StartCoroutine(DeactivateGameObject(delayTime));
        }
    }

    public void SetActive(bool value) {

        if (value) {
            if (deactivateCoroutine != null) {
                StopCoroutine(deactivateCoroutine);
                deactivateCoroutine = null;
            }

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(value);
        } else {

            if (gameObject.activeInHierarchy && defaultDelayTime == 0.0f) {
                gameObject.SetActive(value);
                return;
            }

            if (gameObject.activeInHierarchy && deactivateCoroutine == null)
                deactivateCoroutine = StartCoroutine(DeactivateGameObject(defaultDelayTime));
        }
    }

    private IEnumerator ActivateGameObject(float delayTime) {
        Debug.Log("ActivateGameObject start");

        yield return new WaitForSeconds(delayTime);

        gameObject.SetActive(true);
        activateCoroutine = null;

        Debug.Log("ActivateGameObject end");
    }

    private IEnumerator DeactivateGameObject(float delayTime) {

        yield return new WaitForSeconds(delayTime);

        gameObject.SetActive(false);
        deactivateCoroutine = null;

    }
}