using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraController mainCamera;
    public static CameraController skyBoxCamera;
    public static CameraController mapCamera;

    public static void Init()
    {
        mainCamera = GamePrefabsManager.LoadPrefab<CameraController>("MainCamera");
        mainCamera = Instantiate(mainCamera);
        mainCamera.gameObject.name = "MainCamera";
        skyBoxCamera = GamePrefabsManager.LoadPrefab<CameraController>("SkyboxCamera");
        skyBoxCamera = Instantiate(skyBoxCamera);
        skyBoxCamera.enabled = false;
        skyBoxCamera.transform.SetParent(mainCamera.transform);
        skyBoxCamera.transform.localPosition = Vector3.zero;
        skyBoxCamera.transform.localRotation = Quaternion.identity;
        skyBoxCamera.gameObject.name = "SkyboxCamera";
        mapCamera = GamePrefabsManager.LoadPrefab<CameraController>("MapCamera");
        mapCamera = Instantiate(mapCamera);
        mapCamera.gameObject.name = "MapCamera";
    }
    public static void SwitchCamera(string cameraName)
    {
        if (cameraName == "MainCamera")
        {
            SwitchCamera(mainCamera);
        }
        else if (cameraName == "MapCamera")
        {
            SwitchCamera(mapCamera);
        }
        else if (cameraName == "SkyboxCamera")
        {
            SwitchCamera(skyBoxCamera);
        }
    }
    public static void SwitchCamera(CameraController camera)
    {
        mainCamera.gameObject.SetActive(false);
        mapCamera.gameObject.SetActive(false);
        camera.gameObject.SetActive(true);
    }
}
