using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameVariablesManager : Singleton<GameVariablesManager> {
	public List<Variable> gameVariables = new List<Variable>();
	public List<Variable> sceneVariables = new List<Variable>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public List<Variable> getAllVariables(){
		List<Variable> variables = new List<Variable> (GameVariablesManager.Instance.gameVariables.ToArray());
		variables.AddRange (GameVariablesManager.Instance.sceneVariables.ToArray());
		return variables;
	}
}
