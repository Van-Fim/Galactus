using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public abstract class SPObject : NetworkBehaviour
{
    public GameObject main;
    [SyncVar]
    public bool isPlayerControll;
    [SyncVar]
    public bool isCanSyncPos;
    //public Controller controller;
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
    [SyncVar]
    public Vector3 globalPos;

    public NetTranform netTranform;

    public Rigidbody rigidbodyMain;

    public Controller controller;

    public static UnityAction OnRenderAction;
    [SyncVar]
    public bool isInitialized = false;
    [SyncVar]
    public bool syncGlobalPos = false;

    public void Update()
    {
        if (syncGlobalPos)
        {
            if (main != null && SpaceManager.spaceContainer != null)
            {
                globalPos = currSector.GetPosition() + currZone.GetPosition();
            }
        }
    }

    public virtual void Init()
    {
        OnRenderAction += OnRender;
        isInitialized = true;
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