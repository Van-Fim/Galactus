using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;

public class GameStartManager
{
    public static GameStartData LoadGameStart(string gameStartName)
    {
        GameStartData ret = new GameStartData();
        Template template = TemplateManager.FindTemplate(gameStartName, "start");
        List<TemplateNode> spaceNodes = template.GetNodeList("space");
        ret.templateName = gameStartName;
        bool plyShipExist = false;
        for (int i = 0; i < spaceNodes.Count; i++)
        {
            TemplateNode spaceNode = spaceNodes[i];
            List<TemplateNode> allNodes = template.GetNodeList("object");
            List<TemplateNode> shipNodes = template.GetNodeList("ship");
            List<TemplateNode> gateNodes = template.GetNodeList("gate");
            allNodes.AddRange(shipNodes);
            allNodes.AddRange(gateNodes);
            for (int j = 0; j < allNodes.Count; j++)
            {
                TemplateNode objectNode = allNodes[j];
                if (objectNode.ParentNode != spaceNode)
                {
                    continue;
                }
                plyShipExist = System.Convert.ToBoolean(byte.Parse(objectNode.GetValue("playerShip")));
                int galaxyId = int.Parse(spaceNode.GetValue("galaxy"));
                int systemId = int.Parse(spaceNode.GetValue("system"));
                if (plyShipExist)
                {
                    ret.galaxyId = galaxyId;
                    ret.systemId = systemId;
                }
                string templateStringName = objectNode.GetValue("template");
                TemplateNode positionNode = objectNode.GetChildNode("position");
                Vector3 position = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                if (positionNode != null)
                {
                    int x = int.Parse(positionNode.GetValue("x"));
                    int y = int.Parse(positionNode.GetValue("y"));
                    int z = int.Parse(positionNode.GetValue("z"));
                    position = new Vector3(x, y, z);
                    if (plyShipExist)
                    {
                        ret.SetPosition(position);
                    }
                }
                TemplateNode rotationNode = objectNode.GetChildNode("rotation");
                if (rotationNode != null)
                {
                    int x = int.Parse(rotationNode.GetValue("x"));
                    int y = int.Parse(rotationNode.GetValue("y"));
                    int z = int.Parse(rotationNode.GetValue("z"));
                    rotation = new Vector3(x, y, z);
                    if (plyShipExist)
                    {
                        ret.SetRotation(rotation);
                    }
                }

                SpaceObjectData spaceObjectData = new SpaceObjectData();
                TemplateNode destinationGate = objectNode.GetChildNode("destinationGate");
                if (destinationGate != null)
                {
                    int galaxy = int.Parse(rotationNode.GetValue("galaxy"));
                    int system = int.Parse(rotationNode.GetValue("system"));
                    int gate = int.Parse(rotationNode.GetValue("gate"));
                    spaceObjectData.targetGalaxyId = galaxy;
                    spaceObjectData.targetSystemId = system;
                    spaceObjectData.targetGateId = gate;
                }
                spaceObjectData.isPlayerControll = plyShipExist;
                spaceObjectData.isStartObject = plyShipExist;
                spaceObjectData.templateName = templateStringName;
                spaceObjectData.type = objectNode.Node;
                spaceObjectData.SetPosition(position);
                spaceObjectData.SetRotation(rotation);
                spaceObjectData.galaxyId = galaxyId;
                spaceObjectData.systemId = systemId;
                spaceObjectData.id = (uint)ret.spaceObjectDatas.Count;

                ret.spaceObjectDatas.Add(spaceObjectData);
            }
        }

        return ret;
    }
    public static void InitStart(string start)
    {
        GameStartData gameStartData = GameStartManager.LoadGameStart(start);
        LocalClient.gamestartTemplateName = start;
        LocalClient.galaxyId = gameStartData.galaxyId;
        LocalClient.systemId = gameStartData.systemId;
        Galaxy galaxy = SpaceManager.singleton.galaxies.Find(f => f.id == LocalClient.galaxyId);
        SpaceManager.singleton.currentGalaxy = galaxy;
        SpaceManager.singleton.GenerateSystems();
        SpaceSystem spaceSystem = SpaceManager.singleton.spaceSystems.Find(f => f.id == LocalClient.systemId && f.galaxyId == LocalClient.galaxyId);
        LocalClient.spaceSystem = spaceSystem;
        LocalClient.is_gamestart_started = true;

        for (int i = 0; i < gameStartData.spaceObjectDatas.Count; i++)
        {
            Data.SpaceObjectData data = gameStartData.spaceObjectDatas[i];
            if (data.type == "gate")
            {
                Gate obj = Gate.Create(SpaceManager.singleton, data);
                obj.transform.localPosition = data.GetPosition();
                obj.transform.localEulerAngles = data.GetRotation();
                obj.Init();
                obj.LoadHardpoints();
                Gate fgate = SpaceManager.singleton.gates.Find(f=>f.galaxyId == data.targetGalaxyId && f.systemId == data.targetSystemId && f.inSystemGateId == data.targetGateId);
                if (fgate != null)
                {
                    obj.ConnectGate(SpaceManager.singleton, fgate);
                }
                if (data.isPlayerControll)
                {
                    LocalClient.controlledObject = obj;
                }
            }
            if (data.type == "object")
            {
                SpaceObject obj = SpaceObject.Create(SpaceManager.singleton, data);
                obj.Init();
                obj.LoadHardpoints();
                if (data.isPlayerControll)
                {
                    LocalClient.controlledObject = obj;
                }
            }
            if (data.type == "ship")
            {
                Ship ship = (Ship)SpaceObject.Create(SpaceManager.singleton, data);
                ship.Init();
                ship.LoadHardpoints();
                if (data.isPlayerControll)
                {
                    LocalClient.controlledObject = ship;
                }
            }
        }
    }
}
