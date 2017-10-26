using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour {
    public CameraFollow cameraFollow;


	public void SwitchCurrentCharacter(Controller2D switchTo) {

        PlayerInput current = cameraFollow.target.GetComponent<PlayerInput>();
        if (current != null)
            current.Stop();
        PlayerInput newTarget = switchTo.GetComponent<PlayerInput>();
        if (newTarget != null)
            newTarget.Resume();

        switchTo.transform.position = cameraFollow.target.transform.position;

        cameraFollow.target = switchTo;
    }
}
