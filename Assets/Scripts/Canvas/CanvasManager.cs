using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager
{
    public static CanvasController canvas;
    
    public static void Init()
    {
        canvas = GameObject.Instantiate(GameContentManager.canvasPrefab);
        GameObject.DontDestroyOnLoad(CanvasManager.canvas);
    }
}
