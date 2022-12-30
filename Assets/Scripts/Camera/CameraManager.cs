using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
    public static CameraController mainCamera;
    public static CameraController minimapCamera;
    public static CameraController skyboxCamera;

    public static byte currentCameraCode = 0;

    public static void Init()
    {
        CameraManager.mainCamera = GameObject.Instantiate(GameContentManager.mainCameraPrefab);
        CameraManager.skyboxCamera = GameObject.Instantiate(GameContentManager.skyboxCameraPrefab, CameraManager.mainCamera.transform);
        CameraManager.minimapCamera = GameObject.Instantiate(GameContentManager.minimapCameraPrefab);

        GameObject.DontDestroyOnLoad(CameraManager.mainCamera);
        GameObject.DontDestroyOnLoad(CameraManager.minimapCamera);
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

            MinimapPanel.currentGalaxyId = NetClient.localClient.galaxyId;
            MinimapPanel.currentSystemId = NetClient.localClient.systemId;
        }
    }
}
