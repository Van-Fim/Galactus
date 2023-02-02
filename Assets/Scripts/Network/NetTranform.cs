using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetTranform : NetworkTransform
{
    public SPObject sPObject;
    public uint clientId;
    public Client client;
    public void Start()
    {
        sPObject = gameObject.GetComponent<SPObject>();
        if (syncGlobalPos)
        {
            client = NetworkClient.spawned[clientId].GetComponent<Client>();
        }
    }

    [SyncVar]
    public Vector3 globalPos;
    [SyncVar]
    public bool syncGlobalPos = false;

    protected override void OnClientToServerSync(Vector3? position, Quaternion? rotation, Vector3? scale)
    {
        base.OnClientToServerSync(position, rotation, scale);
    }
    protected override void OnServerToClientSync(Vector3? position, Quaternion? rotation, Vector3? scale)
    {
        position = SyncGlobalPos(position);
        base.OnServerToClientSync(position, rotation, scale);
    }

    public Vector3? SyncGlobalPos(Vector3? position){
        if (SpaceManager.spaceContainer != null && syncGlobalPos)
        {
            if (sPObject != null && !sPObject.isLocalPlayerControll && client != null)
            {
                position = (SpaceManager.spaceContainer.transform.localPosition + client.transform.localPosition);
                //DebugConsole.Log($"{client} {SpaceManager.spaceContainer.transform.localPosition} {client.transform.localPosition} {SpaceManager.spaceContainer.transform.localPosition + client.transform.localPosition}", false);
            }
            //DebugConsole.Log($"{sPObject != null && !sPObject.isLocalPlayerControll && client != null} {syncGlobalPos}", false);
        }
        return position;
    }
    public void SyncGlobalPos(Vector3 position){
        if (SpaceManager.spaceContainer != null && syncGlobalPos)
        {
            if (sPObject != null && !sPObject.isLocalPlayerControll && client != null)
            {
                position = (SpaceManager.spaceContainer.transform.localPosition + client.transform.localPosition);
                //DebugConsole.Log($"{client} {SpaceManager.spaceContainer.transform.localPosition} {client.transform.localPosition} {SpaceManager.spaceContainer.transform.localPosition + client.transform.localPosition}", false);
            }
            //DebugConsole.Log($"{sPObject != null && !sPObject.isLocalPlayerControll && client != null} {syncGlobalPos}", false);
        }
        transform.localPosition = position;
    }
}
