using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Data;
using GameContent;

public class NetClient : NetworkBehaviour
{
    public string login;
    string password = "";
    public AccountData accountData;
    public CharacterData characterData;
    public static NetClient singleton;
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            ConfigData cdata = GameManager.singleton.configData;
            ServerDataManager.singleton.CheckAccount(netId, cdata.login, password);
            singleton = this;
            UpdateCharacters();
        }
    }
    [TargetRpc]
    public void UpdateCharactersRpc(ServerData serverData)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        ServerDataManager.singleton.serverData = serverData;
        if (characterData != null)
        {
            characterData = ServerDataManager.singleton.serverData.GetCharacterByLogin(characterData.login);
        }
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        pn.UpdateCharacters(ServerDataManager.singleton.serverData);
    }
    [TargetRpc]
    public void UpdateAccountRpc(ServerData serverData)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        ServerDataManager.singleton.serverData = serverData;
        accountData = ServerDataManager.singleton.serverData.GetAccountByLogin(accountData.login);
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        pn.UpdateAccount(ServerDataManager.singleton.serverData);
    }
    public void DeleteCharacter(string login)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        ServerDataManager.singleton.DeleteCharacter(netId, login);
    }
    public void UpdateAccounts()
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        ServerDataManager.singleton.UpdateAccount(netId);
    }
    public void UpdateCharacters()
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        ServerDataManager.singleton.UpdateCharacters(netId);
    }
    public void SetResourceValue(string login, string name, string subtype, float value)
    {
        if (isServer)
        {
            ServerDataManager.singleton.SetResourceValue(login, name, subtype, value);
        }
        else
        {
            ServerDataManager.singleton.SetResourceValueCmd(login, name, subtype, value);
        }
    }
    public void AddResourceValue(string login, string name, string subtype, float value)
    {
        if (isServer)
        {
            ServerDataManager.singleton.AddResourceValue(login, name, subtype, value);
        }
        else
        {
            ServerDataManager.singleton.AddResourceValueCmd(login, name, subtype, value);
        }
    }
    [TargetRpc]
    public void AccountError(int errorCode)
    {
        if (errorCode == 1)
        {
            DebugConsole.Log("Wrong password", "error");
        }
    }
    [TargetRpc]
    public void AccountSuccess(AccountData accountData, int code)
    {
        this.accountData = accountData;
        if (code == 1)
        {
            DebugConsole.Log($"Account {accountData.login} already exists", "success");
        }
        else if (code == 2)
        {
            DebugConsole.Log($"New account {accountData.login} created", "success");
        }
    }
    [TargetRpc]
    public void CharacterError(int errorCode)
    {
        if (errorCode == 1)
        {
            DebugConsole.Log("Wrong password", "error");
        }
        else if (errorCode == 2)
        {
            DebugConsole.Log("Wrong account id", "error");
        }
        else if (errorCode == 3)
        {
            DebugConsole.Log("Login already exists", "error");
            NewCharacterClientPanel panel = ClientPanelManager.GetPanel<NewCharacterClientPanel>();
            panel.SetState(LangSystem.ShowText(1000, 4, 5));
        }
    }
    [TargetRpc]
    public void CharacterSuccess(CharacterData characterData, int code)
    {
        this.characterData = characterData;
        NewCharacterClientPanel panel = ClientPanelManager.GetPanel<NewCharacterClientPanel>();
        if (code == 1)
        {
            DebugConsole.Log($"Character {characterData.login} already exists", "success");
        }
        else if (code == 2)
        {
            DebugConsole.Log($"New character {characterData.login} created", "success");
        }
        else if (code == 3)
        {
            DebugConsole.Log($"Login ok", "success");
            WarpData warpData = new WarpData();
            warpData.galaxyId = characterData.galaxyId;
            warpData.systemId = characterData.systemId;
            warpData.sectorId = characterData.sectorId;
            warpData.zoneId = characterData.zoneId;
            warpData.position = characterData.GetPosition();
            warpData.rotation = characterData.GetRotation();
            WarpClient(warpData);
            ServerDataManager.singleton.CheckCharacter(netId, characterData, accountData.id);
            panel.SetState(null);
            panel.Close();
        }
        UpdateCharacters();
    }
    [TargetRpc]
    public void CompleteWarp(WarpData warpData)
    {
        characterData.galaxyId = warpData.galaxyId;
        characterData.systemId = warpData.systemId;
        characterData.sectorId = warpData.sectorId;
        characterData.zoneId = warpData.zoneId;
        characterData.SetPosition(warpData.position);
        characterData.SetRotation(warpData.rotation);
        DebugConsole.Log($"Character warp data:/n{characterData.galaxyId}");
    }
    public void CheckLogin(string login, string gameStart)
    {
        DebugConsole.Log($"CheckLogin {gameStart}");
        ServerDataManager.singleton.CheckLogin(netId, login, gameStart, false);
    }
    public void WarpClient(WarpData warpData)
    {
        ServerDataManager.singleton.WarpClient(characterData, netId, warpData);
    }
    public static int GetGalaxyId()
    {
        return singleton.characterData.galaxyId;
    }
    public static int GetSystemId()
    {
        return singleton.characterData.systemId;
    }
    public static int GetSectorId()
    {
        return singleton.characterData.sectorId;
    }
    public static int GetZoneId()
    {
        return singleton.characterData.zoneId;
    }
    public static Galaxy GetGalaxy()
    {
        Galaxy ret = null;
        ret = SpaceManager.singleton.GetGalaxyByID(GetGalaxyId());
        return ret;
    }
    public static StarSystem GetSystem()
    {
        StarSystem ret = null;
        ret = SpaceManager.singleton.GetSystemByID(GetGalaxyId(), GetSystemId());
        return ret;
    }
    public static Sector GetSector()
    {
        Sector ret = null;
        ret = SpaceManager.singleton.GetSectorByID(GetGalaxyId(), GetSystemId(), GetSectorId());
        return ret;
    }
    public static Zone GetZone()
    {
        Zone ret = null;
        ret = SpaceManager.singleton.GetZoneByID(GetGalaxyId(), GetSystemId(), GetSectorId(), GetZoneId());
        return ret;
    }
    public static string GetGamestartTemplateName()
    {
        string ret = null;
        ret = singleton.characterData.gameStart;
        return ret;
    }
}
