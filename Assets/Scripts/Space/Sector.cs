using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : GameContent.Space
{
    public int galaxyId;
    public int systemId;
    public static int sectorStep = 275000;
    public Vector3 startPos;
    public Sector() { }
    public Sector(string templateName) : base(templateName)
    {
        SpaceManager spm = SpaceManager.singleton;
        int id = 0;
        while (spm.sectors.Find(f => f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
