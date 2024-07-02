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
    public bool isActive;
    public int id;
    public int depth;
    public GameObject GMObject;
    public HudData parent;
    public string parentHudName;
    public List<HudData> childList = new List<HudData>();
    public List<ParamData> paramsData = new List<ParamData>();
    public Color32 bgColor = new Color32(0, 0, 0, 255);
    public bool isHideInds;
    public bool freezeTime;
}
