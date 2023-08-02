using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameContent;
public class Galaxy : GameContent.Space
{
    public bool loaded;
    public Galaxy() { }
    public Galaxy(SpaceManager spm, string templateName) : base(templateName)
    {
        int id = 0;
        while (spm.galaxies.Find(f=>f.id == id) != null)
        {
            id++;
        }
        this.id = id;
    }
}
