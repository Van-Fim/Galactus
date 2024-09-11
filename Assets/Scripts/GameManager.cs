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
    public void Awake()
    {
        Application.targetFrameRate = 60;
        singleton = this;
        GamePrefabsManager.Init();
        CanvasController canvasController = GamePrefabsManager.LoadPrefab<CanvasController>("Canvas");
        canvasController = Instantiate(canvasController);
        CameraManager.Init();
        CameraManager.SwitchCamera(CameraManager.mainCamera);
<<<<<<< HEAD
        SpaceManager.Init();
        SpaceManager.SetRandomSkybox();

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canvasController.gameObject);
        DontDestroyOnLoad(GamePrefabsManager.singleton.gameObject);
        DontDestroyOnLoad(CameraManager.mainCamera.gameObject);
        DontDestroyOnLoad(CameraManager.mapCamera.gameObject);
        DontDestroyOnLoad(CameraManager.skyBoxCamera.gameObject);
        DontDestroyOnLoad(SpaceManager.singleton.gameObject);

        MenuManager.singleton.BuildAll();
        for (int i = 0; i < MenuManager.huds.Count; i++)
        {
            Hud h = MenuManager.huds[i];
            h.Hide();
        }
        MultiplayerPanel.Init();
        // PositionFixer.Init();
        // SpaceObjectManager.Init();
    }
    public void LoadContent()
    {
        LocalClient.isServer = true;
        SpaceObjectManager.Init();
        IND_targetManager.Init();
        SpaceManager.BuildGalaxies();
        SpaceManager.BuildSystems();
        SpaceManager.BuildSystemsContent();
    }
    public void StartGame(GameStartData gameStartData)
    {
        CameraManager.SwitchCamera(CameraManager.mainCamera);
        gameStartData.spaceObjectDatas = SpaceObjectManager.ReadSpaceContent(gameStartData.templateName, "start");
        SpaceObjectManager.BuildObjectsByData(gameStartData.spaceObjectDatas, LocalClient.netSpaceObject.gameObject);
        SpaceManager.LoadSystem(LocalClient.SpaceSystem);
        SpaceObject.InvokeRender();

        LocalClient.ControlledObject.WarpSystem(LocalClient.SpaceSystem, LocalClient.Sector.id);
        if (LocalClient.ControlledObject != null)
        {
            SpaceObject cobj = LocalClient.ControlledObject;
            LocalClient.ControlledObject.isInitialized = true;
            LocalClient.ControlledObject.isPlayerControll = true;

            Hardpoint camHP = cobj.GetHardpointByType("camera");
            CameraManager.mainCamera.IsCamEnabled = false;
            CameraManager.mainCamera.transform.SetParent(cobj.main.transform);
            CameraManager.mainCamera.transform.localPosition = camHP.GetPosition();
            CameraManager.mainCamera.transform.localEulerAngles = camHP.GetRotation();

            SOShipController controller = cobj.AddComponent<SOShipController>();
            controller.obj = cobj;
            cobj.transform.SetParent(null);
        }
        Space.InvokeMinimapRender();
        Hud hud = MenuManager.GetHud("MainHudMenu");
        hud.ShowSingle();
=======
        MenuManager.singleton.gameObject.SetActive(false);
        Material mat = Resources.Load<Material>($"Materials/Skybox/Skybox01");
        RenderSettings.skybox = mat;
        return;
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    }
}
