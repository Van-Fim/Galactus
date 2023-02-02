using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ClientManager : NetworkBehaviour
{
    public SyncList<uint> clientIds = new SyncList<uint>();
    public static ClientManager singleton;
    public static void Init()
    {
        singleton = GameObject.Instantiate(GameContentManager.ClientManagerPrefab);
    }

    public override void OnStartClient()
    {
        singleton = this;
    }

    [ClientRpc]
    public void UpdaeGlobalClientPos()
    {
        for (int i = 0; i < clientIds.Count; i++)
        {
            Client cl = NetworkClient.spawned[clientIds[i]].GetComponent<Client>();
            SPObject sp = NetworkClient.spawned[cl.targetId].GetComponent<SPObject>();
            cl.ReadSpace();

            if (sp.clientContainer != null && !cl.isLocalPlayer)
            {
                sp.clientContainer.transform.localPosition = SpaceManager.spaceContainer.transform.localPosition + cl.currSector.GetPosition() + cl.currZoneIndexes * Zone.zoneStep;
            }
        }
    }
}