using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : GameContent.Space
{
    public int galaxyId;
    public int systemId;
    public int sectorId;
    public Zone() { }
    public Zone(string templateName) : base(templateName)
    {
        SpaceManager spm = SpaceManager.singleton;
        int id = 0;
        while (spm.zones.Find(f => f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
