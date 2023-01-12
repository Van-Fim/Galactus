using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : Space
{
    public int galaxyId;
    public string skyboxName;
    public StarSystem() : base() { }
    public StarSystem(string templateName) : base(templateName)
    {
    }
    public StarSystem(string templateName, Galaxy galaxy) : base(templateName)
    {
        this.galaxyId = galaxy.id;
    }
    public override int GenerateId()
    {
        int curId = 0;
        Space fnd = SpaceManager.starSystems.Find(f => f.galaxyId == galaxyId && f.id == curId);

        while (fnd != null)
        {
            curId++;
            fnd = SpaceManager.starSystems.Find(f => f.galaxyId == galaxyId && f.id == curId);
        }
        id = curId;
        return id;
    }
    public override void OnRender()
    {
        Vector3 pos = GetPosition();
        Vector3 rot = GetRotation();

        if (MapClientPanel.selectedGalaxyId == galaxyId)
        {
            spaceController = GameObject.Instantiate(GameContentManager.systemPrefab);
        }
    }
}
