using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public enum Fade{IN, OUT};
	public Texture2D fadeInOutTexture;
	public float time;
	public Fade kind;

	private float alpha = 1.0f; 
	private int fadeDir = 1;
	// Use this for initialization
	void Start () {
		if( kind == Fade.OUT ){
			fadeDir = 1;
			alpha = 0;
		}else if( kind == Fade.IN ){
			fadeDir = -1;
			alpha = 1;
		}

	}
	
	// Update is called once per frame
	void OnGUI () {


		alpha +=  fadeDir * Time.deltaTime/time;	
		alpha = Mathf.Clamp01(alpha);	

		Color color = GUI.color;
		color.a = alpha;
		GUI.color = color;
		//GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeInOutTexture);


	}
}
