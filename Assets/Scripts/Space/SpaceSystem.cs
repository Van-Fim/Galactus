using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SpaceSystem : Space
{
    public string skyboxName;
    public static UnityAction OnRegionCheckAction;
    public List<Region> regions = new List<Region>();
    public override void Init()
    {
        base.Init();
        OnRegionCheckAction += OnRegionCheck;
    }
    public void OnRegionCheck()
    {
        if (SpaceManager.currentMapGalaxyId == galaxyId)
        {
            for (int i = 0; i < SpaceManager.regions.Count; i++)
            {
                float scl = SpaceManager.regions[i].scale;
                Bounds bounds = new Bounds(SpaceManager.regions[i].GetPosition(), new Vector3(scl, scl, scl));
                if (bounds.Contains(GetPosition()))
                {
                    regions.Add(SpaceManager.regions[i]);
                    SpaceManager.regions[i].spaceSystems.Add(this);
                }
            }
        }
    }
    public static void InvokeRegionCheck()
    {
        OnRegionCheckAction?.Invoke();
    }
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
                if (regions.Find(x => x.templateName == "Region01") != null)
                {
                    mp.obj.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color32(255, 255, 255, 255));
                    mp.obj.GetComponent<MeshRenderer>().material.SetFloat("_Scaling", 0.5f);
                }
                else
                {
                    mp.obj.GetComponent<MeshRenderer>().material.SetColor("_Color", mp.space.GetColor());
                }
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
