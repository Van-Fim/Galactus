using Mirror.Examples.MultipleMatch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContentManager
{
    public static List<string> list;

    public static CameraController mainCameraPrefab;
    public static CameraController skyboxCameraPrefab;
    public static CameraController minimapCameraPrefab;

    public static MainPlayerPanel mainPlayerPanelPrefab;
    public static MinimapPanel minimapPanelPrefab;
    public static UiSpaceObject uiSpaceObjectPrefab;

    public static SpaceManager spaceManagerPrefab;
    public static CanvasController canvasPrefab;

    public static SpaceController galaxyPrefab;
    public static SpaceController systemPrefab;
    public static SpaceController sectorPrefab;
    public static SpaceController zonePrefab;

    public static RandomAreaSpawner asteroidSpawnerPrefab;

    public static SpaceObjectNetManager spaceObjectNetManagerPrefab;
    public static ClientManager clientManagerPrefab;

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
        }
    }

    public static void LoadPrefabs()
    {
        mainCameraPrefab = Resources.Load<CameraController>("Prefabs/MainCameraPrefab");
        skyboxCameraPrefab = Resources.Load<CameraController>("Prefabs/SkyboxCameraPrefab");
        spaceManagerPrefab = Resources.Load<SpaceManager>("Prefabs/SpaceManagerPrefab");
        canvasPrefab = Resources.Load<CanvasController>("Prefabs/canvasPrefab");
        minimapCameraPrefab = Resources.Load<CameraController>("Prefabs/MinimapCameraPrefab");

        mainPlayerPanelPrefab = Resources.Load<MainPlayerPanel>("Prefabs/MainPlayerPanelPrefab");
        minimapPanelPrefab = Resources.Load<MinimapPanel>("Prefabs/MinimapPanelPrefab");
        uiSpaceObjectPrefab = Resources.Load<UiSpaceObject>("Prefabs/UiSpaceObjectPrefab");

        galaxyPrefab = Resources.Load<SpaceController>("Prefabs/GalaxyPrefab");
        systemPrefab = Resources.Load<SpaceController>("Prefabs/SystemPrefab");
        sectorPrefab = Resources.Load<SpaceController>("Prefabs/SectorPrefab");
        zonePrefab = Resources.Load<SpaceController>("Prefabs/ZonePrefab");

        asteroidSpawnerPrefab = Resources.Load<RandomAreaSpawner>("Prefabs/AsteroidSpawner");

        spaceObjectNetManagerPrefab = Resources.Load<SpaceObjectNetManager>("Prefabs/SpaceObjectNetManagerPrefab");
        clientManagerPrefab = Resources.Load<ClientManager>("Prefabs/ClientManagerPrefab");
    }
}