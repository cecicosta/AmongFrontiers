using UnityEngine;
using System.Collections;

[System.Serializable]
public class PathAction : ToolKitAction {

	public override void Execute( GameObject transform ){}
	public override bool isStarted(){
		return true;
	}
	public override bool isFinished(){
		return true;
	}

}
