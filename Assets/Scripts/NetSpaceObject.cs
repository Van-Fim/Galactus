using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Collections;
using UnityEngine;

public class NetSpaceObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangedHook))]
    public SpaceObjectData data;
    [SyncVar]
    public bool isPlayer;
    public bool isGameStartDataReceived;
    public event System.Action OnObjectStartedAction;
    void OnChangedHook(SpaceObjectData _old, SpaceObjectData _new)
    {

    }
    public void OnObjectStarted()
    {
        if (isLocalPlayer)
        {
            LocalClient.netSpaceObject = this;
            GameStartData gameStartData = GameStartManager.LoadGameStart("Start01");
            gameStartData.spaceObjectDatas = SpaceObjectManager.ReadSpaceContent("Start01", "start");
            SpaceObjectData plyData = gameStartData.spaceObjectDatas.Find(x => x.isPlayerControll == true);
            if (plyData != null)
            {
                gameStartData.type = plyData.type;
                SendStartData(gameStartData);
            }
        }
        else
        {
            if (!isServer)
            {
                SpaceObject obj = SpaceObject.Create(data, gameObject);
                obj.Init();
                obj.LoadHardpoints();
                SpaceObject.InvokeRender();
            }
        }
    }
    public override void OnStartClient()
    {
        LocalClient.isServer = isServer;
        OnObjectStartedAction = OnObjectStarted;
        OnObjectStartedAction.Invoke();
    }
    [Command]
    public void SendStartData(GameStartData data)
    {
        for (int i = 0; i < data.spaceObjectDatas.Count; i++)
        {
            data.spaceObjectDatas[i].id = SpaceObject.GetId();
            if (data.spaceObjectDatas[i].isPlayerControll)
            {
                this.data = data.spaceObjectDatas[i];
                ObjectsDataFixed(this.data);
            }
        }
        GameStartDataReceived(data);
    }
    [Command]
    public void FixObjectsData()
    {
        ObjectsDataFixed(this.data);
    }
    [ClientRpc]
    public void ObjectsDataFixed(SpaceObjectData data)
    {
        this.data = data;
    }
    [ClientRpc]
    public void GameStartDataReceived(GameStartData data)
    {
        if (!isGameStartDataReceived && isLocalPlayer)
        {
            LocalClient.netSpaceObject = this;
            if (!isServer)
            {
                SpaceObjectManager.Init();
                IND_targetManager.Init();
                SpaceManager.BuildGalaxies();
                SpaceManager.BuildSystems();
            }
            SpaceManager.LoadSystem(LocalClient.SpaceSystem);
        }
        SpaceObjectManager.BuildObjectsByData(data.spaceObjectDatas, gameObject);
        SpaceObject.InvokeRender();
        if (!isGameStartDataReceived && isLocalPlayer)
        {
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

                SOShipController controller = cobj.gameObject.AddComponent<SOShipController>();
                controller.obj = cobj;
                cobj.transform.SetParent(null);
            }
            Space.InvokeMinimapRender();
            PositionFixer.Init();
            CameraManager.SwitchCamera(CameraManager.mainCamera);
            Hud hud = MenuManager.GetHud("MainHudMenu");
            hud.ShowSingle();
        }
        isGameStartDataReceived = true;
    }
}
