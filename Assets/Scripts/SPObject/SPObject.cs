using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class SPObject : NetworkBehaviour
{
    public Collider boxCollider;
    public MeshRenderer meshRenderer;

    public int galaxyId;
    public int systemId;

    public static UnityAction OnRender;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartClient()
    {
        OnRender += Render;
    }

    public void Render()
    {
        if (Player.galaxyId == galaxyId && Player.systemId == systemId)
        {
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
        }
    }

    public static void InvokeRender()
    {
        OnRender?.Invoke();
    }
}
