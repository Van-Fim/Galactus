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
    public bool hidden;
    public bool temp;
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
                SpaceManager.regions[i].bounds = new Bounds(SpaceManager.regions[i].GetPosition(), new Vector3(scl, scl, scl));
                if (SpaceManager.regions[i].bounds.Contains(GetPosition()))
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
        if (hidden)
        {
            return;
        }
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
            Show();
        }
    }
    public void Show()
    {
        if (mp != null)
        {
            mp.gameObject.SetActive(true);
            Material mat = mp.obj.GetComponent<MeshRenderer>().material;
            if (CameraManager.mapCamera.gameObject.activeSelf)
            {
                mat.SetFloat("_FadeRangeFactor", 30000f);
                if (regions.Find(x => x.templateName == "Region01") != null)
                {
                    mat.SetColor("_Color", new Color32(255, 255, 255, 255));
                    mat.SetFloat("_Scaling", 0.5f);
                }
            }
            else
            {
                mat.SetFloat("_FadeRangeFactor", 5000f);
                mat.SetColor("_Color", GetColor());
                mat.SetFloat("_Scaling", 0.1f);
            }
            if (temp)
            {
                // mat.SetFloat("_FadeRangeFactor", 30000f);
                // mat.SetColor("_Color", GetColor());
                // mat.SetFloat("_Scaling", 0.5f);
            }
        }
    }
    public SpaceSystem() : base()
    {
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
    public override void Destroy()
    {
        OnRegionCheckAction -= OnRegionCheck;
        regions = null;
        int ind = SpaceManager.spaceSystems.IndexOf(this);
        SpaceManager.spaceSystems.RemoveAt(ind);
        base.Destroy();
    }
    public void Hide()
    {
        if (mp != null)
        {
            mp.gameObject.SetActive(false);
        }
        hidden = true;
    }
}
