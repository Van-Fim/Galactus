using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionFixer : MonoBehaviour
{
    public static PositionFixer singleton;
    public static Vector3 sectorIndexes = Vector3.zero;
    public static Vector3 zoneIndexes = Vector3.zero;
    public static Vector3 currentSectorIndexes = Vector3.zero;
    public static Vector3 currentZoneIndexes = Vector3.zero;
    public static int stepSize = 50000;
    public static int sectorStepSize = 500000;
    public static UnityAction OnFixZonePositionAction;
    public static UnityAction OnFixSectorPositionAction;
    public static bool isInitialized = false;
    public static bool isStoppedAutoUpdate = false;
    public static void OnFixZonePosition()
    {
        zoneIndexes = currentZoneIndexes;
        SolarController.zoneIndexes = SolarController.currentZoneIndexes;
        SpaceManager.spaceContainer.transform.localPosition = -(zoneIndexes * stepSize);
        LocalClient.ControlledObject.SetZoneIndexes(zoneIndexes);
        LocalClient.ControlledObject.transform.localPosition = -(PositionFixer.RecalcPos(LocalClient.ControlledObject.transform.localPosition, stepSize) - LocalClient.ControlledObject.transform.localPosition);
        NetClientManager.singleton.SendFixedIndexes(LocalClient.ControlledObject.GetComponent<NetSpaceObject>().netId, zoneIndexes);
    }
    public static void OnFixSectorPosition()
    {
        sectorIndexes = currentSectorIndexes;
        LocalClient.ControlledObject.SetSectorIndexes(sectorIndexes);
    }
    public static void Init()
    {
        singleton = GameManager.singleton.gameObject.AddComponent<PositionFixer>();
        sectorIndexes = currentSectorIndexes = LocalClient.ControlledObject.GetSectorIndexes();
        Vector3 plyPos = (sectorIndexes * PositionFixer.sectorStepSize + LocalClient.ControlledObject.transform.localPosition);
        currentZoneIndexes = PositionFixer.RecalcPos(plyPos, stepSize);
        currentZoneIndexes = new Vector3((int)(currentZoneIndexes.x / stepSize), (int)(currentZoneIndexes.y / stepSize), (int)(currentZoneIndexes.z / stepSize));
        SolarController.currentZoneIndexes = Vector3.zero;
        OnFixZonePositionAction += OnFixZonePosition;
        OnFixSectorPositionAction += OnFixSectorPosition;
        CameraManager.planetCamera.transform.SetParent(null);
        OnFixZonePositionAction?.Invoke();
        OnFixSectorPositionAction?.Invoke();
        GameManager.singleton.testCube.transform.SetParent(SpaceManager.spaceContainer.transform);
        isInitialized = true;
    }
    public void Update()
    {
        UpdateFixedPos();
    }
    public void UpdateFixedPos()
    {
        if (LocalClient.ControlledObject != null && isInitialized)
        {
            currentZoneIndexes = PositionFixer.RecalcPos(LocalClient.ControlledObject.transform.localPosition + zoneIndexes * stepSize, stepSize);
            SolarController.currentZoneIndexes = PositionFixer.RecalcPos(LocalClient.ControlledObject.transform.localPosition + SolarController.zoneIndexes * SolarController.stepSize, SolarController.stepSize);
            if (!isStoppedAutoUpdate)
            {
                currentZoneIndexes = new Vector3((int)(currentZoneIndexes.x / stepSize), (int)(currentZoneIndexes.y / stepSize), (int)(currentZoneIndexes.z / stepSize));
                SolarController.currentZoneIndexes = new Vector3((int)(SolarController.currentZoneIndexes.x / SolarController.stepSize), (int)(SolarController.currentZoneIndexes.y / SolarController.stepSize), (int)(SolarController.currentZoneIndexes.z / SolarController.stepSize));
                if (zoneIndexes != currentZoneIndexes)
                {
                    OnFixZonePositionAction?.Invoke();
                }
                if (sectorIndexes != currentSectorIndexes)
                {
                    OnFixSectorPositionAction?.Invoke();
                }
            }
            else
            {
                //Debug.Log($"{PositionFixer.sectorIndexes} {PositionFixer.zoneIndexes} {PositionFixer.currentZoneIndexes} {LocalClient.controlledObject.transform.localPosition} {LocalClient.controlledObject.transform.localPosition + zoneIndexes * stepSize}");
            }
        }
    }
    public static Vector3 RecalcPos(Vector3 position, int stepSize, bool recl = false)
    {
        Vector3 ret = new Vector3();
        Vector3 c1 = position;
        Vector3 stepVals = new Vector3(stepSize, stepSize, stepSize);
        position = new Vector3(position.x + stepVals.x / 2, position.y + stepVals.y / 2, position.z + stepVals.z / 2);
        ret = new Vector3((int)(position.x / stepSize), (int)(position.y / stepSize), (int)(position.z / stepSize));

        if (c1.x + stepSize / 2 < 0)
        {
            ret.x -= 1;
        }
        if (c1.y + stepSize / 2 < 0)
        {
            ret.y -= 1;
        }
        if (c1.z + stepSize / 2 < 0)
        {
            ret.z -= 1;
        }
        ret = ret * stepSize;
        return ret;
    }
}
