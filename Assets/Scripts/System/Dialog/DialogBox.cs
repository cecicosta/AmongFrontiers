using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : Singleton<DialogBox> {
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
    private RectTransform rectTransform;
    private RectTransform parentRect;

    public delegate void OnOptionChoose(int id);
    public OnOptionChoose onOptionChoose;
    private int numberOfOptions;
    private bool phraseFinished = false;
    public bool autoSeparatePonctuation = true;
    private Speaker currentSpeaker;

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();

        foreach (UnityEngine.UI.Button b in buttons) {
            buttonsText.Add(b.GetComponentInChildren<Text>());
            b.onClick.AddListener(()=>{ ChooseOption(buttons.IndexOf(b)); });
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetNumberOfOptions(int numberOfOptions) {
        this.numberOfOptions = numberOfOptions;
        foreach (UnityEngine.UI.Button b in buttons) {
            if(buttons.IndexOf(b) >= numberOfOptions) {
                b.gameObject.SetActive(false);
            } else {
                b.gameObject.SetActive(true);
            }
        }
    }

    public void SetOption(int id, string text) {
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
        StartCoroutine(RenderTextPerPhrases(textToRender));
    }

    public void StartRenderText(string textToRender) {
        if (renderFinished == false)
            return;
        renderFinished = false;
        StartCoroutine(RenderTextPerPhrases(textToRender));
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

            StartCoroutine(RenderTextAnimate(words));

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
