using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentCharacterBackup : Singleton<CurrentCharacterBackup> {
    private Vector3 position;
    private string identifier;
    private bool hasBackup;
    private int buildIndex;

    private Dictionary<string, Vector3> characters = new Dictionary<string, Vector3>();
    public void BackupCurrentCharacter() {
        DontDestroyOnLoad(gameObject);

        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput p in playerInputs) {
            characters.Add(p.GetComponent<Speaker>().identifier, p.transform.position);
        }

        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        Controller2D characterController = cameraFollow.target;
        Speaker character = characterController.GetComponent<Speaker>();
        identifier = character.identifier;

        hasBackup = true;
        buildIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void RestoreCharacterBackup() {
        if (!hasBackup)
            return;
        
        Speaker[] speakers = FindObjectsOfType<Speaker>();
        Speaker character = Array.Find<Speaker>(speakers, x => x.identifier == identifier);

        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach(PlayerInput p in playerInputs) {
            string pIdentifier = p.GetComponent<Speaker>().identifier;
            p.transform.position = characters[pIdentifier];
            if (pIdentifier == identifier)
                p.Resume();
            else
                p.Stop();
        }

        Controller2D target = character.GetComponent<Controller2D>();
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        cameraFollow.target = target;
    }

    public void InvalidateBackup() {
        hasBackup = false;
    }

    void OnEnable() {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if(hasBackup && buildIndex == scene.buildIndex) {
            RestoreCharacterBackup();
        }
    }
}
