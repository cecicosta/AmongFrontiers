using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSortingLayer : MonoBehaviour {
    [HideInInspector]
    public int sortingLayer;
    [HideInInspector]
    public int orderInLayer;
    ParticleSystem ps;

    private void OnValidate() {
        ps = GetComponent<ParticleSystem>();
        ps.GetComponent<Renderer>().sortingLayerID = sortingLayer;
        ps.GetComponent<Renderer>().sortingOrder = orderInLayer;
    }
}
