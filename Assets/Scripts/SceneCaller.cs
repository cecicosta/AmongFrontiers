using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCaller : MonoBehaviour {

	public void GotoScene(string name) {
        Application.LoadLevel(name);
    }

    public void PauseScene() {
        Time.timeScale = 0;
    }

    public void ResumeScene() {
        Time.timeScale = 1;
    }

    public void Exit() {
        Application.Quit();
    }
}
