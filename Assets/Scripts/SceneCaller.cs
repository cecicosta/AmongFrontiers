using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCaller : MonoBehaviour {

	public void GotoScene(string name) {
        Application.LoadLevel(name);
    }
}
