using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    public GameObject testCube;
    public static GameManager singleton;
    public static string seed = "myseed";

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
        SpaceManager.Init();
        GamePrefabsManager.Init();
        CanvasController canvasController = GamePrefabsManager.LoadPrefab<CanvasController>("Canvas");
        canvasController = Instantiate(canvasController);
        CameraManager.Init();
        CameraManager.SwitchCamera(CameraManager.mainCamera);
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
        SpaceObjectManager.Init();
        IND_targetManager.Init();
        SpaceManager.BuildGalaxies();
        SpaceManager.BuildSystems();
        SpaceManager.BuildSystemsContent();
    }
    public void StartGame()
    {
        CameraManager.SwitchCamera(CameraManager.mainCamera);
        SpaceManager.LoadSystem(LocalClient.SpaceSystem);

        if (LocalClient.ControlledObject != null)
        {
            SpaceObject cobj = LocalClient.ControlledObject;
            LocalClient.galaxyId = cobj.galaxyId;
            LocalClient.systemId = cobj.systemId;
            LocalClient.sectorId = cobj.sectorId;
            LocalClient.ControlledObject.WarpSystem(LocalClient.SpaceSystem, LocalClient.sectorId);
            SpaceObject.InvokeRender();
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
        PositionFixer.Init();
        Hud hud = MenuManager.GetHud("MainHudMenu");
        hud.ShowSingle();
    }
}
