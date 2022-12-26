using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
    public static CameraController mainCamera;
    public static CameraController minimapCamera;

    public static byte currentCameraCode = 0;

    public static void Init()
    {
        CameraManager.mainCamera = GameObject.Instantiate(GameContentManager.mainCameraPrefab);
        CameraManager.minimapCamera = GameObject.Instantiate(GameContentManager.minimapCameraPrefab);

        GameObject.DontDestroyOnLoad(CameraManager.mainCamera);
    }

    public static void SwitchByCode(byte code){
        currentCameraCode = code;
        if (currentCameraCode == 0)
        {
            mainCamera.GetComponent<Camera>().enabled = true;
            minimapCamera.GetComponent<Camera>().enabled = false;
        }
        else if (currentCameraCode == 1)
        {
            mainCamera.GetComponent<Camera>().enabled = false;
            minimapCamera.GetComponent<Camera>().enabled = true;
        }
    }
}
