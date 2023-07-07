using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : GameContent.Space
{
    public int galaxyId;
    public StarSystem() { }
    public StarSystem(string templateName) : base(templateName)
    {
        SpaceManager spm = SpaceManager.singleton;
        int id = 0;
        while (spm.starSystems.Find(f=>f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
