using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameContent;
public class SpaceUiObj : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public GameContent.Space space;

    public static Color32 defcolor = new Color32(100, 100, 100, 100);
    public static Color32 curSysColor = new Color32(100, 100, 255, 255);
    public static Color32 selcolor = new Color32(100, 255, 100, 255);
    MapClientPanel mapClientPanel;
    Rect rawRect;
    public static bool canShow = false;
    public bool selected = false;

    public static int maxRenderDistance = 1000;

    public static UnityAction OnRenderAction;
    public static UnityAction OnDestroyAllAction;
    bool destroyed;

    public void Init()
    {
        OnRenderAction += OnRender;
        OnDestroyAllAction += DestroyAll;
        image.color = defcolor;

        mapClientPanel = ClientPanelManager.GetPanel<MapClientPanel>();

        if (space is Galaxy)
        {
            Galaxy sp = (Galaxy)space;
            if (sp.id == MapSpaceManager.selectedGalaxyId)
            {
                selected = true;
            }
        }
        else if (space is StarSystem)
        {
            StarSystem sp = (StarSystem)space;
            if (sp.galaxyId == MapSpaceManager.selectedGalaxyId && sp.id == MapSpaceManager.selectedSystemId)
            {
                selected = true;
            }
        }
        else if (space is Sector)
        {
            Sector sp = (Sector)space;
            if (sp.galaxyId == MapSpaceManager.selectedGalaxyId && sp.systemId == MapSpaceManager.selectedSystemId && sp.id == MapSpaceManager.selectedSectorId)
            {
                selected = true;
            }
        }
        else if (space is Zone)
        {
            Zone sp = (Zone)space;
            if (sp.galaxyId == MapSpaceManager.selectedGalaxyId && sp.systemId == MapSpaceManager.selectedSystemId && sp.sectorId == MapSpaceManager.selectedSectorId && sp.id == MapSpaceManager.selectedZoneId)
            {
                selected = true;
            }
        }
    }

    public void Destroy()
    {
        OnRenderAction -= OnRender;
        OnDestroyAllAction -= DestroyAll;
        image.color = defcolor;
        destroyed = true;
        //DestroyImmediate(this);
        DestroyImmediate(this.gameObject);
        destroyed = true;
    }
    public void DestroyAll()
    {
        Destroy();
    }
    public void OnRender()
    {
        if (this == null || destroyed || image == null)
        {
            return;
        }
        Camera cam = CameraManager.minimapCamera.curCamera;
        if (!mapClientPanel.gameObject.activeSelf)
        {
            Destroy();
            return;
        }
        if (!CameraManager.minimapCamera.enabled)
        {
            return;
        }

        Vector3 spPos = space.GetPosition();

        if (space.GetType() == typeof(Sector))
        {
            spPos /= SolarObject.scaleFactor;
            maxRenderDistance = 10000000;
        }
        else if (space.GetType() == typeof(Zone))
        {
            spPos /= SolarObject.scaleFactor;
            maxRenderDistance = 10000000;
        }
        else
        {
            maxRenderDistance = 1000;
        }
        float dist = Vector3.Distance(cam.transform.position, spPos);
        if (!selected && dist > maxRenderDistance)
        {
            image.enabled = false;
            this.enabled = false;
            return;
        }
        else
        {
            this.enabled = true;
        }
        float thing1 = Vector3.Dot((spPos - cam.transform.position).normalized, cam.transform.forward);
        if (MapClientPanel.currentLayer != space.spaceMapObject.layer)
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
            spacePosition = spacePosition / SolarObject.scaleFactor;
        }
        else if (space.GetType() == typeof(Zone))
        {
            Zone zn = (Zone)space;
            Sector sctr = MapSpaceManager.singleton.GetSectorByID(zn.galaxyId, zn.systemId, zn.sectorId);
            spacePosition = spacePosition / SolarObject.scaleFactor;
        }
        Vector3 pp = CameraManager.minimapCamera.curCamera.WorldToScreenPoint(spacePosition);

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
            if (NetClient.singleton != null && space.id == LocalClient.GetGalaxyId())
            {
                image.color = curSysColor;
                selected = false;
                if (space.id == MapSpaceManager.selectedGalaxyId)
                {
                    image.color = selcolor;
                    selected = true;
                }
            }
            else if (space.id == MapSpaceManager.selectedGalaxyId)
            {
                image.color = selcolor;
                selected = true;
            }
            else
            {
                image.color = defcolor;
                selected = false;
            }
        }
        else if (space.GetType() == typeof(StarSystem))
        {
            StarSystem ssp = (StarSystem)space;
            if (NetClient.singleton != null && ssp.galaxyId == LocalClient.GetGalaxyId() && ssp.id == LocalClient.GetSystemId())
            {
                image.color = curSysColor;
                selected = false;
                if (ssp.galaxyId == MapSpaceManager.selectedGalaxyId && ssp.id == MapSpaceManager.selectedSystemId)
                {
                    image.color = selcolor;
                    selected = true;
                }
            }
            else if (ssp.galaxyId == MapSpaceManager.selectedGalaxyId && ssp.id == MapSpaceManager.selectedSystemId)
            {
                image.color = selcolor;
                selected = true;
            }
            else
            {
                image.color = defcolor;
                selected = false;
                if (!(ssp.galaxyId == MapSpaceManager.selectedGalaxyId))
                {
                    Destroy();
                    return;
                }
            }
        }
        else if (space.GetType() == typeof(Sector))
        {
            Sector ssp = (Sector)space;
            if (!(ssp.galaxyId == MapSpaceManager.selectedGalaxyId && ssp.systemId == MapSpaceManager.selectedSystemId))
            {
                Destroy();
                return;
            }
            if (NetClient.singleton != null && ssp.galaxyId == LocalClient.GetGalaxyId() && ssp.systemId == LocalClient.GetSystemId() && ssp.id == LocalClient.GetSectorId())
            {
                image.color = curSysColor;
                selected = false;
                if (ssp.galaxyId == MapSpaceManager.selectedGalaxyId && ssp.systemId == MapSpaceManager.selectedSystemId && ssp.id == MapSpaceManager.selectedSectorId)
                {
                    image.color = selcolor;
                    selected = true;
                }
            }
            else if (ssp.galaxyId == MapSpaceManager.selectedGalaxyId && ssp.systemId == MapSpaceManager.selectedSystemId && ssp.id == MapSpaceManager.selectedSectorId)
            {
                image.color = selcolor;
                selected = true;
            }
            else
            {
                image.color = defcolor;
                selected = false;
            }
        }
        else if (space.GetType() == typeof(Zone))
        {
            Zone ssp = (Zone)space;
            if (!(ssp.galaxyId == MapSpaceManager.selectedGalaxyId && ssp.systemId == MapSpaceManager.selectedSystemId && ssp.sectorId == MapSpaceManager.selectedSectorId))
            {
                Destroy();
                return;
            }
            if (NetClient.singleton != null && ssp.galaxyId == LocalClient.GetGalaxyId() && ssp.systemId == LocalClient.GetSystemId() && ssp.sectorId == LocalClient.GetSectorId() && ssp.id == LocalClient.GetZoneId())
            {
                image.color = curSysColor;
            }
            else if (ssp.galaxyId == MapSpaceManager.selectedGalaxyId && ssp.systemId == MapSpaceManager.selectedSystemId && ssp.sectorId == MapSpaceManager.selectedSectorId && ssp.id == MapSpaceManager.selectedZoneId)
            {
                image.color = selcolor;
            }
            else
            {
                image.color = defcolor;
            }
        }
        if (!destroyed)
        {
            transform.SetAsLastSibling();
        }
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (space is Galaxy)
        {
            if (MapSpaceManager.selectedGalaxyId != space.id)
            {
                MapSpaceManager.anotherGalaxySelected = true;
                MapSpaceManager.anotherSystemSelected = true;
                MapSpaceManager.anotherSectorSelected = true;
                MapSpaceManager.anotherZoneSelected = true;
            }
            MapSpaceManager.selectedGalaxyId = space.id;
        }
        else if (space is StarSystem)
        {
            if (MapSpaceManager.selectedSystemId != space.id)
            {
                MapSpaceManager.anotherSystemSelected = true;
                MapSpaceManager.anotherSectorSelected = true;
                MapSpaceManager.anotherZoneSelected = true;
            }
            MapSpaceManager.selectedSystemId = space.id;
        }
        if (space is Sector)
        {
            if (MapSpaceManager.selectedSectorId != space.id)
            {
                MapSpaceManager.anotherSectorSelected = true;
                MapSpaceManager.anotherZoneSelected = true;
            }
            MapSpaceManager.selectedSectorId = space.id;
        }
        if (space is Zone)
        {
            if (MapSpaceManager.selectedZoneId != space.id)
            {
                MapSpaceManager.anotherZoneSelected = true;
            }
            MapSpaceManager.selectedZoneId = space.id;
        }
        MapSpaceManager.InvokeAnotherSpaceSelected(space);
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