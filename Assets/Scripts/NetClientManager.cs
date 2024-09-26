using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
public struct LoginData : NetworkMessage
{
    public NetSpaceObject netId;
    public string gameStart;
}
public class NetClientManager : NetworkBehaviour
{
    public List<NetSpaceObject> netSpaceObjects = new List<NetSpaceObject>();
    public static NetClientManager singleton;
    public void Awake()
    {
        singleton = this;
    }
    [Command(requiresAuthority = false)]
    public void SendFixedIndexes(uint netid, Vector3 indexes)
    {
        for (int i = 0; i < netSpaceObjects.Count; i++)
        {
            netSpaceObjects[i].SendBackFixedIndexes(netid, indexes);
        }
    }
}
