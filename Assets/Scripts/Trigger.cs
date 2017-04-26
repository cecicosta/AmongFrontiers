using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

	public delegate void OnFlagChange(string flag, bool value);
	public event OnFlagChange onFlagChangeEvent;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TriggerFlag(string flag, bool value){
		try{
			onFlagChangeEvent (flag, value);
		}catch(System.Exception e){
			Debug.Log(e.Message + ": Triggering " + flag);		
		}
	}
}
