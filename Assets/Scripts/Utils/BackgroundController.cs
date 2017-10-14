using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BackgroundController : MonoBehaviour {

    public string pathFolder;

    private RawImage background;
    private void Start() {
        background = GetComponent<RawImage>();        
    }

    public void LoadBackground(string name) {
        Texture2D bg = Resources.Load<Texture2D>(pathFolder + "/" + name);
        if (bg == null)
            return;

        Texture tmp = background.mainTexture;
        background.texture = bg;
        Resources.UnloadAsset(tmp);
    }
}
