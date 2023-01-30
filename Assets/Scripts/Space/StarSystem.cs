using System.Collections;
using System.Collections.Generic;
using Mirror;
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

    public void LoadAsteroids(int chance, int countMin, int countMax)
    {
        int chr = Random.Range(0, 101);
        if (chr <= chance)
        {
            int count = Random.Range(countMin, countMax + 1);
            for (int i = 0; i < count; i++)
            {
                Asteroid ast = Asteroid.Create("Aster01");
                ast.SetSpace(this);
                Vector3 pos = Random.insideUnitSphere * 1000000;
                ast.transform.localPosition = pos;
                NetworkServer.Spawn(ast.gameObject);
            }
        }
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

        if (MapClientPanel.currentLayer == GameContentManager.systemPrefab.layer && MapClientPanel.selectedGalaxyId == galaxyId)
        {
            spaceController = GameObject.Instantiate(GameContentManager.systemPrefab, SpaceManager.singleton.transform);
            spaceController.transform.localPosition = pos;
            spaceController.transform.localEulerAngles = rot;
            spaceController.meshRenderer.material.SetColor("_TintColor", GetBgColor());
            spaceController.meshRenderer.material.SetColor("_Color", GetBgColor());
            spaceController.meshRenderer.enabled = true;
            spaceController.Init();
        }
        else
        {
            DestroyController(1);
        }
    }
    public override void OnDrawUi()
    {
        base.OnDrawUi();
    }
}
