using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class SPObject : NetworkBehaviour
{
    public GameObject main;

    public bool isPlayerControll;
    public NetworkTransform networkTransform;

    public Controller controller;

    [SyncVar]
    public int galaxyId;
    [SyncVar]
    public int systemId;
    [SyncVar]
    public int sectorId;
    [SyncVar]
    public int zoneId;

    [SyncVar]
    public string modelPatch;

    [SyncVar]
    public string templateName;

    public Rigidbody rigidbodyMain;

    public static UnityAction OnRender;

    public bool isInitialized = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        OnRender += Render;
        isInitialized = true;
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
    }

    public override void OnStartClient()
    {
        if (!isInitialized)
        {
            Init();
            InvokeRender();
        }
    }

    public void Render()
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

        if (NetClient.localClient.galaxyId == galaxyId && NetClient.localClient.systemId == systemId)
        {
            main.SetActive(true);
            networkTransform.enabled = true;
        }
        else
        {
            main.SetActive(false);
            networkTransform.enabled = false;
        }
    }

    public static void InvokeRender()
    {
        OnRender?.Invoke();
    }
}
