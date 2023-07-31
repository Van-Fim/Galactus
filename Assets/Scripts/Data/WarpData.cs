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
    }
}