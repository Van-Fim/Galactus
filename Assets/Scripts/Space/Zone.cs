using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : GameContent.Space
{
    public bool loaded;
    public int galaxyId;
    public int systemId;
    public int sectorId;
    public static int zoneStep = 25000;
    public Zone() { }
    public Zone(SpaceManager spm, string templateName, Sector sector) : base(templateName)
    {
        sectorId = sector.id;
        List<Zone> allZones = spm.zones.FindAll(f => f.sectorId == sectorId);
        int id = 0;
        while (allZones.Find(f => f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
