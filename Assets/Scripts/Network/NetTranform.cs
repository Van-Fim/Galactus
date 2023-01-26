using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetTranform : NetworkTransform
{
    protected override void OnClientToServerSync(Vector3? position, Quaternion? rotation, Vector3? scale)
    {
        base.OnClientToServerSync(position,rotation,scale);
    }
    protected override void OnServerToClientSync(Vector3? position, Quaternion? rotation, Vector3? scale)
    {
        position = Vector3.zero;
        base.OnServerToClientSync(position,rotation,scale);
    }
}
