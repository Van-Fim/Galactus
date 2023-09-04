using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public class WarpData : IData
    {
        public int galaxyId;
        public int systemId;
        public int sectorId;
        public int zoneId;
        public Vector3 position;
        public Vector3 rotation;
        public int[] sectorIndexes = { 0, 0, 0 };
        public int[] zoneIndexes = { 0, 0, 0 };
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
    }
}