using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class GameSaveData : IData
    {
        public int id;
        public string name;
        public string date;
        public string gamestartTemplateName;
        public bool is_gamestart_started;
        public bool lastSave;
        public int galaxyId;
        public int systemId;

        public int[] spaceContainerPosition = { 0, 0, 0 };

        public int[] PosFixerSectorIndexes = { 0, 0, 0 };
        public int[] PosFixerZoneIndexes = { 0, 0, 0 };
        public int[] PosFixerCurrentZoneIndexes = { 0, 0, 0 };

        public List<SpaceObjectData> spaceObjectDatas = new List<SpaceObjectData>();
        
        // public List<Galaxy> galaxies = new List<Galaxy>();
        // public List<SpaceSystem> spaceSystems = new List<SpaceSystem>();
        // public List<Sector> sectors = new List<Sector>();
    }
}