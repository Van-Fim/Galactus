using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiSpaceObject : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Space space;

    public static Color32 defcolor = new Color32(100, 100, 100, 100);
    public static Color32 curSysColor = new Color32(100, 100, 255, 255);
    public static Color32 selcolor = new Color32(100, 255, 100, 255);
    public static bool canShow = false;
    public bool selected = false;

    public static UnityAction OnUpdatePos;

    public void Init()
    {
        OnUpdatePos += UpdatePos;
        image.color = defcolor;
    }

    public void DestroyObj()
    {
        OnUpdatePos -= UpdatePos;
        if (gameObject != null)
        {
            DestroyImmediate(gameObject);
        }
    }

    public void UpdatePos()
    {
        Camera cam = CameraManager.minimapCamera.curCamera;
        //float dist = Vector3.Distance(cam.transform.position, space.GetPosition());
        float thing1 = Vector3.Dot((space.GetPosition() - cam.transform.position).normalized, cam.transform.forward);

        if (CameraManager.currentCameraCode != 1)
        {
            image.enabled = false;
            return;
        }
        else
        {
            image.enabled = true;

            if (space.GetType() == typeof(Galaxy))
            {
                Galaxy gal = (Galaxy)space;
                if (gal.id == MinimapPanel.currentGalaxyId)
                {
                    image.color = curSysColor;
                }
                else
                {
                    image.color = defcolor;
                }
                Color32 prevColor = image.color;
                if (gal.id == MinimapPanel.selectedGalaxyId)
                {
                    image.color = selcolor;
                }
                else
                {
                    image.color = prevColor;
                }
            }
            else if (space.GetType() == typeof(StarSystem))
            {
                StarSystem sys = (StarSystem)space;
                if (sys.galaxyId == MinimapPanel.currentGalaxyId && sys.id == MinimapPanel.currentSystemId)
                {
                    image.color = curSysColor;
                }
                else
                {
                    image.color = defcolor;
                }
                Color32 prevColor = image.color;
                if (sys.galaxyId == MinimapPanel.selectedGalaxyId && sys.id == MinimapPanel.selectedSystemId)
                {
                    image.color = selcolor;
                }
                else
                {
                    image.color = prevColor;
                }
            }
            else if (space.GetType() == typeof(Sector))
            {
                Sector sp = (Sector)space;
                if (sp.galaxyId == MinimapPanel.currentGalaxyId && sp.systemId == MinimapPanel.currentSystemId && sp.id == MinimapPanel.currentSectorId)
                {
                    image.color = curSysColor;
                }
                else
                {
                    image.color = defcolor;
                }
                Color32 prevColor = image.color;
                if (sp.galaxyId == MinimapPanel.selectedGalaxyId && sp.systemId == MinimapPanel.selectedSystemId && sp.id == MinimapPanel.selectedSectorId)
                {
                    image.color = selcolor;
                }
                else
                {
                    image.color = prevColor;
                }
            }
            else if (space.GetType() == typeof(Zone))
            {
                Zone sp = (Zone)space;
                if (sp.galaxyId == MinimapPanel.currentGalaxyId && sp.systemId == MinimapPanel.currentSystemId && sp.sectorId == MinimapPanel.currentSectorId && sp.id == MinimapPanel.currentZoneId)
                {
                    image.color = curSysColor;
                }
                else
                {
                    image.color = defcolor;
                }
                Color32 prevColor = image.color;
                if (sp.galaxyId == MinimapPanel.selectedGalaxyId && sp.systemId == MinimapPanel.selectedSystemId && sp.sectorId == MinimapPanel.currentSectorId && sp.id == MinimapPanel.currentZoneId)
                {
                    image.color = selcolor;
                }
                else
                {
                    image.color = prevColor;
                }
            }
        }

        if (MinimapPanel.layer != space.spaceController.layer)
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
        Vector3 pp = CameraManager.minimapCamera.curCamera.WorldToScreenPoint(spacePosition);
        transform.position = new Vector3(pp.x, pp.y, 0);
        image.rectTransform.sizeDelta = new Vector2(30, 30);
        if (MinimapPanel.layer == 0)
        {
            image.rectTransform.sizeDelta = new Vector2(100, 100);
        }
    }

    public static void InvokeUpdatePos()
    {
        OnUpdatePos?.Invoke();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (space.GetType() == typeof(Galaxy))
        {
            /*
            Galaxy gal = (Galaxy)space;
            MinimapPanel.selectedGalaxyId = gal.id;
            MinimapPanel.selectedSystemId = -1;
            */
        }
        else if (space.GetType() == typeof(StarSystem))
        {
            StarSystem sys = (StarSystem)space;
            MinimapPanel.selectedGalaxyId = sys.galaxyId;
            MinimapPanel.selectedSystemId = sys.id;
        }
        else if (space.GetType() == typeof(Sector))
        {
            Sector sp = (Sector)space;
            MinimapPanel.selectedGalaxyId = sp.galaxyId;
            MinimapPanel.selectedSystemId = sp.systemId;
            MinimapPanel.selectedSectorId = sp.id;
        }
        else if (space.GetType() == typeof(Zone))
        {
            Zone sp = (Zone)space;
            MinimapPanel.selectedGalaxyId = sp.galaxyId;
            MinimapPanel.selectedSystemId = sp.systemId;
            MinimapPanel.selectedSectorId = sp.sectorId;
            MinimapPanel.selectedZoneId = sp.id;
        }
    }
}
