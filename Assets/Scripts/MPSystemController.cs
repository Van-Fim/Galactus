using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPSystemController : MonoBehaviour
{
    public LineRenderer eLineRenderer;
    public Space space;
    public SpaceUiObj spaceUiObj;
    public GameObject obj;

    public void Init(){
        spaceUiObj = GamePrefabsManager.LoadPrefab<SpaceUiObj>("SpaceUiObj");
        spaceUiObj = GameObject.Instantiate(spaceUiObj, CanvasController.singleton.transform);
        spaceUiObj.space = space;
        spaceUiObj.Init();

        obj.GetComponent<MeshRenderer>().material.SetColor("_Color", space.GetBgColor());
    }
}
