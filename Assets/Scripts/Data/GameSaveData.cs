using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class GameSaveData : IData
    {
        public string gamestartTemplateName;
        public bool is_gamestart_started;
        public int galaxyId;
        public int systemId;

        public int[] spaceContainerPosition = {0,0,0};

        public int[] PosFixerSectorIndexes = {0,0,0};
        public int[] PosFixerZoneIndexes = {0,0,0};
        public int[] PosFixerCurrentZoneIndexes = {0,0,0};

        public List<SpaceObjectData> spaceObjectDatas = new List<SpaceObjectData>();
    }
}