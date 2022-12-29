using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class SPObject : NetworkBehaviour
{
    public GameObject main;

    [SyncVar]
    public int galaxyId;
    [SyncVar]
    public int systemId;

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
        if (transform.parent == null)
        {
            transform.SetParent(SpaceManager.spaceContainer.transform);
        }
        
        if (NetClient.localClient.galaxyId == galaxyId && NetClient.localClient.systemId == systemId)
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
        OnRender?.Invoke();
    }
}
