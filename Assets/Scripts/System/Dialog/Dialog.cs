using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Dialog: ScriptableObject{
	[SerializeField]
	//General Properties
	public int id;
	public bool updateRoot = false;
	public string characterIdentifier = "";
	public string dialogTag = "";
	public int selectedQuery = 0;
	public string text = "";
    public bool isTrigger = false;
    [SerializeField]
    public Condition toTrigger;
	public List<string> query = new List<string>();
	public List<int> parents = new List<int>();
	public List<int> children = new List<int>();
	
	//Editor properties
	public Rect EditorWindowRect = new Rect (100, 100, 200, 72);
	
}
