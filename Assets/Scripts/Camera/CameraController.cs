using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraController : MonoBehaviour
{
    public Camera curCamera;
    public byte type;
    void Start()
    {
        curCamera = GetComponent<Camera>();
    }

    void Update()
    {
    }
}
