using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalClient : MonoBehaviour
{
    public static string gamestartTemplateName;
    public static bool is_gamestart_started;
    public static bool skipFuckingControlledObjAndDie;
    public static string universeTemplateName = "default";
    private static Galaxy galaxy;
    private static SpaceSystem spaceSystem;
    private static Sector sector;
    public static int galaxyId;
    public static int systemId;
    public static int sectorId;
    public static int[] sectorIndexes = { 0, 0, 0 };
    private static SpaceObject controlledObject;
    public static bool isServer;
    public static Galaxy Galaxy
    {
        get
        {
            return SpaceManager.galaxies.Find(x => x.galaxyId == galaxyId);
        }
    }
    public static SpaceSystem SpaceSystem
    {
        get
        {
            return SpaceManager.spaceSystems.Find(x => x.galaxyId == galaxyId && x.id == systemId);
        }
    }
    public static Sector Sector
    {
        get
        {
            return SpaceManager.sectors.Find(x => x.galaxyId == galaxyId && x.systemId == systemId && x.id == sectorId);
        }
    }

    public static SpaceObject ControlledObject { get => controlledObject; set{
        controlledObject = value;
    }}

    public static Vector3 GetSectorIndexes()
    {
        return new Vector3(sectorIndexes[0], sectorIndexes[1], sectorIndexes[2]);
    }
    public static void SetSectorIndexes(Vector3 value)
    {
        sectorIndexes = new int[] { (int)value.x, (int)value.y, (int)value.z };
    }
}
