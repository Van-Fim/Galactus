using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class SpaceObject : NetworkBehaviour
{
    public int id;
    public GameObject main;
    [SyncVar]
    public string templateName;
    [SyncVar]
    public string type;
    [SyncVar]
    public int galaxyId;
    [SyncVar]
    public int systemId;
    [SyncVar]
    public int sectorId;
    [SyncVar]
    public int zoneId;

    public string hardpointsTemplateName;
    public string modelPatch;

    Galaxy currGalaxy;
    StarSystem currSystem;
    Sector currSector;
    Zone currZone;

    public Rigidbody rigidbodyMain;
    int mass;
    float drag;
    float angulardrag;

    NetworkTransform networkTransform;
    public bool isPlayerControll;
    public bool isInitialized;

    public List<Hardpoint> hardpoints;

    public static UnityAction OnRenderAction;
    public override void OnStartClient()
    {
        Init();
        SpaceObject.InvokeRender();
    }
    public virtual void LoadValues()
    {
        Template template = TemplateManager.FindTemplate(templateName, type);
        TemplateNode hardpointsNode = template.GetNode("hardpoints");
        if (hardpointsNode != null)
        {
            hardpointsTemplateName = hardpointsNode.GetValue("name");
        }
        TemplateNode modelNode = template.GetNode("model");
        if (modelNode != null)
        {
            modelPatch = modelNode.GetValue("patch");
        }
        TemplateNode paramsNode = template.GetNode("params");
        if (paramsNode != null)
        {
            mass = int.Parse(paramsNode.GetValue("mass"));
            drag = XMLF.FloatVal(paramsNode.GetValue("drag"));
            angulardrag = XMLF.FloatVal(paramsNode.GetValue("angulardrag"));
            int maxhull = int.Parse(paramsNode.GetValue("maxhull"));
        }
    }
    public virtual void Init()
    {
        OnRenderAction += OnRender;
        isInitialized = true;
        networkTransform = GetComponent<NetworkTransform>();
        networkTransform.enabled = false;
    }
    public void ReadSpace()
    {
        currGalaxy = SpaceManager.singleton.GetGalaxyByID(galaxyId);
        currSystem = SpaceManager.singleton.GetSystemByID(galaxyId, systemId);
        currSector = SpaceManager.singleton.GetSectorByID(galaxyId, systemId, sectorId);
        currZone = SpaceManager.singleton.GetZoneByID(galaxyId, systemId, sectorId, zoneId);
    }
    public virtual void SetSpace(GameContent.Space space)
    {
        if (space == null)
        {
            DebugConsole.Log($"Error! target space is null!", "error");
            return;
        }
        if (space.GetType() == typeof(Galaxy))
        {
            galaxyId = space.id;
        }
        else if (space.GetType() == typeof(StarSystem))
        {
            StarSystem sp = (StarSystem)space;
            galaxyId = sp.galaxyId;
            systemId = sp.id;
        }
        else if (space.GetType() == typeof(Sector))
        {
            Sector sp = (Sector)space;
            galaxyId = sp.galaxyId;
            systemId = sp.systemId;
            sectorId = sp.id;
        }
        else if (space.GetType() == typeof(Zone))
        {
            Zone sp = (Zone)space;
            galaxyId = sp.galaxyId;
            systemId = sp.systemId;
            sectorId = sp.sectorId;
            zoneId = sp.id;
        }
        ReadSpace();
    }
    public virtual void LoadHardpoints()
    {
        if (hardpointsTemplateName.Length == 0)
        {
            return;
        }
        Template temp = TemplateManager.FindTemplate(hardpointsTemplateName, "hardpoints");
        if (temp == null)
        {
            DebugConsole.Log("Hardpoint template " + hardpointsTemplateName + " not found", "error");
            return;
        }
        List<TemplateNode> nodes = temp.GetNodeList("hardpoint");
        for (int i = 0; i < nodes.Count; i++)
        {
            TemplateNode node = nodes[i];
            int id = int.Parse(node.GetValue("id"));
            string type = node.GetValue("type");
            TemplateNode positionNode = node.GetChildNode("position");
            TemplateNode rotationNode = node.GetChildNode("rotation");
            Vector3 position = Vector3.zero;
            Vector3 rotation = Vector3.zero;
            if (position != null)
            {
                float x = XMLF.FloatVal(positionNode.GetValue("x"));
                float y = XMLF.FloatVal(positionNode.GetValue("y"));
                float z = XMLF.FloatVal(positionNode.GetValue("z"));
                position = new Vector3(x, y, z);
            }
            if (rotation != null)
            {
                float x = XMLF.FloatVal(rotationNode.GetValue("x"));
                float y = XMLF.FloatVal(rotationNode.GetValue("y"));
                float z = XMLF.FloatVal(rotationNode.GetValue("z"));
                rotation = new Vector3(x, y, z);
            }
            Hardpoint hardpoint = new Hardpoint();
            hardpoint.id = id;
            hardpoint.type = type;
            hardpoint.SetPosition(position);
            hardpoint.SetRotation(rotation);
            hardpoints.Add(hardpoint);
        }
    }
    public virtual Hardpoint GetHardpointByType(string type)
    {
        Hardpoint retHp = hardpoints.Find(f => f.type == type);
        return retHp;
    }
    public void InstallMainCamera()
    {
        Hardpoint hp = GetHardpointByType("camera");
        if (hp == null)
        {
            DebugConsole.Log("Camera hardpoint not found", "error");
            return;
        }
        CameraManager.mainCamera.enabled = false;
        CameraManager.mainCamera.transform.SetParent(transform);
        CameraManager.mainCamera.transform.localPosition = hp.GetPosition();
        CameraManager.mainCamera.transform.localEulerAngles = hp.GetRotation();
        hp.installed = true;
    }
    public void OnRender()
    {
        if (!enabled)
        {
            return;
        }
        if (modelPatch.Length > 0 && main == null)
        {
            GameObject minst = Resources.Load<GameObject>($"{modelPatch}/MAIN");
            main = Instantiate(minst, gameObject.transform);
        }

        rigidbodyMain = gameObject.AddComponent<Rigidbody>();
        rigidbodyMain.useGravity = false;
        rigidbodyMain.mass = mass;
        rigidbodyMain.drag = drag;
        rigidbodyMain.angularDrag = angulardrag;
    }
    public static void InvokeRender()
    {
        OnRenderAction?.Invoke();
    }
}
