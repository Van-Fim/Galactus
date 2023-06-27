using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace Data
{
    [System.Serializable]
    public class ServerData : IData
    {
        public List<AccountData> accounts = new List<AccountData>();
        public List<CharacterData> characters = new List<CharacterData>();

        public AccountData GetAccountById(int id)
        {
            AccountData ret = accounts.Find(f => f.id == id);
            return ret;
        }
        public AccountData GetAccountByLogin(string login)
        {
            AccountData ret = accounts.Find(f => f.login == login);
            return ret;
        }
        public CharacterData GetCharacterById(int id)
        {
            CharacterData ret = characters.Find(f => f.id == id);
            return ret;
        }
        public CharacterData GetCharacterByLogin(string login)
        {
            CharacterData ret = characters.Find(f => f.login == login);
            return ret;
        }
        public bool CheckLogin(string login, bool accountCheck = false)
        {
            bool ret = false;
            if (accountCheck)
            {
                AccountData acs = accounts.Find(f => f.login == login);
                if (acs != null)
                {
                    ret = true;
                }
            }
            else
            {
                CharacterData chr = characters.Find(f => f.login == login);
                if (chr != null)
                {
                    ret = true;
                }
            }
            return ret;
        }
        public AccountData CreateAccountData(string login, string password)
        {
            AccountData accountData = accounts.Find(f => f.login == login);
            if (accountData == null)
            {
                int curId = 0;
                AccountData fnd = accounts.Find(f => f.id == curId);

                while (fnd != null)
                {
                    curId++;
                    fnd = accounts.Find(f => f.id == curId);
                }

                accountData = new AccountData();
                accountData.id = curId;
                accountData.login = login;
                accountData.password = XMLF.StrToMD5(password);
                accounts.Add(accountData);
                return accountData;
            }
            return null;
        }
        public CharacterData CreateCharacterData(string login, string password, int accountId)
        {
            CharacterData characterData = characters.Find(f => f.login == login);
            if (characterData == null)
            {
                int curId = 0;
                CharacterData fnd = characters.Find(f => f.id == curId);

                while (fnd != null)
                {
                    curId++;
                    fnd = characters.Find(f => f.id == curId);
                }

                characterData = new CharacterData();
                characterData.id = curId;
                characterData.accountId = accountId;
                characterData.login = login;
                characterData.password = XMLF.StrToMD5(password);
                characters.Add(characterData);
                return characterData;
            }
            return null;
        }
        public bool SaveServerData()
        {
            bool ret = false;
            string dir = XMLF.GetDirPatch();
            string fileName = "server.data";
            FileStream file = null;
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Create(dir + "/" + fileName);
            ServerData sv = new ServerData();
            bf.Serialize(file, this);
            ret = true;
            return ret;
        }
        public void LoadServerData()
        {
            string dir = XMLF.GetDirPatch();
            string fileName = "server.data";
            FileStream file = null;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(dir + "/" + fileName))
            {
                file = File.Open(dir + "/" + fileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                ServerData serverData = (ServerData)bf.Deserialize(file);
                this.accounts = serverData.accounts;
                this.characters = serverData.characters;
            }
        }
    }
}
