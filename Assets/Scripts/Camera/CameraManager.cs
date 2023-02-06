using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager
{
    public static CameraController mainCamera;
    public static CameraController minimapCamera;
    public static PlanetCamera planetCamera;
    public static Camera skyboxCamera;

    public static byte currentCameraCode = 0;

    public static void Init()
    {
        CameraManager.mainCamera = GameObject.Instantiate(GameContentManager.mainCameraPrefab);
        CameraManager.skyboxCamera = GameObject.Instantiate(GameContentManager.skyboxCameraPrefab, CameraManager.mainCamera.transform);
        CameraManager.minimapCamera = GameObject.Instantiate(GameContentManager.minimapCameraPrefab);
        CameraManager.planetCamera = GameObject.Instantiate(GameContentManager.planetCameraPrefab);

        GameObject.DontDestroyOnLoad(CameraManager.mainCamera);
        GameObject.DontDestroyOnLoad(CameraManager.minimapCamera);
        GameObject.DontDestroyOnLoad(CameraManager.planetCamera);

        CameraController.startCamPositions.Add(new Vector3(0, 1500, 0));
        CameraController.startCamPositions.Add(new Vector3(0, 2000, 0));
        CameraController.startCamPositions.Add(new Vector3(0, 10000, 0));
        CameraController.startCamPositions.Add(new Vector3(0, 800, 0));

        RenderTexture camTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        camTexture.Create();
        CameraManager.minimapCamera.curCamera.targetTexture = camTexture;
        CameraManager.minimapCamera.curCamera.Render();
        SwitchByCode(0);
    }

    public static void SwitchByCode(byte code)
    {
        currentCameraCode = code;
        if (currentCameraCode == 0)
        {
            mainCamera.enabled = false;
            mainCamera.curCamera.enabled = true;

            minimapCamera.enabled = false;
            minimapCamera.curCamera.enabled = false;
        }
        else if (currentCameraCode == 1)
        {
            mainCamera.enabled = false;
            mainCamera.curCamera.enabled = true;

            minimapCamera.enabled = true;
            minimapCamera.curCamera.enabled = true;
        }
    }
}
