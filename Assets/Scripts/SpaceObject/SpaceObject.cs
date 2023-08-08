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
    bool isInitialized;

    public static UnityAction OnRenderAction;
    public override void OnStartClient()
    {
        Init();
        SpaceObject.InvokeRender();
    }
    public virtual void LoadValues()
    {
        Template template = TemplateManager.FindTemplate(templateName, type);
        TemplateNode modelNode = template.GetNode("model");
        if (modelNode != null)
        {
            modelPatch = modelNode.GetValue("patch");
        }
        TemplateNode paramsNode = template.GetNode("model");
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
    public void InstallMainCamera()
    {
        CameraManager.mainCamera.enabled = false;
        CameraManager.mainCamera.transform.SetParent(transform);
        CameraManager.mainCamera.transform.localPosition = new Vector3(0, 1.6f, -3);
        CameraManager.mainCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
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
