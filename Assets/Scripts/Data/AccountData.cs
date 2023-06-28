using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class AccountData : ResourcesManager, IData
    {
        public int id;
        public string login;
        public string password;
    }
}
