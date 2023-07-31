using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraController skyboxCamera;
    public static CameraController planetCamera;
    public static CameraController mainCamera;
    public static MinimapCameraController minimapCamera;
    public static CameraManager singleton;
    static byte currentCameraCode;
    public static void SwitchByCode(byte code)
    {
        currentCameraCode = code;
        if (currentCameraCode == 0)
        {
            mainCamera.SetActive(true);
            minimapCamera.SetActive(false);
        }
        else if (currentCameraCode == 1)
        {
            mainCamera.SetActive(false);
            minimapCamera.SetActive(true);
        }
    }
    public static void Init()
    {
        GamePrefabsManager gmpr = GamePrefabsManager.singleton;
        CameraManager.mainCamera = GameObject.Instantiate(gmpr.LoadPrefab<CameraController>("MainCameraPrefab"));
        CameraManager.planetCamera = GameObject.Instantiate(gmpr.LoadPrefab<CameraController>("PlanetCameraPrefab"));
        CameraManager.skyboxCamera = GameObject.Instantiate(gmpr.LoadPrefab<CameraController>("SkyboxCameraPrefab"));
        CameraManager.minimapCamera = GameObject.Instantiate(gmpr.LoadPrefab<MinimapCameraController>());
        singleton = Instantiate(GamePrefabsManager.singleton.LoadPrefab<CameraManager>("CameraManagerPrefab"));
        singleton.gameObject.name = "CameraManager";
        DontDestroyOnLoad(singleton.gameObject);
        DontDestroyOnLoad(CameraManager.mainCamera.gameObject);
        DontDestroyOnLoad(CameraManager.skyboxCamera.gameObject);
        DontDestroyOnLoad(CameraManager.minimapCamera.gameObject);
        CameraManager.skyboxCamera.transform.SetParent(mainCamera.transform);
        CameraController.startCamPositions.Add(new Vector3(0, 3000, 0));
        CameraController.startCamPositions.Add(new Vector3(0, 1000, 0));
        CameraController.startCamPositions.Add(new Vector3(0, 100000, 0));
        CameraController.startCamPositions.Add(new Vector3(0, 1, 0));
    }
    public static void UpdateCamPos(CameraController camera, Vector3 position, Vector3 rotation)
    {
        camera.enabled = false;
        camera.transform.localPosition = position;
        camera.transform.localEulerAngles = rotation;
        camera.enabled = true;
        camera.curCamera.enabled = true;
    }

    public static void UpdateCamBoost(CameraController camera, float boost)
    {
        camera.enabled = false;
        camera.boost = boost;
        camera.enabled = true;
        camera.curCamera.enabled = true;
    }
    public void LateUpdate()
    {
        if (mainCamera != null && planetCamera != null)
        {
            planetCamera.transform.rotation = mainCamera.transform.rotation;
            Vector3 sPos = Vector3.zero;
            Vector3 zPos = Vector3.zero;
            Vector3 cPos = mainCamera.transform.position / SolarObject.scaleFactor;
            CameraManager.planetCamera.transform.localPosition = sPos + zPos + cPos;
        }
    }
}
