using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : Space
{
    public int galaxyId;
    public int systemId;
    public int sectorId;

    public static int minimapDivFactor = 2000;
    public static int zoneStep = 2500;

    public int[] indexes = { 0, 0, 0 };

    public Zone() : base() { }
    public Zone(string templateName) : base(templateName)
    {
    }

    public override int GenerateId()
    {
        int curId = 1;
        Space fnd = SpaceManager.zones.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.sectorId == sectorId && f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.zones.Find(f => f.galaxyId == galaxyId && f.systemId == systemId && f.sectorId == sectorId && f.id == curId);
        }
        id = curId;
        return id;
    }
    public override void OnRender()
    {
        Vector3 pos = GetPosition();
        Vector3 rot = GetRotation();

        if (MapClientPanel.currentLayer == GameContentManager.zonePrefab.layer && MapClientPanel.selectedGalaxyId == galaxyId && MapClientPanel.selectedSystemId == systemId && MapClientPanel.selectedSectorId == sectorId)
        {
            spaceController = GameObject.Instantiate(GameContentManager.zonePrefab, SpaceManager.singleton.transform);
            spaceController.transform.localPosition = pos / minimapDivFactor;
            float fs = (float)zoneStep / minimapDivFactor;
            spaceController.transform.localScale = new Vector3(fs, fs, fs);
            spaceController.transform.localEulerAngles = rot;
            spaceController.meshRenderer.material.SetColor("_TintColor", GetColor());
            spaceController.meshRenderer.material.SetColor("_Color", GetColor());
            spaceController.meshRenderer.enabled = true;
            spaceController.Init();
        }
        else
        {
            DestroyController(3);
        }
    }
    public override void OnDrawUi()
    {
        base.OnDrawUi();
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
