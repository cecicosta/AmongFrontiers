using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BackgroundController : MonoBehaviour {

    public string pathFolder;

    public Texture2D[] preLoaded = new Texture2D[0];
    public int currentPreLoaded;

    

    private RawImage background;
    private void Start() {
        background = GetComponent<RawImage>();
        background.texture = preLoaded.Length - 1 > currentPreLoaded ? preLoaded[currentPreLoaded] : background.texture; 
    }

    public void LoadBackground(string name) {
        Texture2D bg = Resources.Load<Texture2D>(pathFolder + "/" + name);
        if (bg == null)
            return;

        Texture tmp = background.mainTexture;
        background.texture = bg;
        Resources.UnloadAsset(tmp);
    }

    public void Next() {
        currentPreLoaded = currentPreLoaded + 1 >= preLoaded.Length ? currentPreLoaded : currentPreLoaded + 1;
        background.texture = preLoaded[currentPreLoaded];
    }

    public void Back() {
        currentPreLoaded = currentPreLoaded - 1 < 0 ? currentPreLoaded : currentPreLoaded - 1;
        background.texture = preLoaded[currentPreLoaded];
        
    }
}
