using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class ParamData : IData
    {
        public string name;
        public string value;
        public ParamData(){}
        public ParamData(string name, string value){
            this.name = name;
            this.value = value;
        }
    }
}
