using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Region : Space
{
    public GameObject gameObject;
    public List<SpaceSystem> spaceSystems = new List<SpaceSystem>();
    public override void OnMinimapRender()
    {
        if (gameObject == null)
        {
            Transform prefab = GamePrefabsManager.LoadPrefab<Transform>("Mp_Region");
            gameObject = GameObject.Instantiate(prefab, SpaceManager.singleton.transform).gameObject;
            gameObject.transform.localPosition = GetPosition();
            gameObject.transform.localScale = new Vector3(scale,scale,scale);
        }
        if (SpaceManager.currentMapGalaxyId != galaxyId)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
    public Region(Galaxy galaxy, string templateName) : base(templateName)
    {
        galaxyId = galaxy.id;
        GetId();
        SpaceManager.regions.Add(this);
    }
    public int GetId()
    {
        int id = 0;
        while (SpaceManager.regions.Find(f => f.id == id && f.galaxyId == galaxyId) != null)
        {
            id++;
        }

        this.id = id;
        return this.id;
    }
}
