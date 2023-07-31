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
        public string login;
        public string password;
        public string gameStart;
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
