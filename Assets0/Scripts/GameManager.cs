using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.VisualScripting;
using Data;
public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public static CanvasController canvasController;
    public static NetworkManager networkManager;
    public static string seed = "Start Seed";
    public void Awake()
    {
        Application.targetFrameRate = 60;
        GamePrefabsManager.Init();

        singleton = this;
        SpaceManager.Init();

        SpaceManager.singleton.GenerateUniverse();

        CameraManager.Init();
        CameraManager.SwitchCamera(CameraManager.mainCamera);
        canvasController = GamePrefabsManager.singleton.LoadPrefab<CanvasController>("Canvas");
        canvasController = GameObject.Instantiate(canvasController);

        IND_targetManager.Init();

        GameStartManager.InitStart("Start01");
        for (int s = 0; s < SpaceManager.singleton.spaceSystems.Count; s++)
        {
            SpaceSystem spaceSystem = SpaceManager.singleton.spaceSystems[s];
            LoadTemplateData(spaceSystem, "system");
        }
        LocalClient.controlledObject.WarpSystem(LocalClient.spaceSystem);

        if (LocalClient.controlledObject != null)
        {
            SpaceObject cobj = LocalClient.controlledObject;
            LocalClient.controlledObject.isInitialized = true;
            LocalClient.controlledObject.isPlayerControll = true;
            Hardpoint camHP = cobj.GetHardpointByType("camera");
            CameraManager.mainCamera.IsCamEnabled = false;
            CameraManager.mainCamera.transform.SetParent(cobj.main.transform);
            CameraManager.mainCamera.transform.localPosition = camHP.GetPosition();
            CameraManager.mainCamera.transform.localEulerAngles = camHP.GetRotation();

            SOShipController controller = cobj.AddComponent<SOShipController>();
            controller.obj = cobj;
            cobj.transform.SetParent(null);
        }

        PositionFixer.Init();
    }
    public static void LoadTemplateData(Space space, string templateType)
    {
        List<SpaceObjectData> spaceObjectDatas = new List<SpaceObjectData>();
        string templateDataName = space.templateName;
        spaceObjectDatas = LoadTemplateContentData(space, templateType);

        for (int i = 0; i < spaceObjectDatas.Count; i++)
        {
            Data.SpaceObjectData data = spaceObjectDatas[i];
            if (data.type == "gate")
            {
                Gate obj = Gate.Create(SpaceManager.singleton, data);
                obj.transform.localPosition = data.GetPosition();
                obj.transform.localEulerAngles = data.GetRotation();
                obj.Init();
                obj.LoadHardpoints();
                Gate fgate = SpaceManager.singleton.gates.Find(f => f.galaxyId == data.targetGalaxyId && f.systemId == data.targetSystemId && f.inSystemGateId == data.targetGateId);
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
    public static List<SpaceObjectData> LoadTemplateContentData(Space space, string templateType)
    {
        string templateDataName = space.templateName;
        int galaxyId = 0;
        int systemId = 0;
        if (space is SpaceSystem)
        {
            SpaceSystem spaceSystemsystem = (SpaceSystem)space;
            galaxyId = spaceSystemsystem.galaxyId;
            systemId = spaceSystemsystem.id;
        }
        List<SpaceObjectData> ret = new List<SpaceObjectData>();
        Template template = TemplateManager.FindTemplate(templateDataName, templateType);
        List<TemplateNode> spaceNodes = template.GetNodeList("space");
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
                while (SpaceObjectManager.spaceObjects.Find(f => f.id == id) != null)
                {
                    id++;
                }
                spaceObjectData.id = id;

                ret.Add(spaceObjectData);
            }
        }

        return ret;
    }
    void Update()
    {
        SpaceObject.InvokeIndRender();
    }
}
