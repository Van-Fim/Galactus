using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HudSwichGroupData : IData
{
    public string name;
    public List<HudData> list = new List<HudData>();
    public HudSwichGroupData() { }
}
