using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public static string seed = "myseed";
    public List<SpaceObjectData> spaceObjectDatas = new List<SpaceObjectData>();

    public static int GetSeed(int galaxyId = -1, int systemId = -1, int sectorId = -1)
    {
        int ret = 0;
        if (galaxyId >= 0 && systemId >= 0 && sectorId >= 0)
        {
            ret = $"{seed}{galaxyId}{systemId}{sectorId}".GetHashCode();
        }
        else if (galaxyId >= 0 && systemId >= 0 && sectorId < 0)
        {
            ret = $"{seed}{galaxyId}{systemId}".GetHashCode();
        }
        else if (galaxyId >= 0 && systemId < 0 && sectorId < 0)
        {
            ret = $"{seed}{galaxyId}".GetHashCode();
        }
        else if (galaxyId < 0 && systemId < 0 && sectorId < 0)
        {
            ret = $"{seed}".GetHashCode();
        }
        return ret;
    }
    public void Start()
    {
        Application.targetFrameRate = 60;
        singleton = this;
        GamePrefabsManager.Init();
        CanvasController canvasController = GamePrefabsManager.LoadPrefab<CanvasController>("Canvas");
        canvasController = Instantiate(canvasController);
        CameraManager.Init();
        CameraManager.SwitchCamera(CameraManager.mainCamera);
        MenuManager.singleton.gameObject.SetActive(false);
        Material mat = Resources.Load<Material>($"Materials/Skybox/Skybox01");
        RenderSettings.skybox = mat;
        return;
    }
}
