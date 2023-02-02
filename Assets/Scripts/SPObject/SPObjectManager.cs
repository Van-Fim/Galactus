using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SPObjectManager : NetworkBehaviour
{
    public static SPObjectManager singleton;
    public List<uint> shipsIds = new List<uint>();
    public SyncList<uint> pilotsIds = new SyncList<uint>();
    public SyncList<uint> asteroidIds = new SyncList<uint>();

    public static void Init()
    {
        singleton = Instantiate(GameContentManager.SPObjectManagerPrefab);
        singleton.name = "SPObjectManager";
    }

    public override void OnStartClient()
    {
        singleton = this;
    }

    [ClientRpc]
    public void InvokeRenderClient(uint netId, int galaxyId, int systemId, int sectorId, int zoneid)
    {
        Client cl = NetworkClient.spawned[netId].GetComponent<Client>();
        cl.galaxyId = galaxyId;
        cl.systemId = systemId;
        cl.sectorId = sectorId;
        cl.zoneId = zoneid;

        if (cl.targetId > 0)
        {
            SPObject sp = NetworkClient.spawned[cl.targetId].GetComponent<SPObject>();
            sp.galaxyId = galaxyId;
            sp.systemId = systemId;
            sp.sectorId = sectorId;
            sp.zoneId = zoneid;
        }
        SPObject.InvokeRender();
    }
}
