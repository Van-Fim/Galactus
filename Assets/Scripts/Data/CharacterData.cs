using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class CharacterData : ResourcesManager, IData
    {
        public int id;
        public int accountId;
        public int galaxyId;
        public int systemId;
        public int sectorId;
        public int zoneId;
        public float[] position = new float[] { 0, 0, 0 };
        public float[] rotation = new float[] { 0, 0, 0 };
        public int[] sectorIndexes = { 0, 0, 0 };
        public int[] zoneIndexes = { 0, 0, 0 };
        public string login;
        public string password;
        public string gameStart;
        public bool isGameStartDataLoaded;
        public int controlledObjectId;
        public void SetZoneIndexes(Vector3 indexes)
        {
            zoneIndexes = new int[] { (int)indexes.x, (int)indexes.y, (int)indexes.z };
        }
        public Vector3 GetZoneIndexes()
        {
            return new Vector3((int)this.zoneIndexes[0], (int)this.zoneIndexes[1], (int)this.zoneIndexes[2]);
        }
        public void SetSectorIndexes(Vector3 indexes)
        {
            sectorIndexes = new int[] { (int)indexes.x, (int)indexes.y, (int)indexes.z };
        }
        public Vector3 GetSectorIndexes()
        {
            return new Vector3((int)this.sectorIndexes[0], (int)this.sectorIndexes[1], (int)this.sectorIndexes[2]);
        }
        public virtual void SetPosition(Vector3 position)
        {
            this.position = new float[] { position.x, position.y, position.z };
        }
        public virtual Vector3 GetPosition()
        {
            return new Vector3(this.position[0], this.position[1], this.position[2]);
        }
        public virtual void SetRotation(Vector3 rotation)
        {
            this.rotation = new float[] { rotation.x, rotation.y, rotation.z };
        }
        public virtual Vector3 GetRotation()
        {
            return new Vector3(this.rotation[0], this.rotation[1], this.rotation[2]);
        }
    }
}
