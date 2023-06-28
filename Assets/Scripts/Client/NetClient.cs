using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Data;
using GameContent;

public class NetClient : NetworkBehaviour
{
    string login = "";
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
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        pn.UpdateCharacters(ServerDataManager.singleton.serverData);
    }
    public void DeleteCharacter(string login)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        ServerDataManager.singleton.DeleteCharacter(netId, login);
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
            ServerDataManager.singleton.CheckCharacter(netId, characterData.login, characterData.password, accountData.id);
            panel.SetState(null);
            panel.Close();
        }
        UpdateCharacters();
    }
    public void CheckLogin(string login)
    {
        ServerDataManager.singleton.CheckLogin(netId, login, false);
    }
}
