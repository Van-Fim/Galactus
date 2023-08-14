using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SpaceObjectManager
{
    public static List<SpaceObject> spaceObjects = new List<SpaceObject>();
    public static SpaceObjectManager singleton;
    public static void LoadGameStartObjects(NetClient netClient)
    {
        GameStartData gm = GameStartManager.GetGameStart(netClient.characterData.gameStart);
        netClient.characterData.isGameStartDataLoaded = true;
        if (gm.spaceObjectDatas.Count > 0)
        {
            for (int i = 0; i < gm.spaceObjectDatas.Count; i++)
            {
                Data.SpaceObjectData spaceObjectData = gm.spaceObjectDatas[i];
                SpaceObject spaceObject = GameObject.Instantiate(GamePrefabsManager.singleton.LoadPrefab<SpaceObject>("SpaceObjectPrefab"));
                spaceObject.galaxyId = spaceObjectData.galaxyId;
                spaceObject.systemId = spaceObjectData.systemId;
                spaceObject.sectorId = spaceObjectData.sectorId;
                spaceObject.zoneId = spaceObjectData.zoneId;
                spaceObject.templateName = spaceObjectData.templateName;
                spaceObject.type = spaceObjectData.type;
                spaceObject.LoadValues();
                spaceObjects.Add(spaceObject);
                NetworkServer.Spawn(spaceObject.gameObject);
                if (spaceObjectData.isStartObject)
                {
                    NetClient.controlledObject = spaceObject;
                    spaceObject.netIdentity.AssignClientAuthority(netClient.connectionToClient);
                    spaceObject.isPlayerControll = true;
                    PlayerController pc = spaceObject.gameObject.AddComponent<PlayerController>();
                    spaceObject.LoadHardpoints();
                    spaceObject.InstallMainCamera();
                }
            }
        }
    }
}