using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MainHud : MonoBehaviour
{
    public TMPro.TMP_Text speed;
    public TMPro.TMP_Text distance;
    public TMPro.TMP_Text objectName;
    public bool isShow = false;
    public void Update()
    {
        if (IND_target.selectedTarget && IND_target.selectedTarget.spaceObject && LocalClient.controlledObject)
        {
            float dst = Vector3.Distance(IND_target.selectedTarget.spaceObject.transform.position, LocalClient.controlledObject.transform.position);
            dst /= 1000f;
            distance.text = $"{dst.ToString("0.00")} km";
            objectName.text = IND_target.selectedTarget.spaceObject.objectName;
        }
        else
        {
            distance.text = null;
            objectName.text = null;
        }
    }
    public void showMap()
    {
        TMPro.TMP_Text speed = GameManager.canvasController.mainHud.speed;
        isShow = !isShow;
        if (isShow)
        {
            Vector3 pos = LocalClient.spaceSystem.GetPosition();
            pos = new Vector3(pos.x, pos.y + 100, pos.z);
            CameraManager.mapCamera.transform.localPosition = pos;
            CameraManager.mapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
            CameraManager.SwitchCamera(CameraManager.mapCamera);
            Space.InvokeMinimapRender();

            speed.gameObject.SetActive(false);
        }
        else
        {
            CameraManager.SwitchCamera(CameraManager.mainCamera);

            speed.gameObject.SetActive(true);
        }
    }
    public void warp()
    {
        if (SpaceUiObj.selectedObj == null)
            return;
        LocalClient.controlledObject.Warp((SpaceSystem)SpaceUiObj.selectedObj.space, Vector3.zero, Vector3.zero);
    }
    public void save()
    {
        Data.GameSaveData gameSaveData = new Data.GameSaveData();

        gameSaveData.gamestartTemplateName = LocalClient.gamestartTemplateName;
        gameSaveData.is_gamestart_started = LocalClient.is_gamestart_started;
        gameSaveData.galaxyId = LocalClient.galaxyId;
        gameSaveData.systemId = LocalClient.systemId;
        gameSaveData.PosFixerSectorIndexes = new int[] { (int)PositionFixer.sectorIndexes.x, (int)PositionFixer.sectorIndexes.y, (int)PositionFixer.sectorIndexes.z };
        gameSaveData.PosFixerZoneIndexes = new int[] { (int)PositionFixer.zoneIndexes.x, (int)PositionFixer.zoneIndexes.y, (int)PositionFixer.zoneIndexes.z };
        gameSaveData.PosFixerCurrentZoneIndexes = new int[] { (int)PositionFixer.currentZoneIndexes.x, (int)PositionFixer.currentZoneIndexes.y, (int)PositionFixer.currentZoneIndexes.z };
        gameSaveData.spaceContainerPosition = new int[] { (int)SpaceManager.spaceContainer.transform.localPosition.x, (int)SpaceManager.spaceContainer.transform.localPosition.y, (int)SpaceManager.spaceContainer.transform.localPosition.z };
        Debug.Log($"{PositionFixer.sectorIndexes} {PositionFixer.zoneIndexes} {PositionFixer.currentZoneIndexes} {LocalClient.controlledObject.transform.localPosition} {LocalClient.controlledObject.transform.localPosition + PositionFixer.zoneIndexes * PositionFixer.stepSize}");
        for (int i = 0; i < SpaceObjectManager.spaceObjects.Count; i++)
        {
            SpaceObject spaceObject = SpaceObjectManager.spaceObjects[i];
            Data.SpaceObjectData data = spaceObject.GetSpaceObjectData();
            gameSaveData.spaceObjectDatas.Add(data);
        }
        string dir = XMLF.GetDirPatch();
        string fileName = "game.data";
        FileStream file = null;
        BinaryFormatter bf = new BinaryFormatter();
        file = File.Create(dir + "/" + fileName);
        bf.Serialize(file, gameSaveData);
        file.Close();
    }
    public void load()
    {
        string fileName = "game.data";
        string dir = XMLF.GetDirPatch();
        FileStream file = null;
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        if (File.Exists(dir + "/" + fileName))
        {
            file = File.Open(dir + "/" + fileName, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            Data.GameSaveData saveData = (Data.GameSaveData)bf.Deserialize(file);
            CameraManager.mainCamera.transform.SetParent(null);
            for (int i = 0; i < IND_targetManager.list.Count; i++)
            {
                IND_targetManager.Remove(IND_targetManager.list[i]);
            }
            for (int i = 0; i < SpaceObjectManager.spaceObjects.Count; i++)
            {
                SpaceObject spaceObjectspaceObject = SpaceObjectManager.spaceObjects[i];
                GameObject dgm = spaceObjectspaceObject.gameObject;
                spaceObjectspaceObject.Destroy();
                GameObject.DestroyImmediate(dgm);
            }
            SpaceObjectManager.spaceObjects = new List<SpaceObject>();
            LocalClient.gamestartTemplateName = saveData.gamestartTemplateName;
            LocalClient.galaxyId = saveData.galaxyId;
            LocalClient.systemId = saveData.systemId;
            SpaceSystem spaceSystem = SpaceManager.singleton.spaceSystems.Find(f => f.id == LocalClient.systemId && f.galaxyId == LocalClient.galaxyId);
            LocalClient.spaceSystem = spaceSystem;

            LocalClient.controlledObject.WarpSystem(LocalClient.spaceSystem);
            PositionFixer.isStoppedAutoUpdate = true;
            PositionFixer.sectorIndexes = new Vector3(saveData.PosFixerSectorIndexes[0], saveData.PosFixerSectorIndexes[1], saveData.PosFixerSectorIndexes[2]);
            PositionFixer.zoneIndexes = new Vector3(saveData.PosFixerZoneIndexes[0], saveData.PosFixerZoneIndexes[1], saveData.PosFixerZoneIndexes[2]);
            PositionFixer.currentZoneIndexes = new Vector3(saveData.PosFixerCurrentZoneIndexes[0], saveData.PosFixerCurrentZoneIndexes[1], saveData.PosFixerCurrentZoneIndexes[2]);
            Vector3 spp = new Vector3(saveData.spaceContainerPosition[0], saveData.spaceContainerPosition[1], saveData.spaceContainerPosition[2]);
            SpaceManager.spaceContainer.transform.localPosition = spp;
            Vector3 normPos = Vector3.zero;
            for (int i = 0; i < saveData.spaceObjectDatas.Count; i++)
            {
                Data.SpaceObjectData data = saveData.spaceObjectDatas[i];
                if (data.type == "object")
                {
                    SpaceObject obj = SpaceObject.Create(SpaceManager.singleton, data);
                    obj.Init();
                    obj.LoadHardpoints();
                    if (data.isPlayerControll)
                    {
                        LocalClient.controlledObject = obj;
                    }
                }
                if (data.type == "ship")
                {
                    Ship ship = (Ship)SpaceObject.Create(SpaceManager.singleton, data);

                    ship.Init();
                    ship.LoadHardpoints();
                    normPos = data.GetPosition();
                    if (data.isPlayerControll)
                    {
                        LocalClient.controlledObject = ship;
                    }
                }
            }
            file.Close();

            SpaceObject.InvokeRender();

            SOShipController controller = LocalClient.controlledObject.gameObject.AddComponent<SOShipController>();
            controller.obj = LocalClient.controlledObject;

            SpaceManager.spaceContainer.transform.localPosition = spp;
            LocalClient.controlledObject.transform.SetParent(null);
            LocalClient.controlledObject.transform.localPosition = normPos;
            Hardpoint camHP = LocalClient.controlledObject.GetHardpointByType("camera");
            CameraManager.mainCamera.IsCamEnabled = false;
            CameraManager.mainCamera.transform.SetParent(LocalClient.controlledObject.main.transform);
            CameraManager.mainCamera.transform.localPosition = camHP.GetPosition();
            CameraManager.mainCamera.transform.localEulerAngles = camHP.GetRotation();

            PositionFixer.isStoppedAutoUpdate = false;
        }
    }
}
