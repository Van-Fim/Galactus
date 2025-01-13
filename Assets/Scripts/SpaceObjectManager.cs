using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObjectManager : MonoBehaviour
{
    public static SpaceObjectManager singleton;
    public static List<SpaceObject> spaceObjects = new List<SpaceObject>();

    public void LateUpdate()
    {
        if (LocalClient.ControlledObject != null)
        {
            SpaceObject.InvokeIndRender();
        }
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

                Template fTemplate = TemplateManager.FindTemplate(templateStringName, spaceObjectData.type);
                spaceObjectData.hardpointsTemplateName = fTemplate.GetValue("hardpoints", "name");
                spaceObjectData.mass = int.Parse(fTemplate.GetValue("params", "mass"));
                spaceObjectData.drag = int.Parse(fTemplate.GetValue("params", "drag"));
                spaceObjectData.scaleFactor = XMLF.FloatVal(fTemplate.GetValue("params", "scale"));
                if (spaceObjectData.scaleFactor == 0)
                {
                    spaceObjectData.scaleFactor = 1;
                }
                spaceObjectData.angulardrag = int.Parse(fTemplate.GetValue("params", "angulardrag"));
                spaceObjectData.modelPatch = fTemplate.GetValue("model", "patch");
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
                string region = spaceNode.GetValue("region");
                int systemId = int.Parse(spaceNode.GetValue("system"));
                int galaxyId = int.Parse(spaceNode.GetValue("galaxy"));
                int sectorId = int.Parse(spaceNode.GetValue("sector"));
                Region reg = null;
                if (region.Length > 0)
                {
                    reg = SpaceManager.regions.Find(x => x.templateName == region);
                    if (reg != null && reg.spaceSystems.Count > 0)
                    {
                        int rd = Random.Range(0, reg.spaceSystems.Count + 1);
                        systemId = reg.spaceSystems[rd].id;
                        sectorId = 0;
                    }
                }
                plyShipExist = System.Convert.ToBoolean(byte.Parse(objectNode.GetValue("playerShip")));
                if (plyShipExist)
                {
                    LocalClient.galaxyId = galaxyId;
                    LocalClient.systemId = systemId;
                    LocalClient.sectorId = sectorId;

                    if (reg != null)
                    {
                        GalaxyChunkController.galaxyTemplate = TemplateManager.FindTemplate(LocalClient.Galaxy.templateName, "galaxy");
                        GalaxyChunkController.systemNodes = GalaxyChunkController.galaxyTemplate.GetNodeList("system");
                        int Ymax = int.Parse(GalaxyChunkController.galaxyTemplate.GetValue("galaxy", "Ymax"));
                        int Ymin = int.Parse(GalaxyChunkController.galaxyTemplate.GetValue("galaxy", "Ymin"));
                        int maxRange = int.Parse(GalaxyChunkController.galaxyTemplate.GetValue("galaxy", "maxRange"));
                        Bounds b = reg.bounds;
                        Vector3 target = new Vector3(UnityEngine.Random.Range(b.min.x, b.max.x), UnityEngine.Random.Range(b.min.y, b.max.y), UnityEngine.Random.Range(b.min.z, b.max.z));
                        Vector3 p = reg.bounds.ClosestPoint(target);
                        int tryCount = 10;
                        while (tryCount > 0)
                        {
                            tryCount--;
                            bool cnt = false;
                            if (p.y > Ymax || p.y < Ymin)
                            {
                                cnt = true;
                            }
                            if (Vector3.Distance(Vector3.zero, p) > maxRange)
                            {
                                cnt = true;
                            }
                            if (cnt)
                            {
                                target = new Vector3(UnityEngine.Random.Range(b.min.x, b.max.x), UnityEngine.Random.Range(b.min.y, b.max.y), UnityEngine.Random.Range(b.min.z, b.max.z));
                                p = reg.bounds.ClosestPoint(target);
                                continue;
                            }
                            break;
                        }
                        GalaxyChunkController.in_process = true;
                        GalaxyChunkController.mapCameraCurrentIndexes = PositionFixer.RecalcPos(p, GalaxyChunkController.chunkSize);
                        GalaxyChunkController.mapCameraIndexes = GalaxyChunkController.mapCameraCurrentIndexes;
                        ChunkManager chunkManager = ChunkManager.Create(GalaxyChunkController.chunkSize);
                        chunkManager.UpdateChunks(GalaxyChunkController.mapCameraIndexes, true);
                    }
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

                Template fTemplate = TemplateManager.FindTemplate(templateStringName, spaceObjectData.type);
                spaceObjectData.hardpointsTemplateName = fTemplate.GetValue("hardpoints", "name");
                spaceObjectData.mass = int.Parse(fTemplate.GetValue("params", "mass"));
                spaceObjectData.drag = int.Parse(fTemplate.GetValue("params", "drag"));
                spaceObjectData.scaleFactor = XMLF.FloatVal(fTemplate.GetValue("params", "scale"));
                if (spaceObjectData.scaleFactor == 0)
                {
                    spaceObjectData.scaleFactor = 1;
                }
                spaceObjectData.angulardrag = int.Parse(fTemplate.GetValue("params", "angulardrag"));
                spaceObjectData.modelPatch = fTemplate.GetValue("model", "patch");

                spaceObjectData.id = SpaceObject.GetId();
                ret.Add(spaceObjectData);
            }
        }
        return ret;
    }
    public static void BuildObjectsByData(List<SpaceObjectData> dataList, GameObject gmobj = null)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            SpaceObjectData data = dataList[i];
            BuildSpaceObject(data, gmobj);
        }
    }
    public static void BuildSpaceObject(SpaceObjectData data, GameObject gmobj = null)
    {
        SpaceObject obj = SpaceObject.Create(data, gmobj);
        obj.Init();
        obj.LoadHardpoints();
        obj.gameObject.name = $"{obj.templateName}_{obj.id}";
        if (data.isPlayerControll)
        {
            if (!LocalClient.skipFuckingControlledObjAndDie)
            {
                LocalClient.ControlledObject = obj;
                LocalClient.skipFuckingControlledObjAndDie = true;
            }
            if (gmobj != null)
            {
                NetSpaceObject net = gmobj.GetComponent<NetSpaceObject>();
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
                LocalClient.ControlledObject = obj;
            }
        }
    }
}
