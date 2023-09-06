using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class SpaceObjectData : IData
    {
        public uint id;
        public uint netId;
        public string templateName;
        public string characterLogin;
        public bool isPlayerControll;
        public bool isInitialized;
        public string loadoutsTemplateName;
        public string hardpointsTemplateName;
        public bool isStartObject;
        public int galaxyId;
        public int systemId;
        public int sectorId;
        public int zoneId;
        public string type;
        public float[] position = { 0, 0, 0 };
        public float[] rotation = { 0, 0, 0 };
        public int[] sectorIndexes = { 0, 0, 0 };
        public int[] zoneIndexes = { 0, 0, 0 };
        public List<Hardpoint> hardpoints;
        public virtual void SetSpace(GameContent.Space space)
        {
            if (space == null)
            {
                DebugConsole.Log($"Error! target space is null!", "error");
                return;
            }
            if (space.GetType() == typeof(Galaxy))
            {
                galaxyId = space.id;
            }
            else if (space.GetType() == typeof(StarSystem))
            {
                StarSystem sp = (StarSystem)space;
                galaxyId = sp.galaxyId;
                systemId = sp.id;
            }
            else if (space.GetType() == typeof(Sector))
            {
                Sector sp = (Sector)space;
                galaxyId = sp.galaxyId;
                systemId = sp.systemId;
                sectorId = sp.id;
            }
            else if (space.GetType() == typeof(Zone))
            {
                Zone sp = (Zone)space;
                galaxyId = sp.galaxyId;
                systemId = sp.systemId;
                sectorId = sp.sectorId;
                zoneId = sp.id;
            }
        }
        public SpaceObject CreateByType()
        {
            SpaceObject ret = null;
            if (type == null || type == "object")
            {
                ret = GameObject.Instantiate(GamePrefabsManager.singleton.LoadPrefab<SpaceObject>("SpaceObjectPrefab"));
            }
            if (type == "ship")
            {
                ret = GameObject.Instantiate(GamePrefabsManager.singleton.LoadPrefab<SpaceObject>("ShipPrefab"));
            }
            else if (type == "pilot")
            {
                ret = GameObject.Instantiate(GamePrefabsManager.singleton.LoadPrefab<SpaceObject>("PilotPrefab"));
            }
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
        public int GetSectorId()
        {
            return sectorId;
        }
        public int GetZoneId()
        {
            return zoneId;
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
}
