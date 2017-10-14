using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour {

    private Image toFade;
    private void Start() {
        toFade = GetComponent<Image>();
    }

    public void FadeIn(float time) {
        StartCoroutine(DoFade(time, true));
    }

    public void FadeOut(float time) {
        StartCoroutine(DoFade(time, false));
    }

    IEnumerator DoFade(float time, bool fadein) {
        float start = Time.time;
        Color color = new Color();
        while (Time.time - start < time) {
            color = toFade.color;
            color.a = Mathf.Lerp(fadein? 1:0, fadein? 0:1, (Time.time - start) / time);
            toFade.color = color;
            yield return null;
        }

        color = toFade.color;
        color.a = Mathf.Lerp(fadein ? 1 : 0, fadein ? 0 : 1, 1);
        toFade.color = color;
    }
}
