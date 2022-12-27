using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : Space
{
    public Zone() : base() { }
    public Zone(string templateName) : base(templateName)
    {
    }

    public override int GenerateId()
    {
        int curId = 0;
        Space fnd = SpaceManager.zones.Find(f => f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.zones.Find(f => f.id == curId);
        }
        id = curId;
        return id;
    }
}
