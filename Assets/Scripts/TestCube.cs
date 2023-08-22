using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    [SerializeField] private Color32 color;

    public Color32 Color
    {
        get => color; set
        {
            color = value;
            meshRenderer.material.SetColor("_Color", (Color)Color);
        }
    }

    void Start()
    {
        Color = color;
    }
}
