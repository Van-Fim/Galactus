using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionFixer : MonoBehaviour
{
    public static PositionFixer singleton;
    public static Vector3 sectorIndexes = Vector3.zero;
    public static Vector3 zoneIndexes = Vector3.zero;
    public static Vector3 currentZoneIndexes = Vector3.zero;
    public static int stepSize = 5000;
    public static UnityAction OnFixZonePositionAction;
    public static bool isStoppedAutoUpdate = false;
    public static void OnFixZonePosition()
    {
        zoneIndexes = currentZoneIndexes;
        SpaceManager.spaceContainer.transform.localPosition = -(zoneIndexes * stepSize);
        LocalClient.controlledObject.transform.localPosition = -(PositionFixer.RecalcPos(LocalClient.controlledObject.transform.localPosition, stepSize) - LocalClient.controlledObject.transform.localPosition);
        SpaceObject.InvokeFixZonePosition();
    }
    public static void Init()
    {
        singleton = GameManager.singleton.gameObject.AddComponent<PositionFixer>();
        OnFixZonePositionAction += OnFixZonePosition;
    }
    public void Update()
    {
        if (LocalClient.controlledObject)
        {
            
            currentZoneIndexes = PositionFixer.RecalcPos(LocalClient.controlledObject.transform.localPosition + zoneIndexes * stepSize, stepSize);
            if (!isStoppedAutoUpdate)
            {
                currentZoneIndexes = new Vector3((int)(currentZoneIndexes.x / stepSize), (int)(currentZoneIndexes.y / stepSize), (int)(currentZoneIndexes.z / stepSize));
                if (zoneIndexes != currentZoneIndexes)
                {
                    OnFixZonePositionAction?.Invoke();
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
