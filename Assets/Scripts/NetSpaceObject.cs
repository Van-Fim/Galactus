using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Collections;
using UnityEngine;

public class NetSpaceObject : NetworkBehaviour
{
    [SyncVar]
    public bool isPlayer;
    [SyncVar]
    public SpaceObjectData sp;
    public GameObject parentContainer;
    SpaceObject obj;
    public override void OnStartClient()
    {
        StartCoroutine(CheckSpData());
    }
    [TargetRpc]
    public void StartClient()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        LoginData msg = new LoginData { gameStart = "Start01", netId = this };
        NetworkClient.Send(msg);
    }
    IEnumerator CheckSpData()
    {
        while (sp == null || !sp.sendSuccessfull)
            yield return null;
        if (isLocalPlayer && !isServer)
        {
            GameManager.singleton.LoadContent();
        }
        obj = gameObject.GetComponent<SpaceObject>();

        if (obj == null)
        {
            obj = SpaceObject.Create(sp, gameObject);
            obj.id = sp.id;
            obj.Init();
            obj.LoadHardpoints();
        }

        if (isLocalPlayer)
        {
            LocalClient.galaxyId = obj.galaxyId;
            LocalClient.systemId = obj.systemId;
            LocalClient.sectorId = obj.sectorId;
            LocalClient.ControlledObject = obj;
            GameManager.singleton.StartGame();
        }
        else
        {
            if (obj.isPlayerControll)
            {
                parentContainer = new GameObject();
                parentContainer.transform.position = Vector3.zero;
                parentContainer.transform.eulerAngles = Vector3.zero;
                parentContainer.name = $"Player_{obj.id}_container";
                obj.transform.SetParent(parentContainer.transform);
            }
        }
        SpaceObject.InvokeRender();
        Space.InvokeMinimapRender();
        yield return new WaitForEndOfFrame();
    }
    [ClientRpc]
    public void SendBackFixedIndexes(uint netid, Vector3 indexes)
    {
        if (netid == netId)
        {
            obj.SetZoneIndexes(indexes);
        }
        
        if (parentContainer != null)
        {
            parentContainer.transform.position = SpaceManager.spaceContainer.transform.position + (obj.GetZoneIndexes() * PositionFixer.stepSize);
        }
    }
}
