using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameContent;
public class Galaxy : GameContent.Space
{
    public Galaxy() { }
    public Galaxy(string templateName) : base(templateName)
    {
        SpaceManager spm = SpaceManager.singleton;
        int id = 0;
        while (spm.galaxies.Find(f=>f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
