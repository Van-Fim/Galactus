using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HudData : IData
{
    public string item;
    public string name;
    public string swichGroup;
    public string type;
    public string group;
    public bool isActive;
    public int id;
    public int depth;
    public GameObject GMObject;
    public HudData parent;
    public string parentHudName;
    public List<HudData> childList = new List<HudData>();
    public List<ParamData> paramsData = new List<ParamData>();
    public Color32 bgColor1 = new Color32(0, 0, 0, 0);
    public Color32 bgColor2 = new Color32(0, 0, 0, 0);
    public bool bgColor2isActive;
    public bool customPos;
    public Vector3 position;
    public bool isHideInds;
    public bool freezeTime;
}
