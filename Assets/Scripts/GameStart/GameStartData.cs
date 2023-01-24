using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameStartData
{
    public int id;
    public string name;
    public int galaxyId;
    public int starSystemId;
    public int sectorId;
    public int zoneId;

    public string type;
    public string shipTemplatename;
    public string pilotTemplatename;

    public static GameStartData Init(string name)
    {
        GameStartData gameStartData = new GameStartData();
        gameStartData.name = name;
        gameStartData.GetStartSpace();
        gameStartData.GetStartType();
        return gameStartData;
    }

    public void GetStartSpace()
    {
        Template template = TemplateManager.FindTemplate(name, "start");
        if (template == null)
        {
            Debug.LogError($"Error template {name} not found");
            return;
        }

        if (template == null)
        {
            Debug.LogError($"Template {name} not found");
            return;
        }

        List<TemplateNode> spaceNodes = template.GetNodeList("space");

        for (int i = 0; i < spaceNodes.Count; i++)
        {
            TemplateNode spaceNode = spaceNodes[i];
            galaxyId = int.Parse(spaceNode.GetValue("galaxy"));
            starSystemId = int.Parse(spaceNode.GetValue("system"));
            sectorId = int.Parse(spaceNode.GetValue("sector"));
            zoneId = int.Parse(spaceNode.GetValue("zone"));

            List<TemplateNode> shipNodes = template.GetNodeList("ship");
            List<TemplateNode> playerNodes = template.GetNodeList("player");

            for (int j = 0; j < shipNodes.Count; j++)
            {
                TemplateNode shipNode = shipNodes[j];
                bool plyShipExist = System.Convert.ToBoolean(int.Parse(shipNode.GetValue("playerShip")));

                if (plyShipExist)
                {
                    return;
                }
            }
            if (playerNodes.Count > 0)
            {
                return;
            }
        }
    }

    public void GetStartType()
    {
        Template template = TemplateManager.FindTemplate(name, "start");
        if (template == null)
        {
            Debug.LogError("Error template not found");
            return;
        }

        if (template == null)
        {
            Debug.LogError("Template " + name + " not found");
            return;
        }

        List<TemplateNode> spaceNodes = template.GetNodeList("space");

        for (int i = 0; i < spaceNodes.Count; i++)
        {
            List<TemplateNode> shipNodes = template.GetNodeList("ship");
            List<TemplateNode> playerNodes = template.GetNodeList("player");

            for (int j = 0; j < shipNodes.Count; j++)
            {
                TemplateNode shipNode = shipNodes[j];
                bool plyShipExist = System.Convert.ToBoolean(int.Parse(shipNode.GetValue("playerShip")));

                if (plyShipExist)
                {
                    type = "ship";
                    return;
                }
            }
            if (playerNodes.Count > 0)
            {
                type = "pilot";
                return;
            }
        }
    }

    public void LoadContent(Client client)
    {
        Template template = TemplateManager.FindTemplate(name, "start");
        if (template == null)
        {
            Debug.LogError("Error template not found");
            return;
        }

        if (template == null)
        {
            Debug.LogError("Template " + name + " not found");
            return;
        }

        List<TemplateNode> spaceNodes = template.GetNodeList("space");

        for (int i = 0; i < spaceNodes.Count; i++)
        {
            TemplateNode spaceNode = spaceNodes[i];

            List<TemplateNode> shipNodes = template.GetNodeList("ship");
            List<TemplateNode> pilotNodes = template.GetNodeList("player");

            Galaxy galaxy = SpaceManager.GetGalaxyByID(galaxyId);
            StarSystem system = SpaceManager.GetSystemByID(galaxyId, starSystemId);
            Sector sector = SpaceManager.GetSectorByID(galaxyId, starSystemId, sectorId);
            Zone zone = SpaceManager.GetZoneByID(galaxyId, starSystemId, sectorId, zoneId);

            for (int j = 0; j < shipNodes.Count; j++)
            {
                Vector3 position = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                TemplateNode shipNode = shipNodes[j];

                TemplateNode loadoutNode = shipNode.GetChildNode("loadouts");
                if (loadoutNode != null)
                {

                }
                TemplateNode positionNode = shipNode.GetChildNode("position");
                if (positionNode != null)
                {
                    int x = int.Parse(positionNode.GetValue("x"));
                    int y = int.Parse(positionNode.GetValue("y"));
                    int z = int.Parse(positionNode.GetValue("z"));

                    position = new Vector3(x, y, z);
                }
                TemplateNode rotationNode = shipNode.GetChildNode("rotation");
                if (rotationNode != null)
                {
                    int x = int.Parse(rotationNode.GetValue("x"));
                    int y = int.Parse(rotationNode.GetValue("y"));
                    int z = int.Parse(rotationNode.GetValue("z"));

                    rotation = new Vector3(x, y, z);
                }
                bool plyShipExist = System.Convert.ToBoolean(int.Parse(shipNode.GetValue("playerShip")));
                string templateStringName = shipNode.GetValue("template");
                Ship ship = Ship.Create(templateStringName);
                ship.SetSpace(zone);
                ship.transform.localPosition = position;
                NetworkServer.Spawn(ship.gameObject, client.connectionToClient);
                SPObjectManager.singleton.shipsIds.Add(ship.netId);
            }
            if (type == "pilot")
            {
                Vector3 position = Vector3.zero;
                Vector3 rotation = Vector3.zero;
                for (int j = 0; j < pilotNodes.Count; j++)
                {
                    TemplateNode pilotNode = pilotNodes[j];
                    TemplateNode positionNode = pilotNode.GetChildNode("position");
                    if (positionNode != null)
                    {
                        int x = int.Parse(positionNode.GetValue("x"));
                        int y = int.Parse(positionNode.GetValue("y"));
                        int z = int.Parse(positionNode.GetValue("z"));

                        position = new Vector3(x, y, z);
                    }
                    TemplateNode rotationNode = pilotNode.GetChildNode("rotation");
                    if (rotationNode != null)
                    {
                        int x = int.Parse(rotationNode.GetValue("x"));
                        int y = int.Parse(rotationNode.GetValue("y"));
                        int z = int.Parse(rotationNode.GetValue("z"));

                        rotation = new Vector3(x, y, z);
                    }
                    string templateStringName = pilotNode.GetValue("template");
                    Pilot pilot = Pilot.Create(templateStringName);
                    pilot.SetSpace(zone);
                    pilot.transform.localPosition = position;
                    pilot.isPlayerControll = true;
                    NetworkServer.Spawn(pilot.gameObject, client.connectionToClient);
                    SPObjectManager.singleton.pilotsIds.Add(pilot.netId);
                    client.pilotId = pilot.netId;
                }
            }
        }
    }
}