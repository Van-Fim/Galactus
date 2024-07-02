using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : SpaceObject
{
    public override void Warp(SpaceSystem spaceSystem, Vector3 position, Vector3 rotation)
    {
        if (LocalClient.controlledObject == this)
        {
            PositionFixer.sectorIndexes = Vector3.zero;
            PositionFixer.zoneIndexes = Vector3.zero;
        }
        base.Warp(spaceSystem, position, rotation);
    }
    public override void WarpSystem(SpaceSystem spaceSystem)
    {
        galaxyId = spaceSystem.galaxyId;
        systemId = spaceSystem.id;
        if (LocalClient.controlledObject == this)
        {
            LocalClient.galaxyId = galaxyId;
            LocalClient.systemId = systemId;
            LocalClient.galaxy = SpaceManager.singleton.galaxies.Find(x => x.id == galaxyId);
            LocalClient.spaceSystem = SpaceManager.singleton.spaceSystems.Find(x => x.id == systemId && x.galaxyId == LocalClient.galaxyId);
            SpaceManager.LoadSystem(LocalClient.spaceSystem);
            SpaceObject.InvokeRender();
        }
    }
}
