using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gate : SpaceObject
{
    public int inSystemGateId = -1;
    public int destinationGateId = -1;
    public SpaceSystem spaceSystem;
    public bool warped;
    private void OnTriggerEnter(Collider other)
    {
        if (LocalClient.controlledObject.hull == other.gameObject)
        {
            if (!warped)
            {
                warped = destinationGate.warped = true;
                
                SpaceManager.singleton.currentGalaxy = SpaceManager.singleton.galaxies.Find(g => g.id == destinationSystem.galaxyId);
                LocalClient.controlledObject.Warp(destinationSystem, destinationGate.transform.localPosition, destinationGate.transform.localEulerAngles);
            }
            else
            {
                warped = false;
            }
        }
    }

    public SpaceSystem destinationSystem
    {
        get
        {
            SpaceSystem ret = null;
            Gate fgate = spaceManager.gates.Find(gate => gate.id == destinationGateId);
            if (fgate != null)
            {
                ret = fgate.spaceSystem;
            }
            return ret;
        }
    }

    public Gate destinationGate
    {
        get
        {
            Gate ret = null;
            if (destinationSystem != null)
            {
                Gate fgate = spaceManager.gates.Find(gate => gate.id == destinationGateId);
                if (fgate != null)
                {
                    ret = fgate;
                }
            }
            return ret;
        }
    }
    public bool isConnected { get { return destinationGateId != -1; } }
    public static new Gate Create(SpaceManager spaceManager, Data.SpaceObjectData spaceObjectData)
    {
        Gate retPrefab = GamePrefabsManager.singleton.LoadPrefab<Gate>("GatePrefab");
        Gate ret = GameObject.Instantiate(retPrefab);
        ret.templateName = spaceObjectData.templateName;
        int sysid = 0;
        while (spaceManager.gates.Find(f => f.inSystemGateId == sysid && f.galaxyId == spaceObjectData.galaxyId && f.systemId == spaceObjectData.systemId) != null)
        {
            sysid++;
        }
        int id = 0;
        while (spaceManager.gates.Find(f => f.id == id) != null)
        {
            id++;
        }
        Template template = TemplateManager.FindTemplate(ret.templateName, "gate");
        if (template == null)
        {
            Debug.LogError($"Template {ret.templateName} does not exist.");
            return null;
        }
        ret.mass = int.Parse(template.GetValue("params", "mass"));
        ret.drag = int.Parse(template.GetValue("params", "drag"));
        ret.angulardrag = int.Parse(template.GetValue("params", "angulardrag"));
        ret.modelPatch = template.GetValue("model", "patch");
        ret.id = (uint)id;
        ret.inSystemGateId = sysid;
        spaceManager.gates.Add(ret);
        SpaceSystem spaceSystem = spaceManager.spaceSystems.Find(f => f.galaxyId == spaceObjectData.galaxyId && f.id == spaceObjectData.systemId);
        ret.spaceManager = spaceManager;
        ret.spaceSystem = spaceSystem;
        ret.galaxyId = spaceSystem.galaxyId;
        ret.systemId = spaceSystem.id;
        spaceSystem.gates.Add(ret);

        ret.gameObject.name = "Gate";
        ret.Init();
        return ret;
    }
    public bool ConnectGate(SpaceManager spaceManager, SpaceSystem targetSystem, int gateSystemId)
    {
        bool ret = false;
        Gate targetGate = spaceManager.gates.Find(f => f.galaxyId == targetSystem.galaxyId && f.systemId == targetSystem.id && f.inSystemGateId == gateSystemId);
        Debug.Log($"{targetGate} {targetSystem.galaxyId} {targetSystem.id} {gateSystemId} {targetSystem.gates.Count}");
        if (targetGate != null)
        {
            destinationGateId = (int)targetGate.id;
            targetGate.destinationGateId = (int)id;
            ret = true;
        }
        return ret;
    }
    public bool ConnectGate(SpaceManager spaceManager, Gate gate)
    {
        bool ret = false;
        Gate targetGate = spaceManager.gates.Find(f => f.id == gate.id);
        if (targetGate != null)
        {
            destinationGateId = (int)targetGate.id;
            targetGate.destinationGateId = (int)id;
            ret = true;
        }
        return ret;
    }
}
