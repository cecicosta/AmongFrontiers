﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : Singleton<DialogBox> {
    public Camera mainCamera;
    public Image dialogBox;
    public Text text;
    public Text name;
    /// <summary>
    /// Characters per seccond
    /// </summary>
    public float renderVelocity = 10;
    public RectTransform anchor;
    public string textToRender;
    public GameObject container;
    
    [Tooltip("The text of each button have to be its child.")]
    public List<UnityEngine.UI.Button> buttons = new List<UnityEngine.UI.Button>();
    private List<Text> buttonsText = new List<Text>();
    private bool renderFinished = true;
    private bool waitingInput;
    private string renderedText;
    private bool speedUpRendering;
    private RectTransform boxRectTransform;
    private RectTransform canvasRect;

    public delegate void OnOptionChoose(int id);
    public OnOptionChoose onOptionChoose;
    private bool phraseFinished = false;
    public bool autoSeparatePonctuation = true;
    private Speaker currentSpeaker;
    public bool ignoreSpeakerPosition;
    private Coroutine renderPerPhraseRoutine;
    private Coroutine renderPerWordRoutine;
    public bool useScreenPosition;
    public bool copySpeakerRect;
    public bool UseScreenPosition {
        get {
            return useScreenPosition;
        }

        set {
            useScreenPosition = value;
        }
    }

    // Use this for initialization
    void Start () {
        boxRectTransform = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        foreach (UnityEngine.UI.Button b in buttons) {
            buttonsText.Add(b.GetComponentInChildren<Text>());
            b.onClick.AddListener(()=>{ ChooseOption(buttons.IndexOf(b)); });
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StopCurrentRendering() {
        StopAllCoroutines();
        waitingInput = false;
        renderFinished = true;
        HideAllOptions();
    }

    public void HideAllOptions() {
        foreach (UnityEngine.UI.Button b in buttons) {
                b.gameObject.SetActive(false);
        }
    }

    public void ShowOption(int id, string text) {
        if (!container.activeSelf)
            return;

        if (id >= buttons.Count || buttons[id] == null)
            return;
        buttons[id].gameObject.SetActive(true);

        if (id >= buttonsText.Count || buttonsText[id] == null)
            return;
        buttonsText[id].text = text;
    }

    public void HideOption(int id, string text = "") {
        if (id >= buttons.Count || buttons[id] == null)
            return;
        buttons[id].gameObject.SetActive(false);

        if (id >= buttonsText.Count || buttonsText[id] == null)
            return;
        buttonsText[id].text = text;
    }

    public void ChooseOption(int id) {
        if (onOptionChoose != null)
            onOptionChoose(id);
    }

    public void RenderTextImmediatelly() {
        speedUpRendering = true;
    }

    public void StartRenderText(Speaker speecher, string textToRender) {
        if (renderFinished == false)
            return;

        if (currentSpeaker != null)
            currentSpeaker.SetFaceActive(false);

        this.currentSpeaker = speecher;

        if (currentSpeaker != null)
            currentSpeaker.SetFaceActive(true);

        if (name != null)
            name.text = currentSpeaker.character;

        if(!ignoreSpeakerPosition)
            PositionDialogBox(speecher);

        renderFinished = false;
        renderPerPhraseRoutine = StartCoroutine(RenderTextPerPhrases(textToRender));
    }

    public void StartRenderText(string textToRender) {
        if (renderFinished == false)
            return;
        renderFinished = false;
        renderPerPhraseRoutine = StartCoroutine(RenderTextPerPhrases(textToRender));
    }

    private void PositionDialogBox(Speaker speecher) {
        //Conlider is mandatory for speecher
        BoxCollider2D collider = speecher.GetComponent<BoxCollider2D>();

        if (boxRectTransform == null) {
            boxRectTransform = GetComponent<RectTransform>();
        }
        if (canvasRect == null) {
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        //TODO: Improve this code
        if (copySpeakerRect) {
            boxRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, collider.GetComponent<RectTransform>().rect.size.x);
            boxRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, collider.GetComponent<RectTransform>().rect.size.y);
            transform.position = collider.transform.position;
            return;

        } else if (useScreenPosition) {
            Vector3 canvasScale = canvasRect.localScale;
            Vector3 speakerHeightExtent = new Vector3(0, collider.GetComponent<RectTransform>().rect.size.y / 2 * canvasScale.y, 0);
            Vector3 boxHeightExtent = new Vector3(0, boxRectTransform.rect.size.y / 2 * canvasScale.y, 0);
            Vector3 scrCenter = collider.transform.position + boxHeightExtent + speakerHeightExtent;
            transform.position = scrCenter;
            return;

        } else {

            Vector3 targetPosition = mainCamera.WorldToScreenPoint(collider.bounds.center);
            targetPosition.Scale(canvasRect.localScale);

            Vector3 targetExtents = (mainCamera.WorldToScreenPoint(collider.bounds.max) - mainCamera.WorldToScreenPoint(collider.bounds.min)) / 2;
            targetExtents.Scale(canvasRect.localScale);

            Vector3 boxExtents = boxRectTransform.rect.size/2;
            boxExtents.Scale(canvasRect.localScale);
            Vector3 canvasExtents = canvasRect.rect.size / 2;
            canvasExtents.Scale(canvasRect.localScale);

            Vector2 arrowSize = new Vector2(0, 0);
            if (anchor != null) {
                arrowSize = anchor.rect.size;
                arrowSize.Scale(canvasRect.localScale);
            }

            transform.position = canvasRect.position + targetPosition - canvasExtents + new Vector3(0, targetExtents.y, 0) + new Vector3(0, boxExtents.y, 0) + new Vector3(0, arrowSize.y, 0);
            return;
        }
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
        if (!visible)
            HideAllOptions();
    }

    private IEnumerator RenderTextPerPhrases(string textToRender) {

        waitingInput = false;
        string[] phrases = new string[] { textToRender };
        if (autoSeparatePonctuation) {
            phrases = SeparateByPonctuation(ref textToRender);
        }
        foreach (string s in phrases) {
            if (s == "")
                continue;

            string[] words = s.Split(' ');
            phraseFinished = false;

            while (waitingInput && renderedText != "") yield return null;

            renderPerWordRoutine = StartCoroutine(RenderTextAnimate(words));

            yield return new WaitUntil(() => { return phraseFinished; });
            waitingInput = true;
        }
        waitingInput = false;
        renderFinished = true;

    }

    private static string[] SeparateByPonctuation(ref string textToRender) {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\r|\n");
        textToRender = regex.Replace(textToRender, "");
        string[] phrases = System.Text.RegularExpressions.Regex.Split(textToRender, "(?<=[?!.]{1,} )");
        return phrases;
    }

    private IEnumerator RenderTextAnimate(string[] words) {
        RectTransform textArea = text.GetComponent<RectTransform>();
        TextGenerationSettings settings = text.GetGenerationSettings(textArea.rect.size);
        TextGenerator generator = new TextGenerator();
        settings.horizontalOverflow = HorizontalWrapMode.Wrap;
        settings.verticalOverflow = VerticalWrapMode.Truncate;
        settings.generateOutOfBounds = false;
        speedUpRendering = false;
        renderedText = "";
        
        float time = Time.time;
        int index = 0;

        while (index < words.Length) {
            string currentWord = words[index];
            float secondsPerChar = 1 / renderVelocity;

            generator.Populate(renderedText + currentWord, settings);
            float w = generator.GetPreferredWidth(renderedText + currentWord + " ", settings);
            float h = generator.GetPreferredHeight(renderedText + currentWord + " ", settings);

            //check if the amount of text already reach the bounds limit of the text area
            if (w > textArea.rect.width && h > textArea.rect.height) {
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
        phraseFinished = true;
    }
}
