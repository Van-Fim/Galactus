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

    public static UnityAction<byte> OnChangeLayerAction;

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

    void Destroy()
    {
        DestroyImmediate(gameObject);
    }

    public void Init()
    {
        OnChangeLayerAction += OnChangeLayer;
        isInitialized = true;
    }

    public void OnChangeLayer(byte layer)
    {

    }

    public static void InvokeChangeLayer(byte layer)
    {
        OnChangeLayerAction?.Invoke(layer);
    }
}
