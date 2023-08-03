using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : GameContent.Space
{
    public bool loaded;
    public int galaxyId;
    public string skyboxName;
    public StarSystem() { }
    public StarSystem(string templateName, Galaxy galaxy, SpaceManager spaceManager) : base(templateName)
    {
        int id = 0;
        while (spaceManager.starSystems.Find(f=>f.id == id) != null)
        {
            id++;
        }
        this.id = id;
        this.galaxyId = galaxy.id;
    }
}
