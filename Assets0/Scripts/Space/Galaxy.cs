using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : Space
{    
    public Galaxy(SpaceManager spaceManager, string templateName) : base(templateName)
    {
        int id = 0;
        while (spaceManager.galaxies.Find(f => f.id == id) != null)
        {
            id++;
        }
        this.id = id;
        spaceManager.galaxies.Add(this);
    }
}
