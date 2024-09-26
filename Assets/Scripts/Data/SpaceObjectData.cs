using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceObjectData : IData
{
    public uint id;
    public uint spaceObjectId;
    public uint netId;
    public string templateName;
    public string characterLogin;
    public bool isPlayerControll;
    public bool isInitialized;
    public string loadoutsTemplateName;
    public string hardpointsTemplateName;
    public bool isStartObject;
    public bool sendSuccessfull;
    public int galaxyId;
    public int systemId;
    public int sectorId;
    public int targetGalaxyId;
    public int targetSystemId;
    public int targetGateId;
    public string type;
    public int mass;
    public float drag;
    public float angulardrag;
    public string modelPatch;

    public float scaleFactor = 1f;
    public float[] position = { 0, 0, 0 };
    public float[] rotation = { 0, 0, 0 };
    public int[] sectorIndexes = { 0, 0, 0 };
    public int[] zoneIndexes = { 0, 0, 0 };
    public virtual void SetSpace(Space space)
    {
        if (space == null)
        {
            Debug.LogError($"Error! target space is null!");
            return;
        }
        if (space.GetType() == typeof(Galaxy))
        {
            galaxyId = space.id;
        }
        else if (space.GetType() == typeof(SpaceSystem))
        {
            SpaceSystem sp = (SpaceSystem)space;
            galaxyId = sp.galaxyId;
            systemId = sp.id;
        }
    }
    public SpaceObject CreateByType()
    {
        Transform tr = GamePrefabsManager.LoadPrefab<Transform>("SpaceObjectPrefab");
        tr = GameObject.Instantiate(tr);
        SpaceObject ret = null;
        if (type == null || type == "spaceobject" || type == "gate")
        {
            ret = tr.gameObject.AddComponent<SpaceObject>();
        }
        if (type == "ship")
        {
            ret = tr.gameObject.AddComponent<Ship>();
        }
        else if (type == "pilot")
        {
            ret = tr.gameObject.AddComponent<Pilot>();
        }
        ret.id = SpaceObject.GetId();
        return ret;
    }
    public SpaceObject AddByType(GameObject gameObject)
    {
        SpaceObject ret = null;
        if (type == null || type == "spaceobject" || type == "gate")
        {
            ret = gameObject.AddComponent<SpaceObject>();
        }
        if (type == "ship")
        {
            ret = gameObject.AddComponent<Ship>();
        }
        else if (type == "pilot")
        {
            ret = gameObject.AddComponent<Pilot>();
        }
        ret.id = SpaceObject.GetId();
        return ret;
    }
    public int GetGalaxyId()
    {
        return galaxyId;
    }
    public int GetSystemId()
    {
        return systemId;
    }
    public Vector3 GetZoneIndexes()
    {
        return new Vector3((int)this.zoneIndexes[0], (int)this.zoneIndexes[1], (int)this.zoneIndexes[2]);
    }
    public Vector3 GetSectorIndexes()
    {
        return new Vector3((int)this.sectorIndexes[0], (int)this.sectorIndexes[1], (int)this.sectorIndexes[2]);
    }
    public void SetZoneIndexes(Vector3 value)
    {
        zoneIndexes = new int[] { (int)value.x, (int)value.y, (int)value.z };
    }
    public void SetSectorIndexes(Vector3 value)
    {
        sectorIndexes = new int[] { (int)value.x, (int)value.y, (int)value.z };
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
