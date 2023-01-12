using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SPObject : MonoBehaviour
{
    public GameObject main;

    public bool isPlayerControll;
    //public Controller controller;

    public int galaxyId;
    public int systemId;
    public int sectorId;
    public int zoneId;
    public string modelPatch;
    public string templateName;

    public Rigidbody rigidbodyMain;

    public Controller controller;

    public static UnityAction OnRenderAction;

    public bool isInitialized = false;

    public virtual void Init()
    {
        OnRenderAction += OnRender;
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

    public void Start()
    {
        if (!isInitialized)
        {
            Init();
            InvokeRender();
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

        if (Client.localClient.galaxyId == galaxyId && Client.localClient.systemId == systemId && Client.localClient.sectorId == sectorId)
        {
            main.SetActive(true);
        }
        else
        {
            main.SetActive(false);
        }
    }

    public static void InvokeRender()
    {
        OnRenderAction?.Invoke();
    }
}