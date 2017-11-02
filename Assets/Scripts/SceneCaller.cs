using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneCaller : MonoBehaviour {

    public UnityEvent onPause;
    public UnityEvent onResume;

	public void GotoScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void PauseScene() {
        Time.timeScale = 0;
    }

    public void ResumeScene() {
        Time.timeScale = 1;
    }

    public void Pause() {
        Time.timeScale = 0;
    }

    public void Resume() {
        Time.timeScale = 1;
    }

    public void Exit() {
        Application.Quit();
    }
}
