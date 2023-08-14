using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameContent;

public class SpaceManager : MonoBehaviour
{
    public static SpaceManager singleton;
    public GameObject spaceContainer;
    public GameObject solarContainer;
    public List<Galaxy> galaxies = new List<Galaxy>();
    public List<StarSystem> starSystems = new List<StarSystem>();
    public List<Sector> sectors = new List<Sector>();
    public List<Zone> zones = new List<Zone>();
    public List<Sun> suns = new List<Sun>();
    public List<Planet> planets = new List<Planet>();

    public static void Init()
    {
        singleton = new GameObject().AddComponent<SpaceManager>();

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

        singleton.gameObject.name = "SpaceManager";
        DontDestroyOnLoad(singleton.gameObject);
    }
    public void DestroyGalaxies()
    {
        for (int i = 0; i < galaxies.Count; i++)
        {
            galaxies[i].Destroy();
        }
        galaxies = new List<Galaxy>();
    }
    public void DestroySystems()
    {
        for (int i = 0; i < starSystems.Count; i++)
        {
            starSystems[i].Destroy();
        }
        starSystems = new List<StarSystem>();
    }
    public void DestroySectors()
    {
        for (int i = 0; i < sectors.Count; i++)
        {
            sectors[i].Destroy();
        }
        sectors = new List<Sector>();
    }
    public void DestroyZones()
    {
        for (int i = 0; i < zones.Count; i++)
        {
            zones[i].Destroy();
        }
        zones = new List<Zone>();
    }
    public void BuildSystem(int galaxyId, int systemId)
    {
        Galaxy currentGalaxy = GetGalaxyByID(galaxyId);
        GalaxyBuilder.Build(this, currentGalaxy);
        StarSystem currentSystem = GetSystemByID(galaxyId, systemId);
        NetClient.singleton.Galaxy = currentGalaxy;
        NetClient.singleton.System = currentSystem;
        string buildSeed = (GameManager.singleton.seed + "_" + currentGalaxy.id.ToString() + "_" + currentSystem.id.ToString());
        Random.InitState((buildSeed).GetHashCode());
        PlanetsBuilder.Build(this, currentSystem);
        Random.InitState((buildSeed).GetHashCode());
        SectorsBuilder.Build(this, currentSystem);
        Random.InitState((buildSeed).GetHashCode());
        ZonesBuilder.Build(this, currentSystem);
        Random.InitState((buildSeed).GetHashCode());
        Sector.InvokeRender();
        Zone.InvokeRender();
        Planet.InvokeRender();
        Sun.InvokeRender();
    }

    public void LoadGalaxies()
    {
        UniverseBuilder.Build(this);
    }
    public Galaxy GetGalaxyByID(int id)
    {
        Galaxy galaxy = galaxies.Find(f => f.id == id);
        return galaxy;
    }
    public StarSystem GetSystemByID(int galaxyId, int id)
    {
        StarSystem system = starSystems.Find(f => f.galaxyId == galaxyId && f.id == id);
        return system;
    }
    public Sector GetSectorByID(int galaxyId, int systemId, int id)
    {
        Sector sector = sectors.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.id == id);
        return sector;
    }
    public Zone GetZoneByID(int galaxyId, int systemId, int sectorId, int id)
    {
        Zone zone = zones.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.sectorId == sectorId && f.id == id);
        return zone;
    }
}
