using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
    public class SwitchCharacter : ToolKitAction {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Game object which start the interaction"></param>
    public override void Execute(GameObject colliding, GameObject interactionController) {
        CameraFollow cf = Camera.main.GetComponent<CameraFollow>();
        if (cf == null)
            return;

        Controller2D c2d = interactionController.GetComponent<Controller2D>();
        if (c2d == null)
            return;
        if (cf.target.gameObject == colliding)
            c2d.StartCoroutine(SwitchCharacterDelayed(cf, colliding, interactionController));
        else if (cf.target.gameObject == interactionController)
            c2d.StartCoroutine(SwitchCharacterDelayed(cf, interactionController, colliding));
    }

    IEnumerator SwitchCharacterDelayed(CameraFollow cf, GameObject active, GameObject inactive) {
        yield return null;
        PlayerInput pActive = active.GetComponent<PlayerInput>();
        pActive.Stop();

        PlayerInput pInactive = inactive.GetComponent<PlayerInput>();
        pInactive.Resume();

        cf.target = pInactive.GetComponent<Controller2D>();
    }
}
