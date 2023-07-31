using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemBuilder : ISpaceBuilder
{
    public static void Build(SpaceManager spm, StarSystem system)
    {
        PlanetsBuilder.Build(spm, system);
        SectorsBuilder.Build(spm, system);
        ZonesBuilder.Build(spm, system);
    }
}
