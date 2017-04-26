using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimAction : ToolKitAction {
	public Motion animation;
	public int layer;

	public override void Execute( GameObject animator ){}
	public override bool isStarted(){
		return false;
	}
	public override bool isFinished(){
		return false;
	}

}
