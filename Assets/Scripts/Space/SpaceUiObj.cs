using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpaceUiObj : MonoBehaviour
{
    public Image image;
    public Space space;

    public static Color32 defcolor = new Color32(100, 100, 100, 100);
    public static Color32 curSysColor = new Color32(100, 100, 255, 255);
    public static Color32 selcolor = new Color32(100, 255, 100, 255);
    public static bool canShow = false;
    public bool selected = false;

    public static UnityAction OnRenderAction;

    public void Init()
    {
        OnRenderAction += OnRender;
        image.color = defcolor;
    }

    public void OnRender()
    {

    }
}
