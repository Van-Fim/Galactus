using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public static CanvasController singleton;
    public void Awake()
    {
        singleton = GetComponent<CanvasController>();
    }
}
