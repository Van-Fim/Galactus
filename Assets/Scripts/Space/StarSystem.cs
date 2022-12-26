using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : Space
{
    public int galaxyId;
    public StarSystem() : base() { }
    public StarSystem(string templateName) : base(templateName)
    {
    }
    public StarSystem(string templateName, Galaxy galaxy) : base(templateName)
    {
        this.galaxyId = galaxy.id;
    }
}
