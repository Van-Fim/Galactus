using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Color32 color;
    void Start()
    {
        meshRenderer.material.SetColor("_TintColor", (Color)color);
    }
}
