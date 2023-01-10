using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;

public class SpaceController : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Space space;
    public byte layer = 0;

    public bool isInitialized = false;
    public bool isDestroyed = false;

    public static UnityAction<byte> OnChangeLayer;

    void Start()
    {
        if (!isInitialized)
        {
            Init();
        }
    }

    void Update()
    {
        
    }

    public void Init()
    {
        OnChangeLayer += ChangeLayer;
        isInitialized = true;
    }

    public void ChangeLayer(byte layer)
    {

    }


    public static void InvokeChangeLayer(byte layer)
    {
        OnChangeLayer?.Invoke(layer);
    }
}
