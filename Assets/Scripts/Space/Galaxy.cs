using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : Space
{
    public List<SpaceSystem> spaceSystems = new List<SpaceSystem>();
    public Galaxy(string templateName) : base(templateName)
    {
        GetId();
        SpaceManager.galaxies.Add(this);
    }
    public int GetId()
    {
        int id = 0;
        while (SpaceManager.galaxies.Find(f => f.id == id) != null)
        {
            id++;
        }

        this.id = id;
        return this.id;
    }
}
