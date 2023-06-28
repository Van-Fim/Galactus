using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class ResourcesData : IData
    {
        public int id;
        public string templateName;
        public string name;
        public string subType;
        public float value;
    }
}
