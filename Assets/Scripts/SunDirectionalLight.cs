using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class SunDirectionalLight : MonoBehaviour
{
    Light singleton;
    public void Awake()
    {
        singleton = gameObject.GetComponent<Light>();
    }
    public Color32 Color
    {
        get => singleton.color; set
        {
            singleton.color = value;
        }
    }

    public float Intensity
    {
        get => singleton.intensity; set
        {
            singleton.intensity = value;
        }
    }
}
