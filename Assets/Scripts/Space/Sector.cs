using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : Space
{
    public Sector() : base() { }
    public Sector(string templateName) : base(templateName)
    {
    }

    public override int GenerateId()
    {
        int curId = 0;
        Space fnd = SpaceManager.sectors.Find(f => f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.sectors.Find(f => f.id == curId);
        }
        id = curId;
        return id;
    }
}
