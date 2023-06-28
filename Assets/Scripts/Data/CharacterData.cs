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
        public string login;
        public string password;
    }
}
