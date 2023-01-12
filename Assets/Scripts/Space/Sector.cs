using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : Space
{
    public int galaxyId;
    public int systemId;

    public static int sectorStep = 1000000;
    public static int minimapDivFactor = 2000;

    public int[] indexes = { 0, 0, 0 };
    public Sector() : base() { }
    public Sector(string templateName) : base(templateName)
    {
    }

    public override int GenerateId()
    {
        int curId = 0;
        Space fnd = SpaceManager.sectors.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.sectors.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.id == curId);
        }
        id = curId;
        return id;
    }
    public override void OnRender()
    {
        Vector3 pos = GetPosition();
        Vector3 rot = GetRotation();

        if (MapClientPanel.selectedGalaxyId == galaxyId && MapClientPanel.selectedSystemId == systemId)
        {
            spaceController = GameObject.Instantiate(GameContentManager.sectorPrefab, SpaceManager.singleton.transform);
            spaceController.transform.localPosition = pos/minimapDivFactor;
            float fs = (float)sectorStep/minimapDivFactor;
            spaceController.transform.localScale = new Vector3(fs,fs,fs);
            spaceController.transform.localEulerAngles = rot;
            spaceController.Init();
        }
        else
        {
            DestroyController(2);
        }
    }
    public void SetIndexes(Vector3 indexes)
    {
        this.indexes = new int[] { (int)indexes.x, (int)indexes.y, (int)indexes.z };
    }
    public Vector3 GetIndexes()
    {
        return new Vector3((int)this.indexes[0], (int)this.indexes[1], (int)this.indexes[2]);
    }
}
