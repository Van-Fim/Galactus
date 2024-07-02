using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
public enum SystemType
{
    Default,
    Empty
}
public class SpaceSystem : Space
{
    public List<Gate> gates = new List<Gate>();
    public List<SpaceSystem> connectedSystems = new List<SpaceSystem>();
    public List<SpaceSystem> nearSystems = new List<SpaceSystem>();
    public int size;
    public int galaxyId;
    public MPSystemController mp;
    public SystemType systemType = SystemType.Default;
    public string skyboxName;
    public override void OnMinimapRender()
    {
        if (mp == null && systemType == SystemType.Default)
        {
            MPSystemController prefab = GamePrefabsManager.singleton.LoadPrefab<MPSystemController>("Mp_System");
            mp = GameObject.Instantiate(prefab, SpaceManager.singleton.transform);
            mp.transform.localPosition = GetPosition();
            mp.space = this;
            mp.Init();
        }
        if (SpaceManager.singleton.currentGalaxy.id != galaxyId)
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
        if (systemType == SystemType.Default)
        {
            int id = 0;
            while (SpaceManager.singleton.spaceSystems.Find(f => f.id == id && f.galaxyId == galaxy.id) != null)
            {
                id++;
            }
            this.id = id;
        }

        SpaceManager.singleton.spaceSystems.Add(this);
    }
}
