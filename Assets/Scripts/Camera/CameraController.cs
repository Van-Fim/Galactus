using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public void SetActive(bool active)
    {
        Camera camera = this.GetComponent<Camera>();
        camera.enabled = active;
        gameObject.SetActive(active);
    }
}
