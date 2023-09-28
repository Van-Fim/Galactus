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
        
        netClient.SetIsGameStartDataLoaded(true);
        if (gm.spaceObjectDatas.Count > 0)
        {
            for (int i = 0; i < gm.spaceObjectDatas.Count; i++)
            {
                Data.SpaceObjectData spaceObjectData = gm.spaceObjectDatas[i];
                SpaceObject spaceObject = spaceObjectData.CreateByType();
                if (spaceObjectData.galaxyId == netClient.GetGalaxyId() && spaceObjectData.sectorId == netClient.GetSectorId())
                {
                    spaceObjectData.SetSectorIndexes(netClient.GetSectorIndexes());
                    if (spaceObjectData.zoneId == netClient.GetZoneId())
                    {
                        spaceObjectData.SetZoneIndexes(netClient.GetZoneIndexes());
                    }
                }
                spaceObject.galaxyId = spaceObjectData.galaxyId;
                spaceObject.systemId = spaceObjectData.systemId;
                spaceObject.sectorId = spaceObjectData.sectorId;
                spaceObject.sectorIndexes = spaceObjectData.sectorIndexes;
                spaceObject.zoneIndexes = spaceObjectData.zoneIndexes;
                spaceObject.zoneId = spaceObjectData.zoneId;
                spaceObject.templateName = spaceObjectData.templateName;
                spaceObject.LoadValues();
                spaceObjects.Add(spaceObject);
                spaceObject.LoadHardpoints();
                NetworkServer.Spawn(spaceObject.gameObject);
                if (spaceObjectData.isStartObject)
                {
                    spaceObject.netIdentity.AssignClientAuthority(netClient.connectionToClient);
                    spaceObject.isPlayerControll = true;
                    spaceObject.characterLogin = netClient.characterData.login;
                    spaceObjectData.characterLogin = netClient.characterData.login;
                    spaceObjectData.netId = spaceObject.netId;
                    netClient.ControlledObject = spaceObject;
                    netClient.controlledObjectId = spaceObject.id;
                    netClient.RenderLocal(spaceObjectData);
                }
                spaceObject.ServerInit();
            }
        }
    }
}