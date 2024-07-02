using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpaceSystem : Space
{
    public string skyboxName;
    public override void OnMinimapRender()
    {
        if (mp == null)
        {
            MPSystemController prefab = GamePrefabsManager.LoadPrefab<MPSystemController>("Mp_System");
            mp = GameObject.Instantiate(prefab, SpaceManager.singleton.transform);
            mp.transform.localPosition = GetPosition();
            mp.space = this;
            mp.Init();
        }
        if (SpaceManager.currentMapGalaxyId != galaxyId)
        {
            if (mp != null)
            {
                mp.gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            if (mp != null)
            {
                mp.gameObject.SetActive(true);
            }
        }
    }
    public SpaceSystem(Galaxy galaxy, string templateName) : base(templateName)
    {
        galaxyId = galaxy.id;
        GetId();
        SpaceManager.spaceSystems.Add(this);
    }
    public int GetId()
    {
        int id = 0;
        while (SpaceManager.spaceSystems.Find(f => f.id == id && f.galaxyId == galaxyId) != null)
        {
            id++;
        }

        this.id = id;
        return this.id;
    }
}
