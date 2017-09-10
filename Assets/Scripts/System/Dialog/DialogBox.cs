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

    public string textToRender;
    private bool renderFinished = true;
    private bool waitingInput;
    private string renderedText;
    public GameObject container;
    private bool speedUpRendering;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RenderTextImmediatelly() {
        speedUpRendering = true;
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
