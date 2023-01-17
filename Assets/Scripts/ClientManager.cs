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
}
