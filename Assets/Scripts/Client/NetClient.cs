using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Data;

public class NetClient : NetworkBehaviour
{
    string login = "Efim";
    string password = "!#1Efim1#!";
    public AccountData accountData;
    public CharacterData characterData;
    public static NetClient singleton;
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            ServerDataManager.singleton.CheckAccount(netId, login, password);
            singleton = this;
            UpdateCharacters();
        }
    }
    public void UpdateCharacters()
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        pn.UpdateCharacters(ServerDataManager.singleton.serverData);
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
}
