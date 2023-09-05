using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ServerDataManager : NetworkBehaviour
{
    private ServerData serverData;
    public static ServerDataManager singleton;

    public ServerData ServerData
    {
        get => serverData; set
        {
            serverData = value;
        }
    }

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
        singleton.ServerData = new ServerData();
    }
    public void CreateAccount(uint netId, string login, string password)
    {
        AccountData adt = ServerData.CreateAccountData(login, password);
        DebugConsole.ShowErrorIsNull(adt, $"Account {login} already exists");
        if (adt != null)
        {
            NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
            cl.AccountSuccess(adt, 2);
        }
    }
    public void CreateCharacter(uint netId, string login, string password, string gameStart, int accountId)
    {
        CharacterData cht = ServerData.CreateCharacterData(login, password, gameStart, accountId);
        DebugConsole.ShowErrorIsNull(cht, $"Character {login} already exists");
        if (cht != null)
        {
            NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();

            GameStartData gameStartData = ServerData.gameStarts.Find(f => f.templateName == gameStart);
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
        AccountData adt = ServerData.GetAccountByLogin(login);
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
        CharacterData cht = ServerData.characters.Find(f => f.login == login);
        if (cht != null)
        {
            int ind = ServerData.characters.IndexOf(cht);
            ServerData.characters.Remove(ServerData.characters[ind]);
            cl.UpdateCharactersRpc(ServerData);
        }
    }
    [Command(requiresAuthority = false)]
    public void CheckCharacter(uint netId, CharacterData characterData, int accountId)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        CharacterData cht = ServerData.GetCharacterByLogin(characterData.login);
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
        bool isLoginExists = ServerData.CheckLogin(login, accountCheck);
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
        cl.UpdateCharactersRpc(ServerData);
    }
    [Command(requiresAuthority = false)]
    public void UpdateAccount(uint netId)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        cl.UpdateAccountRpc(ServerData);
    }
    [Command(requiresAuthority = false)]
    public void LoadGameStartObjects(uint netId)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        SpaceObjectManager.LoadGameStartObjects(cl);
        SpaceObject.InvokeRender();
    }
    [Command(requiresAuthority = false)]
    public void SendCharacterData(uint netId, CharacterData characterData)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        CharacterData chr = ServerData.GetCharacterByLogin(characterData.login);
        if (chr != null)
        {
            int ind = ServerData.characters.IndexOf(chr);
            ServerData.characters[ind] = characterData;
            cl.characterData = ServerData.characters[ind];
            cl.UpdateCharactersRpc(ServerData);
        }
    }
    [Command(requiresAuthority = false)]
    public void SendAccountData(uint netId, AccountData accountData)
    {
        NetClient cl = NetworkServer.spawned[netId].GetComponent<NetClient>();
        AccountData acd = ServerData.GetAccountByLogin(accountData.login);
        if (acd != null)
        {
            int ind = ServerData.accounts.IndexOf(acd);
            ServerData.accounts[ind] = acd;
            cl.accountData = ServerData.accounts[ind];
            cl.UpdateAccountRpc(ServerData);
        }
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
            AccountData dta = ServerData.accounts.Find(f => f.login == login);
            dta.SetResourceValue(name, subtype, value);
        }
        else if (subtype == "1")
        {
            CharacterData dta = ServerData.characters.Find(f => f.login == login);
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
            AccountData dta = ServerData.accounts.Find(f => f.login == login);
            dta.AddResourceValue(name, subtype, value);
        }
        else if (subtype == "1")
        {
            CharacterData dta = ServerData.characters.Find(f => f.login == login);
            dta.AddResourceValue(name, subtype, value);
        }
    }
    [Command(requiresAuthority = false)]
    public void SaveServerData()
    {
        SaveSpaceObjects();
        ServerData.SaveServerData();
    }
    [Command(requiresAuthority = false)]
    public void SaveSpaceObjects()
    {
        ServerData.spaceObjectDatas = new List<SpaceObjectData>();
        for (int i = 0; i < SpaceObjectManager.spaceObjects.Count; i++)
        {
            SpaceObject spaceObject = SpaceObjectManager.spaceObjects[i];
            SpaceObjectData spd = spaceObject.GetSpaceObjectData();
            ServerData.spaceObjectDatas.Add(spd);
        }
    }
    [Command(requiresAuthority = false)]
    public void LoadSpaceObjects()
    {
        string textString = "";

        textString += $"\n<color=green>||||||||||||||||||||||||||||||||||||||</color>\n\n";
        for (int i = 0; i < singleton.ServerData.spaceObjectDatas.Count; i++)
        {
            SpaceObjectData spd = ServerDataManager.singleton.ServerData.spaceObjectDatas[i];
            textString += $"<color=white>Type: </color><color=green>{spd.type}</color>\n";
            textString += $"<color=white>Sector indexes: </color><color=green>{spd.GetSectorIndexes()}</color>\n";
            textString += $"<color=white>Zone indexes: </color><color=green>{spd.GetZoneIndexes()}</color>\n";
            textString += $"<color=white>Local Sector indexes: </color><color=green>{LocalClient.GetSectorIndexes()}</color>\n";
            textString += $"<color=white>Local Zone indexes: </color><color=green>{LocalClient.GetZoneIndexes()}</color>\n";
            textString += $"<color=white>Position: </color><color=green>{spd.GetPosition()}</color>\n";
            textString += $"<color=white>Rotation: </color><color=green>{spd.GetRotation()}</color>\n";
            textString += $"<color=white>Galaxy id: </color><color=green>{LocalClient.GetGalaxyId()}</color>\n";
            textString += $"<color=white>System id: </color><color=green>{LocalClient.GetSystemId()}</color>\n";
            textString += $"<color=white>Sector id: </color><color=green>{LocalClient.GetSectorId()}</color>\n";
            textString += $"<color=blue>--------------------------------------\n</color>\n";

            SpaceObject spaceObject = spd.CreateByType();
            spaceObject.ReadSpaceObjectData(spd);
            Template template = TemplateManager.FindTemplate(spaceObject.templateName, spd.type);
            TemplateNode paramsNode = template.GetNode("params");
            if (paramsNode != null)
            {
                spaceObject.mass = int.Parse(paramsNode.GetValue("mass"));
                spaceObject.drag = XMLF.FloatVal(paramsNode.GetValue("drag"));
                spaceObject.angulardrag = XMLF.FloatVal(paramsNode.GetValue("angulardrag"));
            }
            SpaceObjectManager.spaceObjects.Add(spaceObject);
            spaceObject.LoadHardpoints();
            spaceObject.transform.SetParent(SpaceManager.singleton.spaceContainer.transform);
            spaceObject.transform.localPosition = GameContent.Space.RecalcPos(spaceObject.GetSectorIndexes() * Sector.sectorStep + spaceObject.GetZoneIndexes() * Zone.zoneStep, Zone.zoneStep);
            spaceObject.transform.localEulerAngles = spd.GetRotation();
            NetworkServer.Spawn(spaceObject.gameObject);
            spaceObject.ServerInit();
        }
        textString += $"<color=green>||||||||||||||||||||||||||||||||||||||</color>\n\n";
        //DebugConsole.Log(textString);

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
