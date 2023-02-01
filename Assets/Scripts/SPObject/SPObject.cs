using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public abstract class SPObject : NetworkBehaviour
{
    public GameObject main;
    public bool isLocalPlayerControll;
    [SyncVar]
    public bool isPlayerControll;
    [SyncVar]
    public bool isCanSyncPos;
    [SyncVar]
    public int galaxyId;
    [SyncVar]
    public int systemId;
    [SyncVar]
    public int sectorId;
    [SyncVar]
    public int zoneId;

    Galaxy currGalaxy;
    StarSystem currSystem;
    Sector currSector;
    Zone currZone;

    [SyncVar]
    public string modelPatch;
    [SyncVar]
    public string templateName;

    public NetTranform netTranform;

    public Rigidbody rigidbodyMain;

    public Controller controller;

    public static UnityAction OnRenderAction;
    [SyncVar]
    public bool isInitialized = false;

    public virtual void Init()
    {
        OnRenderAction += OnRender;
        isInitialized = true;
    }

    public void SetPlayerControll()
    {
        SPObject controllTarget = this;
        controllTarget.isPlayerControll = true;
        controllTarget.galaxyId = galaxyId;
        controllTarget.systemId = systemId;
        controllTarget.sectorId = sectorId;
        controllTarget.zoneId = zoneId;
        Template template = null;
        TemplateNode paramsNode = null;
        if (controllTarget is Pilot)
        {
            template = TemplateManager.FindTemplate(controllTarget.templateName, "pilot");
            paramsNode = template.GetNode("params");

            controllTarget.controller = controllTarget.gameObject.AddComponent<PlayerController>();
            CameraManager.mainCamera.enabled = false;
            CameraManager.mainCamera.transform.SetParent(controllTarget.transform);
            CameraManager.mainCamera.transform.localPosition = new Vector3(0, 1.6f, -3f);
        }
        else if (controllTarget is Ship)
        {
            template = TemplateManager.FindTemplate(controllTarget.templateName, "ship");
            paramsNode = template.GetNode("params");

            controllTarget.controller = controllTarget.gameObject.AddComponent<ShipPlayerController>();
            CameraManager.mainCamera.enabled = false;
            CameraManager.mainCamera.transform.SetParent(controllTarget.transform);
            CameraManager.mainCamera.transform.localPosition = new Vector3(0, 75, -200);
        }
        float scaleMin = XMLF.FloatVal(paramsNode.GetValue("scaleMin"));
        float scaleMax = XMLF.FloatVal(paramsNode.GetValue("scaleMax"));
        float scale = UnityEngine.Random.Range(scaleMin, scaleMax + 1);
        if (scale == 0 || scaleMax == 0)
        {
            scale = XMLF.FloatVal(paramsNode.GetValue("scale"));
            if (scale == 0)
            {
                scale = 1;
            }
        }
        byte scaleMass = byte.Parse(paramsNode.GetValue("scaleMass"));
        controllTarget.rigidbodyMain = controllTarget.gameObject.AddComponent<Rigidbody>();
        controllTarget.rigidbodyMain.useGravity = false;
        controllTarget.rigidbodyMain.angularDrag = int.Parse(paramsNode.GetValue("angulardrag"));
        controllTarget.rigidbodyMain.drag = int.Parse(paramsNode.GetValue("drag"));
        controllTarget.rigidbodyMain.mass = int.Parse(paramsNode.GetValue("mass"));
        controllTarget.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        if (scaleMass > 0)
        {
            controllTarget.rigidbodyMain.mass *= scale;
        }
        Client.localClient.targetId = controllTarget.netId;
        controllTarget.controller.obj = controllTarget;
        controllTarget.isLocalPlayerControll = true;
        Client.localClient.controllTarget = controllTarget;
        Client.localClient.controllTarget.netTranform.syncGlobalPos = true;
        Client.localClient.controllTarget.netTranform.clientId = Client.localClient.netId;
    }

    public void ReadSpace()
    {
        currGalaxy = SpaceManager.GetGalaxyByID(galaxyId);
        currSystem = SpaceManager.GetSystemByID(galaxyId, systemId);
        currSector = SpaceManager.GetSectorByID(galaxyId, systemId, sectorId);
        currZone = SpaceManager.GetZoneByID(galaxyId, systemId, sectorId, zoneId);
    }

    public virtual void SetSpace(Space space)
    {
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

    public override void OnStartClient()
    {
        Init();
        if (!isLocalPlayer)
        {
            isCanSyncPos = true;
        }
    }

    public virtual void OnRender()
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

        if (enabled && transform.parent == null && !isPlayerControll)
        {
            transform.SetParent(SpaceManager.spaceContainer.transform);
        }
        else if (transform.parent == null && isPlayerControll)
        {
            transform.SetParent(null);
        }

        if (!(this is Asteroid))
        {
            if (Client.localClient.galaxyId == galaxyId && Client.localClient.systemId == systemId && Client.localClient.sectorId == sectorId)
            {
                main.SetActive(true);
                netTranform.enabled = true;
            }
            else
            {
                main.SetActive(false);
                netTranform.enabled = false;
            }
        }
        else
        {
            if (Client.localClient.galaxyId == galaxyId && Client.localClient.systemId == systemId)
            {
                main.SetActive(true);
                netTranform.enabled = true;
            }
            else
            {
                main.SetActive(false);
                netTranform.enabled = false;
            }
        }
    }

    public static void InvokeRender()
    {
        OnRenderAction?.Invoke();
    }
}