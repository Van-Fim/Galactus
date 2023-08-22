using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public TMPro.TMP_Text speed;
    public static CanvasController singleton;
    void Awake()
    {
        speed.text = "";
        singleton = this;
    }
}
