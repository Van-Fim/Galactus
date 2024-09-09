using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetObj : NetworkBehaviour
{
    public SpaceObject spaceObject;
    public override void OnStartClient()
    {
        if (!isOwned || !isClient)
        {
            return;
        }
        else
        {
            // SOController controller = GetComponent<SOController>();
            // if (controller != null)
            // {
            //     controller.Init();
            // }
            // if (spaceObject != null)
            // {
            //     SpaceObjectData data = GameManager.singleton.spaceObjectDatas.Find(x => x.id == spaceObject.id);
            //     data.SetPosition(new Vector3(20000, 0, 0));
            //     SendData(data);
            // }
            Debug.Log(NetworkConnectionToClient.LocalConnectionId);
        }
    }
    [Command(requiresAuthority = false)]
    public void SendData(SpaceObjectData data)
    {
        Debug.Log(data.GetPosition());
    }
}
