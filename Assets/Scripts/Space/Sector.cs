using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : Space
{
    public override void OnMinimapRender()
    {

    }
    public Sector(SpaceSystem system, string templateName) : base(templateName)
    {
        galaxyId = system.galaxyId;
        systemId = system.id;
        GetId();
        SpaceManager.sectors.Add(this);
    }
    public int GetId()
    {
        int id = 0;
        while (SpaceManager.sectors.Find(f => f.id == id && f.galaxyId == galaxyId && f.systemId == systemId) != null)
        {
            id++;
        }

        this.id = id;
        return this.id;
    }
}