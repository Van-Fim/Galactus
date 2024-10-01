using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraController mainCamera;
    public static Camera planetCamera;
    public static CameraController skyBoxCamera;
    public static CameraController mapCamera;
    public static CameraManager singleton;

    public static void Init()
    {
        mainCamera = GamePrefabsManager.LoadPrefab<CameraController>("MainCamera");
        mainCamera = Instantiate(mainCamera);
        mainCamera.gameObject.name = "MainCamera";
        DontDestroyOnLoad(mainCamera);
        planetCamera = GamePrefabsManager.LoadPrefab<Camera>("PlanetCamera");
        planetCamera = Instantiate(planetCamera);
        planetCamera.gameObject.name = "PlanetCamera";
        DontDestroyOnLoad(planetCamera);
        skyBoxCamera = GamePrefabsManager.LoadPrefab<CameraController>("SkyboxCamera");
        skyBoxCamera = Instantiate(skyBoxCamera);
        skyBoxCamera.enabled = false;
        skyBoxCamera.transform.SetParent(mainCamera.transform);
        skyBoxCamera.transform.localPosition = Vector3.zero;
        skyBoxCamera.transform.localRotation = Quaternion.identity;
        skyBoxCamera.gameObject.name = "SkyboxCamera";
        DontDestroyOnLoad(skyBoxCamera);
        mapCamera = GamePrefabsManager.LoadPrefab<CameraController>("MapCamera");
        mapCamera = Instantiate(mapCamera);
        mapCamera.gameObject.name = "MapCamera";
        DontDestroyOnLoad(mapCamera);
        singleton = GameManager.singleton.AddComponent<CameraManager>();
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
    public void LateUpdate()
    {
        if (mainCamera != null && planetCamera != null && SpaceManager.singleton != null && LocalClient.ControlledObject != null)
        {
            Sector sec = LocalClient.Sector;

            planetCamera.transform.rotation = mainCamera.transform.rotation;
            skyBoxCamera.transform.rotation = mainCamera.transform.rotation;
            Vector3 sPos = (LocalClient.ControlledObject.GetSectorIndexes() * PositionFixer.sectorStepSize);
            Vector3 cPos = mainCamera.transform.position / SolarObject.scaleFactor;
            Vector3 zPos = (LocalClient.ControlledObject.GetZoneIndexes() * PositionFixer.stepSize)/ SolarObject.scaleFactor;
            CameraManager.planetCamera.transform.localPosition = zPos + cPos;
            CameraManager.skyBoxCamera.transform.SetParent(SpaceManager.galaxyContainer.transform);
            CameraManager.skyBoxCamera.transform.localPosition = LocalClient.SpaceSystem.GetPosition();
        }
    }
}
