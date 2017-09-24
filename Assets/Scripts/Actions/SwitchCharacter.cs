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

        Controller2D c2dInter = interactionController.GetComponentInParent<Controller2D>();
        Controller2D c2dCol = colliding.GetComponentInParent<Controller2D>();

        if (c2dInter == null || c2dCol == null)
            return;

        if (cf.target == c2dCol)
            c2dCol.StartCoroutine(SwitchCharacterDelayed(cf, c2dCol, c2dInter));
        else if (cf.target.gameObject == interactionController)
            c2dInter.StartCoroutine(SwitchCharacterDelayed(cf, c2dInter, c2dCol));
    }

    IEnumerator SwitchCharacterDelayed(CameraFollow cf, Controller2D active, Controller2D inactive) {
        yield return null;
        PlayerInput pActive = active.GetComponent<PlayerInput>();
        pActive.Stop();

        PlayerInput pInactive = inactive.GetComponent<PlayerInput>();
        pInactive.Resume();

        cf.target = pInactive.GetComponent<Controller2D>();
    }
}
