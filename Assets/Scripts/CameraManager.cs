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
        planetCamera = mainCamera.transform.Find("PlanetCamera").gameObject.GetComponent<Camera>();
        planetCamera.transform.SetParent(null);
        planetCamera.gameObject.name = "PlanetCamera";
        DontDestroyOnLoad(planetCamera);
        skyBoxCamera = mainCamera.transform.Find("SkyboxCamera").gameObject.GetComponent<CameraController>();
        skyBoxCamera.transform.SetParent(null);
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
            Space.InvokeMinimapRender();
        }
        else if (cameraName == "MapCamera")
        {
            SwitchCamera(mapCamera);
            MPSystemController mp = LocalClient.SpaceSystem.mp;
            Space.InvokeMinimapRender();
            GalaxyChunkController.galaxyTemplate = TemplateManager.FindTemplate(LocalClient.Galaxy.templateName, "galaxy");
            GalaxyChunkController.systemNodes = GalaxyChunkController.galaxyTemplate.GetNodeList("system");
            if (mp == null || mp.spaceUiObj == null)
            {
                return;
            }
            mp.spaceUiObj.selected = true;
            SpaceUiObj.selectedObj = mp.spaceUiObj;
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
            planetCamera.transform.rotation = mainCamera.transform.rotation;
            skyBoxCamera.transform.rotation = mainCamera.transform.rotation;
            CameraManager.skyBoxCamera.transform.SetParent(SpaceManager.galaxyContainer.transform);
            CameraManager.skyBoxCamera.transform.localPosition = LocalClient.SpaceSystem.GetPosition();
            Vector3 cPos = mainCamera.transform.position / SolarObject.scaleFactor;
            Vector3 zPos = PositionFixer.zoneIndexes * PositionFixer.stepSize;
            CameraManager.planetCamera.transform.localPosition = zPos / SolarObject.scaleFactor + cPos;
            
            GalaxyChunkController.in_process = false;
            GalaxyChunkController.mapCameraCurrentIndexes = PositionFixer.RecalcPos(mapCamera.transform.localPosition, GalaxyChunkController.chunkSize);
            if ((CameraManager.mapCamera.direction.magnitude > 0 && CameraManager.mapCamera.boost <= 3.5f) || CameraManager.mapCamera.direction.magnitude == 0)
            {
                GalaxyChunkController.in_process = true;
            }
            if (GalaxyChunkController.in_process && GalaxyChunkController.mapCameraCurrentIndexes != GalaxyChunkController.mapCameraIndexes)
            {
                GalaxyChunkController.OnFixAction?.Invoke();
            }
        }
    }
}
