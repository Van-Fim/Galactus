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
    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            ServerDataManager.singleton.CheckAccount(netId, login, password);
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
            DebugConsole.Log($"Account already exists", "success");
        }
        else if (code == 2)
        {
            DebugConsole.Log($"New account created", "success");
        }
        ServerDataManager.singleton.CheckCharacter(netId, "Char01", "", accountData.id);
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
    }
    [TargetRpc]
    public void CharacterSuccess(CharacterData characterData, int code)
    {
        this.characterData = characterData;
        if (code == 1)
        {
            DebugConsole.Log($"Character already exists", "success");
        }
        else if (code == 2)
        {
            DebugConsole.Log($"New character created", "success");
        }
    }
}
