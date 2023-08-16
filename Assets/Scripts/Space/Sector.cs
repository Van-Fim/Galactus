using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : GameContent.Space
{
    public bool loaded;
    public int galaxyId;
    public int systemId;
    public static int sectorStep = 27500;
    public Vector3 startPos;
    public Sector() { }
    public Sector(SpaceManager spm, string templateName) : base(templateName)
    {
        int id = 0;
        while (spm.sectors.Find(f => f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
