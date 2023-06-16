using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public class ServerData : IData
    {
        public List<AccountData> accounts = new List<AccountData>();
        public List<CharacterData> characters = new List<CharacterData>();

        public AccountData GetAccountById(int id)
        {
            AccountData ret = accounts.Find(f => f.id == id);
            DebugConsole.ShowErrorIsNull(ret, $"Account {id} not found!");
            return ret;
        }
    }
}
