﻿using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ActionManager : Singleton<ActionManager> {

	private List<string> avaiableActions = new List<string>();
	public List<string> AvaiableActions{
		get{ return avaiableActions; }
	}
	//[HideInInspector]
	[SerializeField]
	private List<ToolKitAction> actions = new List<ToolKitAction> ();
	public List<ToolKitAction> Actions{
		get{ return actions; }
	}
	//public GenericAction[] action; 
	// Use this for initialization
	void Start () {
		ListActionScripts ();
	}

	void ListActionScripts(){
		DirectoryInfo dir = new DirectoryInfo( Application.dataPath + ProjectPaths.ACTION_SCRIPTS);
		FileInfo[] info = dir.GetFiles("*.cs");
		avaiableActions.Clear();

		foreach (FileInfo f in info) {
			string scriptName = f.Name.Substring(0, f.Name.IndexOf("."));
			avaiableActions.Add(scriptName);
		}
	}
	/*
	void ListActionAssets(){
		DirectoryInfo dir = new DirectoryInfo( Application.dataPath + ProjectPaths.ACTION_DATA);
		FileInfo[] info = dir.GetFiles("*" + ProjectPaths.ACTION_EXTENSION );
		actions.Clear();
		
		foreach (FileInfo f in info) {
			string scriptName = f.Name.Substring(0, f.Name.IndexOf("."));
			avaiableActions.Add(scriptName);
		}
	}*/

	void OnValidate(){
		ListActionScripts ();
	}

	// Update is called once per frame
	void Update () {
		//GenericAction action = actions [0];

	}

	public ToolKitAction CreateActionAsset(string script){

        System.Type type = System.Type.GetType(script);
        ToolKitAction action = (ToolKitAction)System.Activator.CreateInstance(type);

        //ToolKitAction action = (ToolKitAction)ScriptableObject.CreateInstance( script );
		return action;
	}
	public void SaveActionAsset(ToolKitAction action){
#if UNITY_EDITOR
		//string path = Application.dataPath + ProjectPaths.ACTION_DATA + "/" + action.action + ProjectPaths.ACTION_EXTENSION;
		//path = path.Substring( Application.dataPath.Length - 6 );
		//AssetDatabase.CreateAsset(action, path);
		actions.Add (action);
		//AssetDatabase.SaveAssets();
#endif
	}
}
