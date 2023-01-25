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

    public static void Init(){
        singleton = Instantiate(GameContentManager.SPObjectManagerPrefab);
        singleton.name = "SPObjectManager";
    }
}
