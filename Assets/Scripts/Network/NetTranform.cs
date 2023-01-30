using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetTranform : NetworkTransform
{
    public SPObject sPObject;

    public void Start()
    {
        sPObject = gameObject.GetComponent<SPObject>();
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
        if (SpaceManager.spaceContainer != null && syncGlobalPos)
        {
            if (sPObject != null && !sPObject.isLocalPlayerControll)
            {
                position = SpaceManager.spaceContainer.transform.localPosition + globalPos;
            }
        }
        base.OnServerToClientSync(position, rotation, scale);
    }
}
