using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class DialogBox : Singleton<DialogBox> {
    public Image dialogBox;
    public Text text;

    /// <summary>
    /// Characters per seccond
    /// </summary>
    public float renderVelocity = 10;
    public RectTransform anchor;
    public string textToRender;
    private bool renderFinished = true;
    private bool waitingInput;
    private string renderedText;
    public GameObject container;
    private bool speedUpRendering;
    private RectTransform rectTransform;
    private RectTransform parentRect;

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RenderTextImmediatelly() {
        speedUpRendering = true;
    }

    public void StartRenderText(Speecher speecher, string textToRender) {
        if (renderFinished == false)
            return;
        //Conlider is mandatory for speecher
        BoxCollider2D colider = speecher.GetComponent<BoxCollider2D>();

        Vector2 scrCenter = Camera.main.WorldToScreenPoint(colider.bounds.center);
        Vector2 scrMin = Camera.main.WorldToScreenPoint(colider.bounds.min);
        Vector2 scrMax = Camera.main.WorldToScreenPoint(colider.bounds.max);

        if (rectTransform == null) {
            rectTransform = GetComponent<RectTransform>();
        }
        if (parentRect == null) {
            parentRect = transform.parent.GetComponent<RectTransform>();
        }

        Vector2 boxPosition = new Vector2(0,0);
        if (anchor != null) {
            //anchor.localPosition = new Vector2(scrCenter.x, scrMax.y);
            boxPosition = new Vector2(0, anchor.rect.height);
        }

        //Adjust by the parent reference center point
        Vector2 parentReferenceCenter = new Vector2(parentRect.pivot.x* parentRect.rect.width, parentRect.pivot.x* parentRect.rect.height);

        //Adjust by its own center
        Vector2 referenceCenter = new Vector2(rectTransform.pivot.x * rectTransform.rect.width, rectTransform.pivot.x * rectTransform.rect.height);
        Vector2 centerAdjust = new Vector2(referenceCenter.x - rectTransform.rect.width / 2, referenceCenter.y);

        transform.localPosition = boxPosition + new Vector2(scrCenter.x - parentReferenceCenter.x + centerAdjust.x, 
                                                            scrMax.y - parentReferenceCenter.y + centerAdjust.y);

        renderFinished = false;
        string[] words = textToRender.Split(' ');
        StartCoroutine(RenderTextAnimate(words));
    }

    public void StartRenderText(string textToRender) {
        if (renderFinished == false)
            return;
        renderFinished = false;
        string[] words = textToRender.Split(' ');
        StartCoroutine(RenderTextAnimate(words));
    }

    public void ContinueRenderText() {
        renderedText = "";
    }

    public bool IsRenderFinished() {
        return renderFinished;
    }

    public bool IsWaitingInput() {
        return waitingInput;
    }

    public void SetVisible(bool visible) {
        container.SetActive(visible);
    }

    private IEnumerator RenderTextAnimate(string[] words) {
        RectTransform textArea = text.GetComponent<RectTransform>();
        TextGenerationSettings settings = text.GetGenerationSettings(textArea.rect.size);
        TextGenerator generator = new TextGenerator();
        settings.horizontalOverflow = HorizontalWrapMode.Wrap;
        settings.generateOutOfBounds = false;

        renderedText = "";
        
        float time = Time.time;
        int index = 0;

        while (index < words.Length) {
            string currentWord = words[index];
            float secondsPerChar = 1 / renderVelocity;

            generator.Populate(renderedText + currentWord, settings);
            float w = generator.GetPreferredWidth(renderedText + currentWord, settings);
            float h = generator.GetPreferredHeight(renderedText + currentWord, settings);

            //check if the amount of text already reach the bounds limit of the text area
            if (w * h > textArea.rect.width * textArea.rect.height) {
                waitingInput = true;
                speedUpRendering = false; //Stop "speed up" when text reach limit
                yield return null;
                continue;
            }else {
                waitingInput = false;
            }

            //Render text without velocity restriction
            if(speedUpRendering) {
                secondsPerChar = -1;
            }

            //Render each character according to the render velocity
            for (int i = 0; i < currentWord.Length; i++) {
                renderedText += currentWord.Substring(i, 1);
                time = Time.time;
                text.text = renderedText;
                while (Time.time - time < secondsPerChar) {
                    yield return null;
                }
            }

            renderedText += " ";
            index++;
            
            yield return null;
        }
        speedUpRendering = false;
        renderFinished = true;
    }
}
