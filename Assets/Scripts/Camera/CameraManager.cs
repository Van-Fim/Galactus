using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraController skyboxCamera;
    public static CameraController mainCamera;
    public static CameraController minimapCamera;
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
        CameraManager.skyboxCamera = GameObject.Instantiate(gmpr.LoadPrefab<CameraController>("SkyboxCameraPrefab"));
        CameraManager.minimapCamera = GameObject.Instantiate(gmpr.LoadPrefab<MinimapCameraController>());
        singleton = Instantiate(GamePrefabsManager.singleton.LoadPrefab<CameraManager>("CameraManagerPrefab"));
        singleton.gameObject.name = "CameraManager";
        DontDestroyOnLoad(singleton.gameObject);
        DontDestroyOnLoad(CameraManager.mainCamera.gameObject);
        DontDestroyOnLoad(CameraManager.skyboxCamera.gameObject);
        DontDestroyOnLoad(CameraManager.minimapCamera.gameObject);
    }
}
