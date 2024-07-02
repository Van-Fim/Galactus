using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IND_targetManager : MonoBehaviour
{
    public static IND_targetManager singleton;
    public static List<IND_target> list = new List<IND_target>();
    public static void Init()
    {
        singleton = GameManager.singleton.gameObject.AddComponent<IND_targetManager>();
    }
    public static void Remove(IND_target iND_Target)
    {
        if (!iND_Target)
        {
            return;
        }
        if (list.Contains(iND_Target))
        {
            list.Remove(iND_Target);
            DestroyImmediate(iND_Target);
        }
    }
    public static IND_target Create(SpaceObject spaceObject)
    {
        IND_target ret = GamePrefabsManager.LoadPrefab<IND_target>("IND_target");
        ret = Instantiate(ret, CanvasController.singleton.transform);
        ret.spaceObject = spaceObject;
        list.Add(ret);
        return ret;
    }

    public void LateUpdate()
    {
        for (int i = 0; i < list.Count; i++)
        {
            IND_target iND_Target = list[i];
            Vector3 screenPosition = CameraManager.mainCamera.curCamera.WorldToScreenPoint(iND_Target.spaceObject.transform.position);
            iND_Target.transform.position = new Vector3(screenPosition.x, screenPosition.y, 0);
        }
    }
}
