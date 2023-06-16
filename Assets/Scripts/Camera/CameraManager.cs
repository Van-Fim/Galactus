using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraController skyboxCamera;
    public static CameraController mainCamera;
    public static CameraManager singleton;
    public static void Init()
    {
        GamePrefabsManager gmpr = GamePrefabsManager.singleton;
        CameraManager.mainCamera = GameObject.Instantiate(gmpr.LoadPrefab<CameraController>("MainCameraPrefab"));

        CameraManager.skyboxCamera = GameObject.Instantiate(gmpr.LoadPrefab<CameraController>("SkyboxCameraPrefab"));

        singleton = Instantiate(GamePrefabsManager.singleton.LoadPrefab<CameraManager>("CameraManagerPrefab"));
        singleton.gameObject.name = "CameraManager";
        DontDestroyOnLoad(singleton.gameObject);
        DontDestroyOnLoad(CameraManager.mainCamera.gameObject);
        DontDestroyOnLoad(CameraManager.skyboxCamera.gameObject);
    }
}
