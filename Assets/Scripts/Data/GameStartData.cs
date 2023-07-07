using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
[System.Serializable]
public class GameStartData : IData
{
    public List<ParamData> paramDatas = new List<ParamData>();
    public List<ParamData> resourceDatas = new List<ParamData>();
    public string name;
    public string templateName;
    public string startType;
    public float[] position = { 0, 0, 0 };
    public float[] rotation = { 0, 0, 0 };
    public string GetParam(string name)
    {
        string ret = null;
        ParamData pd = paramDatas.Find(f=>f.name == name);
        //DebugConsole.ShowErrorIsNull(pd, $"Error! param {name} not found");
        if (pd != null)
        {
            ret = pd.value;
        }
        return ret;
    }
    public void SetPosition(Vector3 position)
    {
        this.position = new float[] { position.x, position.y, position.z };
    }
    public Vector3 GetPosition()
    {
        return new Vector3(this.position[0], this.position[1], this.position[2]);
    }
    public void SetRotation(Vector3 rotation)
    {
        this.rotation = new float[] { rotation.x, rotation.y, rotation.z };
    }
    public Vector3 GetRotation()
    {
        return new Vector3(this.rotation[0], this.rotation[1], this.rotation[2]);
    }
}
