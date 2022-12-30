using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : Space
{
    public int galaxyId;
    public int systemId;
    public int sectorId;

    public static int minimapDivFactor = 2000;

    public int[] indexes = { 0, 0, 0 };

    public Zone() : base() { }
    public Zone(string templateName) : base(templateName)
    {
    }

    public override int GenerateId()
    {
        int curId = 0;
        Space fnd = SpaceManager.zones.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.sectorId == sectorId && f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.zones.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.sectorId == sectorId && f.id == curId);
        }
        id = curId;
        return id;
    }
    public virtual void SetIndexes(Vector3 indexes)
    {
        this.indexes = new int[] { (int)indexes.x, (int)indexes.y, (int)indexes.z };
    }
    public virtual Vector3 GetIndexes()
    {
        return new Vector3((int)this.indexes[0], (int)this.indexes[1], (int)this.indexes[2]);
    }
}
