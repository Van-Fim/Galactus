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
        mainCamera = GamePrefabsManager.singleton.LoadPrefab<CameraController>("MainCamera");
        mainCamera = Instantiate(mainCamera);
        skyBoxCamera = GamePrefabsManager.singleton.LoadPrefab<CameraController>("SkyboxCamera");
        skyBoxCamera = Instantiate(skyBoxCamera);
        skyBoxCamera.enabled = false;
        skyBoxCamera.transform.SetParent(mainCamera.transform);
        skyBoxCamera.transform.localPosition = Vector3.zero;
        skyBoxCamera.transform.localRotation = Quaternion.identity;
        mapCamera = GamePrefabsManager.singleton.LoadPrefab<CameraController>("MapCamera");
        mapCamera = Instantiate(mapCamera);
    }
    public static void SwitchCamera(CameraController camera)
    {
        mainCamera.gameObject.SetActive(false);
        mapCamera.gameObject.SetActive(false);
        camera.gameObject.SetActive(true);
    }
}
