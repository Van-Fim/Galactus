using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class SpaceObjectData : IData
    {
        public int id;
        public string templateName;
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
