using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpaceUiObj : MonoBehaviour, IPointerClickHandler
{
    public Image image;

    public static Color32 defcolor = new Color32(100, 100, 100, 100);
    public static Color32 curSysColor = new Color32(100, 100, 255, 255);
    public static Color32 selcolor = new Color32(100, 255, 100, 255);
    Rect rawRect;
    public static bool canShow = false;
    public bool selected = false;

    public static SpaceUiObj selectedObj;

    public static int maxRenderDistance = 1000;
    public static UnityAction OnSelectAction;
    public static UnityAction OnRenderAction;
    public static UnityAction OnDestroyAllAction;
    bool destroyed;
    public Space space;

    public void Init()
    {
        OnSelectAction += OnSelect;
        OnRenderAction += OnRender;
        OnDestroyAllAction += DestroyAll;
        image.color = defcolor;
    }
    public void DestroyAll()
    {
        // Destroy();
    }
    public void OnSelect()
    {
        if (selectedObj == this)
        {
            selected = true;
        }
        else
        {
            selected = false;
        }
    }
    public void OnRender()
    {
        curSysColor = defcolor;
        Vector3 spPos = space.GetPosition();
        Vector3 pp = CameraManager.mapCamera.curCamera.WorldToScreenPoint(spPos);
        if (space is SpaceSystem)
        {
            SpaceSystem spaceSystem = (SpaceSystem)space;
            if (SpaceManager.singleton.currentGalaxy.id != spaceSystem.galaxyId)
            {
                gameObject.SetActive(false);
                return;
            }
        }
        transform.position = new Vector3(pp.x, pp.y, 0);
        float thing1 = Vector3.Dot((spPos - CameraManager.mapCamera.transform.position).normalized, CameraManager.mapCamera.transform.forward);
        if (thing1 <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        else if (thing1 > 0)
        {
            gameObject.SetActive(true);
        }
        float dist = Vector3.Distance(CameraManager.mapCamera.transform.position, spPos);
        if (dist > 300)
        {
            gameObject.SetActive(false);
            return;
        }
        image.rectTransform.sizeDelta = new Vector2(100, 100);
        if (selected)
        {
            curSysColor = selcolor;
        }
        image.color = curSysColor;
        if (!GameManager.canvasController.mainHud.isShow)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (space is SpaceSystem)
        {
            selectedObj = this;
            InvokeSelect();
        }
    }
    public static void InvokeSelect()
    {
        OnSelectAction?.Invoke();
    }
    public static void InvokeRender()
    {
        OnRenderAction?.Invoke();
    }
    public static void InvokeDestroyAll()
    {
        OnDestroyAllAction?.Invoke();
    }
}