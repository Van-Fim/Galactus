using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ServerDataManager : NetworkBehaviour
{
    public ServerData serverData;
    public static ServerDataManager singleton;
    public override void OnStartClient()
    {
        if (!isServer)
        {
            singleton = this;
        }
    }
    public static void Init()
    {
        ServerDataManager serverManager = GamePrefabsManager.singleton.LoadPrefab<ServerDataManager>();
        singleton = Instantiate(serverManager);
        singleton.gameObject.name = "ServerController";
        DontDestroyOnLoad(singleton.gameObject);
        singleton.serverData = new ServerData();
    }
    public void CreateAccount(uint netId, string login, string password)
    {
        AccountData adt = serverData.CreateAccountData(login, password);
        DebugConsole.ShowErrorIsNull(adt, $"Account {login} already exists");
        if (adt != null)
        {
            NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
            cl.AccountSuccess(adt, 2);
        }
    }
    public void CreateCharacter(uint netId, string login, string password, string gameStart, int accountId)
    {
        CharacterData cht = serverData.CreateCharacterData(login, password, gameStart, accountId);
        DebugConsole.ShowErrorIsNull(cht, $"Character {login} already exists");
        if (cht != null)
        {
            NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
            
            GameStartData gameStartData = serverData.gameStarts.Find(f => f.templateName == gameStart);
            for (int i = 0; i < gameStartData.resourceDatas.Count; i++)
            {
                ParamData pd = gameStartData.resourceDatas[i];
                cht.SetResourceValue(pd.name, "1", XMLF.FloatVal(pd.value));
            }
            cht.galaxyId = int.Parse(gameStartData.GetParam("galaxy"));
            cht.systemId = int.Parse(gameStartData.GetParam("system"));
            cht.sectorId = int.Parse(gameStartData.GetParam("sector"));
            cht.zoneId = int.Parse(gameStartData.GetParam("zone"));
            cht.SetPosition(gameStartData.GetPosition());
            cht.SetRotation(gameStartData.GetRotation());
            cl.CharacterSuccess(cht, 2);
        }
    }
    [Command(requiresAuthority = false)]
    public void WarpClient(CharacterData character, uint netId, WarpData warpData)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        character.galaxyId = warpData.galaxyId;
        character.systemId = warpData.systemId;
        character.sectorId = warpData.sectorId;
        character.zoneId = warpData.zoneId;
        character.SetPosition(warpData.position);
        character.SetRotation(warpData.rotation);
        cl.CompleteWarp(warpData);
    }
    [Command(requiresAuthority = false)]
    public void CheckAccount(uint netId, string login, string password)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        AccountData adt = serverData.GetAccountByLogin(login);
        string mdPass = XMLF.StrToMD5(password);

        if (adt != null)
        {
            if (adt.password != mdPass)
            {
                cl.AccountError(1);
                return;
            }
            cl.AccountSuccess(adt, 1);
        }
        else
        {
            CreateAccount(netId, login, password);
        }
        return;
    }
    [Command(requiresAuthority = false)]
    public void DeleteCharacter(uint netId, string login)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        CharacterData cht = serverData.characters.Find(f => f.login == login);
        if (cht != null)
        {
            int ind = serverData.characters.IndexOf(cht);
            serverData.characters.Remove(serverData.characters[ind]);
            cl.UpdateCharactersRpc(serverData);
        }
    }
    [Command(requiresAuthority = false)]
    public void CheckCharacter(uint netId, CharacterData characterData, int accountId)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        CharacterData cht = serverData.GetCharacterByLogin(characterData.login);
        string mdPass = XMLF.StrToMD5(characterData.password);

        if (cht != null)
        {
            if (cht.password != mdPass)
            {
                cl.CharacterError(1);
                return;
            }
            if (cht.accountId == accountId)
            {
                cl.CharacterSuccess(cht, 1);
            }
            else
            {
                cl.CharacterError(2);
                return;
            }
        }
        else
        {
            CreateCharacter(netId, characterData.login, characterData.password, characterData.gameStart, accountId);
        }
        return;
    }
    [Command(requiresAuthority = false)]
    public void CheckLogin(uint netId, string login, string gameStart, bool accountCheck)
    {
        bool isLoginExists = serverData.CheckLogin(login, accountCheck);
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        if (isLoginExists && !accountCheck)
        {
            cl.CharacterError(3);
        }
        else
        {
            CharacterData chr = new CharacterData();
            chr.login = login;
            chr.gameStart = gameStart;
            cl.CharacterSuccess(chr, 3);
        }
    }
    [Command(requiresAuthority = false)]
    public void UpdateCharacters(uint netId)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        cl.UpdateCharactersRpc(serverData);
    }
    [Command(requiresAuthority = false)]
    public void UpdateAccount(uint netId)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        cl.UpdateAccountRpc(serverData);
    }
    [Command(requiresAuthority = false)]
    public void SetResourceValueCmd(string login, string name, string subtype, float value)
    {
        SetResourceValue(login, name, subtype, value);
    }
    public void SetResourceValue(string login, string name, string subtype, float value)
    {
        if (subtype == "0")
        {
            AccountData dta = serverData.accounts.Find(f => f.login == login);
            dta.SetResourceValue(name, subtype, value);
        }
        else if (subtype == "1")
        {
            CharacterData dta = serverData.characters.Find(f => f.login == login);
            dta.SetResourceValue(name, subtype, value);
        }
    }
    [Command(requiresAuthority = false)]
    public void AddResourceValueCmd(string login, string name, string subtype, float value)
    {
        AddResourceValue(login, name, subtype, value);
    }
    public void AddResourceValue(string login, string name, string subtype, float value)
    {
        if (subtype == "0")
        {
            AccountData dta = serverData.accounts.Find(f => f.login == login);
            dta.AddResourceValue(name, subtype, value);
        }
        else if (subtype == "1")
        {
            CharacterData dta = serverData.characters.Find(f => f.login == login);
            dta.AddResourceValue(name, subtype, value);
        }
    }
    [Command(requiresAuthority = false)]
    public void SaveServerData()
    {
        serverData.SaveServerData();
    }
    [Command(requiresAuthority = false)]
    public void LoadServerData()
    {
        serverData.SaveServerData();
    }
    [Command(requiresAuthority = false)]
    public void SendAccountData(AccountData accountData)
    {

    }
    [Command(requiresAuthority = false)]
    public void SendCharacterData(CharacterData characterData)
    {

    }
}
