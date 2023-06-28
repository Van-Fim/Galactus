using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public class ConfigData : IData
    {
        public class XMLAttribute
        {
            public string name;
            public string value;
        }
        public string login = "Login";
        public string language = "EN";
        public List<XMLAttribute> atrParams = new List<XMLAttribute>();
        public List<string> commands = new List<string>();
        public string GetParamValue(string name)
        {
            XMLAttribute atr = atrParams.Find(f=>f.name == name);
            string ret = null;
            if (atr != null)
            {
                ret = atr.value;
            }
            return ret;
        }
    }
}
