using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
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
    public void Start()
    {
        Application.targetFrameRate = 60;
        singleton = this;
        GamePrefabsManager.Init();
        CanvasController canvasController = GamePrefabsManager.LoadPrefab<CanvasController>("Canvas");
        canvasController = Instantiate(canvasController);
        CameraManager.Init();
        return;
        GameStartData gameStartData = GameStartManager.LoadGameStart("Start01");

        CameraManager.SwitchCamera(CameraManager.mainCamera);
        SpaceManager.Init();
        SpaceObjectManager.Init();
        IND_targetManager.Init();

        SpaceManager.BuildGalaxies();
        SpaceManager.BuildSystems();
        SpaceManager.BuildSystemsContent();

        gameStartData.spaceObjectDatas = SpaceObjectManager.ReadSpaceContent("Start01", "start");
        SpaceObjectManager.BuildObjectsByData(gameStartData.spaceObjectDatas);
        SpaceManager.LoadSystem(LocalClient.SpaceSystem);
        SpaceObject.InvokeRender();

        LocalClient.controlledObject.WarpSystem(LocalClient.SpaceSystem, LocalClient.Sector.id);

        if (LocalClient.controlledObject != null)
        {
            SpaceObject cobj = LocalClient.controlledObject;
            LocalClient.controlledObject.isInitialized = true;
            LocalClient.controlledObject.isPlayerControll = true;

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
        MenuManager.singleton.BuildAll();
        for (int i = 0; i < MenuManager.huds.Count; i++)
        {
            Hud h = MenuManager.huds[i];
            h.Hide();
        }
        Hud hud = MenuManager.GetHud("MainHudMenu");
        hud.ShowSingle();
    }
}
