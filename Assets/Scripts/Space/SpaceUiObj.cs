using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpaceUiObj : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Space space;

    public static Color32 defcolor = new Color32(100, 100, 100, 100);
    public static Color32 curSysColor = new Color32(100, 100, 255, 255);
    public static Color32 selcolor = new Color32(100, 255, 100, 255);
    MapClientPanel mapClientPanel;
    Rect rawRect;
    public static bool canShow = false;
    public bool selected = false;

    public static UnityAction OnRenderAction;
    public static UnityAction OnDestroyAllAction;

    public void Init()
    {
        OnRenderAction += OnRender;
        OnDestroyAllAction += DestroyAll;
        image.color = defcolor;

        mapClientPanel = ClientPanelManager.GetPanel<MapClientPanel>();
        rawRect = mapClientPanel.rawImage.GetPixelAdjustedRect();
    }

    public void Destroy()
    {
        OnRenderAction -= OnRender;
        OnDestroyAllAction -= DestroyAll;
        image.color = defcolor;
        //DestroyImmediate(this);
        DestroyImmediate(this.gameObject);
    }
    public void DestroyAll()
    {
        Destroy();
    }
    public void OnRender()
    {
        Camera cam = CameraManager.minimapCamera.curCamera;
        if (!mapClientPanel.gameObject.activeSelf)
        {
            Destroy();
        }
        if (!CameraManager.minimapCamera.enabled)
        {
            return;
        }
        //float dist = Vector3.Distance(cam.transform.position, space.GetPosition());
        Vector3 spPos = space.GetPosition();
        if (space.GetType() == typeof(Sector))
        {
            spPos /= Sector.minimapDivFactor;
        }
        else if (space.GetType() == typeof(Zone))
        {
            spPos /= Zone.minimapDivFactor;
        }
        float thing1 = Vector3.Dot((spPos - cam.transform.position).normalized, cam.transform.forward);
        if (MapClientPanel.currentLayer != space.spaceController.layer)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
        }

        if (image.enabled && thing1 <= 0)
        {
            image.enabled = false;
        }
        else if (image.enabled && thing1 > 0)
        {
            image.enabled = true;
        }
        Vector3 spacePosition = space.GetPosition();
        if (space.GetType() == typeof(Sector))
        {
            spacePosition = spacePosition / Sector.minimapDivFactor;
        }
        else if (space.GetType() == typeof(Zone))
        {
            spacePosition = spacePosition / Zone.minimapDivFactor;
        }
        Vector3 pp = CameraManager.minimapCamera.curCamera.WorldToViewportPoint(spacePosition);

        pp.Scale(new Vector3(rawRect.width, rawRect.height, 1f));

        transform.position = new Vector3(pp.x, pp.y, 0);
        image.rectTransform.sizeDelta = new Vector2(30, 30);
        if (MapClientPanel.currentLayer == 0)
        {
            image.rectTransform.sizeDelta = new Vector2(100, 100);
        }
        else if (MapClientPanel.currentLayer > 1)
        {

        }
        if (space.GetType() == typeof(Galaxy))
        {
            if (space.id == Client.localClient.galaxyId)
            {
                image.color = curSysColor;
                if (space.id == MapClientPanel.selectedGalaxyId)
                {
                    image.color = selcolor;
                }
            }
            else if (space.id == MapClientPanel.selectedGalaxyId)
            {
                image.color = selcolor;
            }
            else
            {
                image.color = defcolor;
            }
        }
        else if (space.GetType() == typeof(StarSystem))
        {
            StarSystem ssp = (StarSystem)space;
            if (ssp.galaxyId == Client.localClient.galaxyId && ssp.id == Client.localClient.systemId)
            {
                image.color = curSysColor;
                if (ssp.galaxyId == MapClientPanel.selectedGalaxyId && ssp.id == MapClientPanel.selectedSystemId)
                {
                    image.color = selcolor;
                }
            }
            else if (ssp.galaxyId == MapClientPanel.selectedGalaxyId && ssp.id == MapClientPanel.selectedSystemId)
            {
                image.color = selcolor;
            }
            else
            {
                image.color = defcolor;
                if (!(ssp.galaxyId == MapClientPanel.selectedGalaxyId))
                {
                    Destroy();
                }
            }
        }
        else if (space.GetType() == typeof(Sector))
        {
            Sector ssp = (Sector)space;
            if (!(ssp.galaxyId == MapClientPanel.selectedGalaxyId && ssp.systemId == MapClientPanel.selectedSystemId))
            {
                Destroy();
            }
            if (ssp.galaxyId == Client.localClient.galaxyId && ssp.systemId == Client.localClient.systemId && ssp.id == Client.localClient.sectorId)
            {
                image.color = curSysColor;
                if (ssp.galaxyId == MapClientPanel.selectedGalaxyId && ssp.systemId == MapClientPanel.selectedSystemId && ssp.id == MapClientPanel.selectedSectorId)
                {
                    image.color = selcolor;
                }
            }
            else if (ssp.galaxyId == MapClientPanel.selectedGalaxyId && ssp.systemId == MapClientPanel.selectedSystemId && ssp.id == MapClientPanel.selectedSectorId)
            {
                image.color = selcolor;
            }
            else
            {
                image.color = defcolor;
            }
        }
        else if (space.GetType() == typeof(Zone))
        {
            Zone ssp = (Zone)space;
            if (!(ssp.galaxyId == MapClientPanel.selectedGalaxyId && ssp.systemId == MapClientPanel.selectedSystemId && ssp.sectorId == MapClientPanel.selectedSectorId))
            {
                Destroy();
            }
            if (ssp.galaxyId == Client.localClient.galaxyId && ssp.systemId == Client.localClient.systemId && ssp.sectorId == Client.localClient.sectorId && ssp.id == Client.localClient.zoneId)
            {
                image.color = curSysColor;
            }
            else if (ssp.galaxyId == MapClientPanel.selectedGalaxyId && ssp.systemId == MapClientPanel.selectedSystemId && ssp.sectorId == MapClientPanel.selectedSectorId && ssp.id == MapClientPanel.selectedZoneId)
            {
                image.color = selcolor;
            }
            else
            {
                image.color = defcolor;
            }
        }
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (space.GetType() == typeof(Galaxy))
        {
            MapClientPanel.selectedGalaxyId = space.id;
        }
        else if (space.GetType() == typeof(StarSystem))
        {
            MapClientPanel.selectedSystemId = space.id;
        }
        if (space.GetType() == typeof(Sector))
        {
            MapClientPanel.selectedSectorId = space.id;
        }
        if (space.GetType() == typeof(Zone))
        {
            MapClientPanel.selectedZoneId = space.id;
        }
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
