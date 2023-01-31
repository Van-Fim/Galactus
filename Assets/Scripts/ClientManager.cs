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

    [ClientRpc]
    public void UpdaeGlobalClientPos()
    {
        // for (int i = 0; i < clientIds.Count; i++)
        // {
        //     Client cl = NetworkClient.spawned[clientIds[i]].GetComponent<Client>();
        //     SPObject sp = NetworkClient.spawned[cl.targetId].GetComponent<SPObject>();
        //     cl.ReadSpace();
        //     sp.netTranform.globalPos = cl.currSector.GetPosition() + cl.currZone.GetPosition();
        //     Debug.Log(sp.netTranform.globalPos);
        // }
    }
}