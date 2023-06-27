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
    public void CreateCharacter(uint netId, string login, string password, int accountId)
    {
        CharacterData cht = serverData.CreateCharacterData(login, password, accountId);
        DebugConsole.ShowErrorIsNull(cht, $"Character {login} already exists");
        if (cht != null)
        {
            NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
            cl.CharacterSuccess(cht, 2);
        }
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
    public void CheckCharacter(uint netId, string login, string password, int accountId)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        CharacterData cht = serverData.GetCharacterByLogin(login);
        string mdPass = XMLF.StrToMD5(password);

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
            CreateCharacter(netId, login, password, accountId);
        }
        return;
    }
    [Command(requiresAuthority = false)]
    public void CheckLogin(uint netId, string login, bool accountCheck)
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
            cl.CharacterSuccess(chr, 3);
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
