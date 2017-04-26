using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class DialogShelf: ScriptableObject{
	
	public List<Texture2D> frameTexture;
	public List<Rect> frameRect;
	public List<Rect> textRect;
	public GUIStyle windowStyle;

	public List<DialogSet> layers;
	public int selectedLayer = 0;
	void OnEnable(){
		//if already exists or serialized avoid multiple instantiation to avoid memory leak
		if (layers == null){
			layers = new List<DialogSet>();
		}
	}
}