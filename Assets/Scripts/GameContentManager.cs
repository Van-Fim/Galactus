using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContentManager
{
    public static List<string> list;

    public static CameraController mainCameraPrefab;
    public static Camera skyboxCameraPrefab;
    public static CameraController minimapCameraPrefab;

    public static CanvasController canvasPrefab;

    public static Ship shipPrefab;
    public static Pilot pilotPrefab;

    public static HudClientPanel hudClientPanelPrefab;
    public static MapClientPanel mapClientPanelPrefab;

    public static SpaceController galaxyPrefab;
    public static SpaceController systemPrefab;
    public static SpaceController sectorPrefab;
    public static SpaceController zonePrefab;
    public static SpaceUiObj spaceUiObjPrefab;

    public static void InitContent()
    {
        list = new List<string>();
        list.Add("Content");

        for (int i = 0; i < list.Count; i++)
        {
            TemplateManager.LoadTemplates("planet", list[i] + "/Planets");
            TemplateManager.LoadTemplates("sun", list[i] + "/Suns");
            TemplateManager.LoadTemplates("zone", list[i] + "/Zones");
            TemplateManager.LoadTemplates("sector", list[i] + "/Sectors");
            TemplateManager.LoadTemplates("system", list[i] + "/Systems");
            TemplateManager.LoadTemplates("galaxy", list[i] + "/Galaxies");
            TemplateManager.LoadTemplates("universe", list[i] + "/Universe");

            TemplateManager.LoadTemplates("start", list[i] + "/GameStarts");
            TemplateManager.LoadTemplates("loadouts", list[i] + "/Loadouts");
            TemplateManager.LoadTemplates("hardpoint", list[i] + "/Hardpoints");
            TemplateManager.LoadTemplates("ship", list[i] + "/Ships");
            TemplateManager.LoadTemplates("pilot", list[i] + "/Pilots");
        }
    }

    public static void LoadPrefabs()
    {
        mainCameraPrefab = Resources.Load<CameraController>("Prefabs/MainCameraPrefab");
        skyboxCameraPrefab = Resources.Load<Camera>("Prefabs/SkyboxCameraPrefab");
        canvasPrefab = Resources.Load<CanvasController>("Prefabs/canvasPrefab");
        minimapCameraPrefab = Resources.Load<CameraController>("Prefabs/MinimapCameraPrefab");
        shipPrefab = Resources.Load<Ship>("Prefabs/ShipPrefab");
        pilotPrefab = Resources.Load<Pilot>("Prefabs/PilotPrefab");
        hudClientPanelPrefab = Resources.Load<HudClientPanel>("Prefabs/HudClientPanelPrefab");
        mapClientPanelPrefab = Resources.Load<MapClientPanel>("Prefabs/MapClientPanelPrefab");
        galaxyPrefab = Resources.Load<SpaceController>("Prefabs/GalaxyPrefab");
        systemPrefab = Resources.Load<SpaceController>("Prefabs/SystemPrefab");
        sectorPrefab = Resources.Load<SpaceController>("Prefabs/SectorPrefab");
        zonePrefab = Resources.Load<SpaceController>("Prefabs/ZonePrefab");
        spaceUiObjPrefab = Resources.Load<SpaceUiObj>("Prefabs/SpaceUiObjPrefab");
    }
}