using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : GameContent.Space
{
    public int galaxyId;
    public int systemId;
    public int sectorId;
    public static int zoneStep = 25000;
    public Zone() { }
    public Zone(SpaceManager spm, string templateName) : base(templateName)
    {
        int id = 0;
        while (spm.zones.Find(f => f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
