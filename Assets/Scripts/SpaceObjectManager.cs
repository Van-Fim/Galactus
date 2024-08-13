using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObjectManager : MonoBehaviour
{
    public static SpaceObjectManager singleton;
    public static List<SpaceObject> spaceObjects = new List<SpaceObject>();

    public void LateUpdate()
    {
        SpaceObject.InvokeIndRender();
    }
    public static void Init()
    {
        singleton = GameManager.singleton.gameObject.AddComponent<SpaceObjectManager>();
    }
    public static List<SpaceObjectData> ReadSpaceContent(Space space)
    {
        string templateName = space.templateName;
        string templateType = space.GetSpaceType();
        int galaxyId = space.galaxyId;
        int systemId = space.systemId;
        int sectorId = space.sectorId;
        if (templateType == "system")
        {
            if (galaxyId < 0)
            {
                return null;
            }
            systemId = space.id;
        }
        List<SpaceObjectData> ret = new List<SpaceObjectData>();
        Template template = TemplateManager.FindTemplate(templateName, templateType);
        List<TemplateNode> spaceNodes = template.GetNodeList("space");
        if (spaceNodes == null)
        {
            return null;
        }
        bool plyShipExist = false;
        for (int i = 0; i < spaceNodes.Count; i++)
        {
            TemplateNode spaceNode = spaceNodes[i];
            List<TemplateNode> allNodes = template.GetNodeList("spaceobject");
            List<TemplateNode> shipNodes = template.GetNodeList("ship");
            allNodes.AddRange(shipNodes);
            for (int j = 0; j < allNodes.Count; j++)
            {
                TemplateNode objectNode = allNodes[j];
                if (objectNode.ParentNode != spaceNode)
                {
                    continue;
                }

                plyShipExist = System.Convert.ToBoolean(byte.Parse(objectNode.GetValue("playerShip")));
                if (plyShipExist)
                {
                    LocalClient.galaxyId = galaxyId;
                    LocalClient.systemId = systemId;
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
                }
                TemplateNode rotationNode = objectNode.GetChildNode("rotation");
                if (rotationNode != null)
                {
                    int x = int.Parse(rotationNode.GetValue("x"));
                    int y = int.Parse(rotationNode.GetValue("y"));
                    int z = int.Parse(rotationNode.GetValue("z"));
                    rotation = new Vector3(x, y, z);
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
                spaceObjectData.sectorId = sectorId;
                uint id = 0;
                while ((ret.Find(f => f.id == id) != null) || (SpaceObjectManager.spaceObjects.Find(f => f.id == id) != null))
                {
                    id++;
                }
                spaceObjectData.id = id;

                ret.Add(spaceObjectData);
            }
        }
        return ret;
    }
    public static List<SpaceObjectData> ReadSpaceContent(string templateName, string templateType)
    {
        List<SpaceObjectData> ret = new List<SpaceObjectData>();
        Template template = TemplateManager.FindTemplate(templateName, templateType);
        List<TemplateNode> spaceNodes = template.GetNodeList("space");
        if (spaceNodes == null)
        {
            return null;
        }
        bool plyShipExist = false;
        for (int i = 0; i < spaceNodes.Count; i++)
        {
            TemplateNode spaceNode = spaceNodes[i];
            List<TemplateNode> allNodes = template.GetNodeList("spaceobject");
            List<TemplateNode> shipNodes = template.GetNodeList("ship");
            allNodes.AddRange(shipNodes);
            for (int j = 0; j < allNodes.Count; j++)
            {
                TemplateNode objectNode = allNodes[j];
                if (objectNode.ParentNode != spaceNode)
                {
                    continue;
                }
                int galaxyId = int.Parse(spaceNode.GetValue("galaxy"));
                int systemId = int.Parse(spaceNode.GetValue("system"));
                int sectorId = int.Parse(spaceNode.GetValue("sector"));
                plyShipExist = System.Convert.ToBoolean(byte.Parse(objectNode.GetValue("playerShip")));
                if (plyShipExist)
                {
                    LocalClient.galaxyId = galaxyId;
                    LocalClient.systemId = systemId;
                    LocalClient.sectorId = sectorId;
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
                }
                TemplateNode rotationNode = objectNode.GetChildNode("rotation");
                if (rotationNode != null)
                {
                    int x = int.Parse(rotationNode.GetValue("x"));
                    int y = int.Parse(rotationNode.GetValue("y"));
                    int z = int.Parse(rotationNode.GetValue("z"));
                    rotation = new Vector3(x, y, z);
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
                uint id = 0;
                while ((ret.Find(f => f.id == id) != null) || (SpaceObjectManager.spaceObjects.Find(f => f.id == id) != null))
                {
                    id++;
                }
                spaceObjectData.id = id;

                ret.Add(spaceObjectData);
            }
        }
        return ret;
    }
    public static void BuildObjectsByData(List<SpaceObjectData> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            SpaceObjectData data = dataList[i];
            SpaceObject obj = SpaceObject.Create(data);
            obj.Init();
            obj.LoadHardpoints();
            if (data.isPlayerControll)
            {
                LocalClient.controlledObject = obj;
            }
        }
    }
    public static void BuildTemplateData(Space space, string templateType)
    {
        List<SpaceObjectData> spaceObjectDatas = new List<SpaceObjectData>();
        string templateDataName = space.templateName;
        spaceObjectDatas = ReadSpaceContent(space.templateName, space.GetSpaceType());

        for (int i = 0; i < spaceObjectDatas.Count; i++)
        {
            SpaceObjectData data = spaceObjectDatas[i];
            SpaceObject obj = SpaceObject.Create(data);
            obj.Init();
            obj.LoadHardpoints();
            if (data.isPlayerControll)
            {
                LocalClient.controlledObject = obj;
            }
        }
    }
}
