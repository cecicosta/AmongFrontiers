using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NotifyActivateToogle : MonoBehaviour {
    public bool perFrame = false;
    [SerializeField]
    OnActivateToogle onActivateToogle;
    [SerializeField]
    OnActivateToogle onActivateToogleInv;

    
    private void OnEnable() {
        onActivateToogle.Invoke(true);
        onActivateToogleInv.Invoke(false);
    }

    private void OnDisable() {
        onActivateToogle.Invoke(false);
        onActivateToogleInv.Invoke(true);
    }

    private void Update() {
        if (!perFrame)
            return;

        onActivateToogle.Invoke(gameObject.activeInHierarchy);
        onActivateToogleInv.Invoke(!gameObject.activeInHierarchy);
    }
}
