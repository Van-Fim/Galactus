using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using GameContent;

public class MapSpaceManager : SpaceManager
{
    public static int selectedGalaxyId;
    public static int selectedSystemId;
    public static int selectedSectorId;
    public static int selectedZoneId;
    public static bool anotherGalaxySelected;
    public static bool anotherSystemSelected;
    public static bool anotherSectorSelected;
    public static bool anotherZoneSelected;
    public static UnityAction<GameContent.Space> OnAnotherSpaceSelectedAction;
    public static new void Init()
    {
        singleton = new GameObject().AddComponent<MapSpaceManager>();
        MapSpaceManager.selectedGalaxyId = NetClient.GetGalaxyId();
        MapSpaceManager.selectedSystemId = NetClient.GetSystemId();
        MapSpaceManager.selectedSectorId = NetClient.GetSectorId();
        MapSpaceManager.selectedZoneId = NetClient.GetZoneId();
        singleton.spaceContainer = new GameObject();
        singleton.spaceContainer.name = "SpaceContainer";
        singleton.spaceContainer.transform.position = Vector3.zero;
        singleton.spaceContainer.transform.rotation = Quaternion.identity;
        DontDestroyOnLoad(singleton.spaceContainer);

        singleton.solarContainer = new GameObject();
        singleton.solarContainer.name = "SolarContainer";
        singleton.solarContainer.transform.position = Vector3.zero;
        singleton.solarContainer.transform.rotation = Quaternion.identity;
        DontDestroyOnLoad(singleton.solarContainer);

        singleton.gameObject.name = "MapSpaceManager";
        DontDestroyOnLoad(singleton.gameObject);

        OnAnotherSpaceSelectedAction += OnAnotherSpaceSelected;
    }
    public static void OnAnotherSpaceSelected(GameContent.Space space)
    {
        if (space is Galaxy && anotherGalaxySelected)
        {
            DebugConsole.Log($"Another galaxy selected");
            MapSpaceManager.selectedSystemId = 0;
            MapSpaceManager.selectedSectorId = 0;
            MapSpaceManager.selectedZoneId = 0;
            StarSystem sp = MapSpaceManager.singleton.GetSystemByID(space.id, MapSpaceManager.selectedSystemId);
            if (sp != null)
            {
                InvokeAnotherSpaceSelected(sp);
            }
        }
        if (space is StarSystem && anotherSystemSelected)
        {
            DebugConsole.Log($"Another system selected");
            MapSpaceManager.selectedSectorId = 0;
            MapSpaceManager.selectedZoneId = 0;
            StarSystem starSystem = (StarSystem)space;
            Sector sp = MapSpaceManager.singleton.GetSectorByID(starSystem.galaxyId, starSystem.id, MapSpaceManager.selectedSystemId);
            if (sp != null)
            {
                InvokeAnotherSpaceSelected(sp);
            }
        }
        if (space is Sector && anotherSectorSelected)
        {
            DebugConsole.Log($"Another sector selected");
            MapSpaceManager.selectedZoneId = 0;
            Sector sector = (Sector)space;
            Zone sp = MapSpaceManager.singleton.GetZoneByID(sector.galaxyId, sector.systemId, sector.id, MapSpaceManager.selectedZoneId);
            if (sp != null)
            {
                InvokeAnotherSpaceSelected(sp);
            }
        }
        if (space is Zone && anotherZoneSelected)
        {
            DebugConsole.Log($"Another zone selected");
        }
    }
    public void LateUpdate()
    {
        SpaceUiObj.InvokeRender();
    }
    public static void InvokeAnotherSpaceSelected(GameContent.Space space)
    {
        OnAnotherSpaceSelectedAction?.Invoke(space);
    }
}
