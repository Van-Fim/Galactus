using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameContent;

public class MapSpaceManager : SpaceManager
{
    public static int selectedGalaxyId;
    public static int selectedSystemId;
    public static int selectedSectorId;
    public static int selectedZoneId;
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
    }
}
