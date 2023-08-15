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
    public static SpaceObject controlledObject;
    private Galaxy galaxy;
    private StarSystem system;
    private Sector sector;
    private int[] sectorIndexes = { 0, 0, 0 };
    private Zone zone;
    private int[] zoneIndexes = { 0, 0, 0 };

    public Galaxy Galaxy { get => galaxy; set => galaxy = value; }
    public StarSystem System { get => system; set => system = value; }
    public Sector Sector { get => sector; set => sector = value; }
    public int[] SectorIndexes { get => sectorIndexes; set => sectorIndexes = value; }
    public Zone Zone { get => zone; set => zone = value; }
    public int[] ZoneIndexes { get => zoneIndexes; set => zoneIndexes = value; }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            FixSpace();
            ConfigData cdata = GameManager.singleton.configData;
            ServerDataManager.singleton.CheckAccount(netId, cdata.login, password);
            singleton = this;
            UpdateCharacters();
        }
    }
    public void Update()
    {
        FixPos();
        Sun.InvokeFixLightDir();
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
            DebugConsole.Log($"Character {characterData.login} {characterData.systemId} already exists", "success");
        }
        else if (code == 2)
        {
            DebugConsole.Log($"New character {characterData.login} created", "success");
        }
        else if (code == 3)
        {
            DebugConsole.Log($"Login ok", "success");
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
        if (controlledObject)
        {
            controlledObject.transform.localPosition = warpData.position;
            controlledObject.transform.localEulerAngles = warpData.rotation;
            for (int i = 0; i < SpaceManager.singleton.starSystems.Count; i++)
            {
                SpaceManager.singleton.starSystems[i].Destroy();
            }
            for (int i = 0; i < SpaceManager.singleton.sectors.Count; i++)
            {
                SpaceManager.singleton.sectors[i].Destroy();
            }
            for (int i = 0; i < SpaceManager.singleton.zones.Count; i++)
            {
                SpaceManager.singleton.zones[i].Destroy();
            }
            for (int i = 0; i < SpaceManager.singleton.suns.Count; i++)
            {
                SpaceManager.singleton.suns[i].Destroy();
            }
            for (int i = 0; i < SpaceManager.singleton.planets.Count; i++)
            {
                SpaceManager.singleton.planets[i].Destroy();
            }
            Galaxy = SpaceManager.singleton.GetGalaxyByID(characterData.galaxyId);
            Galaxy.loaded = false;
            SpaceManager.singleton.starSystems = new List<StarSystem>();
            SpaceManager.singleton.sectors = new List<Sector>();
            SpaceManager.singleton.zones = new List<Zone>();
            SpaceManager.singleton.suns = new List<Sun>();
            SpaceManager.singleton.planets = new List<Planet>();
            SpaceManager.singleton.BuildSystem(LocalClient.GetGalaxyId(), LocalClient.GetSystemId());
            FixSpace();
            FixPos(true);
        }
        Planet.InvokeRender();
        Sun.InvokeRender();
    }
    public void FixSpace()
    {
        if (!SpaceManager.singleton || SpaceManager.singleton.galaxies.Count == 0)
        {
            return;
        }
        Galaxy = SpaceManager.singleton.GetGalaxyByID(characterData.galaxyId);
        System = SpaceManager.singleton.GetSystemByID(characterData.galaxyId, characterData.systemId);
        Sector = SpaceManager.singleton.GetSectorByID(characterData.galaxyId, characterData.systemId, characterData.sectorId);
        Zone = SpaceManager.singleton.GetZoneByID(characterData.galaxyId, characterData.systemId, characterData.sectorId, characterData.zoneId);
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
    public void SetZoneIndexes(Vector3 indexes, bool characterToo = false)
    {
        ZoneIndexes = new int[] { (int)indexes.x, (int)indexes.y, (int)indexes.z };
        if (characterToo)
        {
            if (characterData != null)
            {
                characterData.SetZoneIndexes(indexes);
            }
        }
    }
    public Vector3 GetZoneIndexes()
    {
        return new Vector3((int)this.ZoneIndexes[0], (int)this.ZoneIndexes[1], (int)this.ZoneIndexes[2]);
    }
    public void SetSectorIndexes(Vector3 indexes, bool characterToo = false)
    {
        SectorIndexes = new int[] { (int)indexes.x, (int)indexes.y, (int)indexes.z };
        if (characterToo)
        {
            if (characterData != null)
            {
                characterData.SetSectorIndexes(indexes);
            }
        }
    }
    public Vector3 GetSectorIndexes()
    {
        return new Vector3((int)this.SectorIndexes[0], (int)this.SectorIndexes[1], (int)this.SectorIndexes[2]);
    }
    public Vector3 GetObjectPosition()
    {
        Vector3 sectorPosition = Vector3.zero;
        Vector3 zonePosition = Vector3.zero;
        Vector3 playerPos = Vector3.zero;
        Vector3 ret = Vector3.zero;
        if (Sector != null)
        {
            sectorPosition = Sector.GetIndexes();
        }
        if (Zone != null)
        {
            zonePosition = Zone.GetIndexes();
        }
        if (controlledObject)
        {
            playerPos = controlledObject.transform.localPosition;
        }
        ret = (sectorPosition + zonePosition) * Zone.zoneStep;
        DebugConsole.Log($"{ret}");
        return ret;
    }
    public int GetGalaxyId()
    {
        return characterData.galaxyId;
    }
    public int GetSystemId()
    {
        return characterData.systemId;
    }
    public int GetSectorId()
    {
        return characterData.sectorId;
    }
    public int GetZoneId()
    {
        return characterData.zoneId;
    }
    public Galaxy GetGalaxy()
    {
        Galaxy ret = null;
        ret = SpaceManager.singleton.GetGalaxyByID(GetGalaxyId());
        return ret;
    }
    public StarSystem GetSystem()
    {
        StarSystem ret = null;
        ret = SpaceManager.singleton.GetSystemByID(GetGalaxyId(), GetSystemId());
        return ret;
    }
    public Sector GetSector()
    {
        Sector ret = null;
        ret = SpaceManager.singleton.GetSectorByID(GetGalaxyId(), GetSystemId(), GetSectorId());
        return ret;
    }
    public Zone GetZone()
    {
        Zone ret = null;
        ret = SpaceManager.singleton.GetZoneByID(GetGalaxyId(), GetSystemId(), GetSectorId(), GetZoneId());
        return ret;
    }
    public bool GetIsGameStartDataLoaded()
    {
        return characterData.isGameStartDataLoaded;
    }
    public void SetIsGameStartDataLoaded(bool value)
    {
        characterData.isGameStartDataLoaded = value;
        ServerDataManager.singleton.SendCharacterData(singleton.characterData);
    }
    public string GetGamestartTemplateName()
    {
        string ret = null;
        ret = singleton.characterData.gameStart;
        return ret;
    }
    public void FixPos(bool force = false)
    {
        if (netIdentity != null && isLocalPlayer)
        {
            if (Zone == null || Sector == null)
            {
                return;
            }
            if (!controlledObject)
            {
                return;
            }
            Vector3 curSIndexes = GetSectorIndexes();
            Vector3 curZIndexes = GetZoneIndexes();
            Vector3 znPos = Vector3.zero;
            Vector3 rzn = Vector3.zero;
            znPos = Zone.GetPosition() + controlledObject.transform.localPosition;
            znPos = GameContent.Space.RecalcPos(znPos, Zone.zoneStep) / Zone.zoneStep;
            Vector3 secPos = Vector3.zero;
            secPos = Sector.GetPosition();
            secPos = GameContent.Space.RecalcPos(secPos, Sector.sectorStep) / Sector.sectorStep;
            if (curZIndexes != znPos || force)
            {
                SetZoneIndexes(znPos, true);
                if (Zone.id == 0)
                {
                    Zone.SetIndexes(znPos);
                    Zone.SetPosition(znPos * Zone.zoneStep);
                }
                controlledObject.transform.localPosition = -(GameContent.Space.RecalcPos(controlledObject.transform.localPosition, Zone.zoneStep) - controlledObject.transform.localPosition);
                SpaceManager.singleton.spaceContainer.transform.localPosition = -GameContent.Space.RecalcPos(secPos * Sector.sectorStep + znPos * Zone.zoneStep, Zone.zoneStep);
            }
        }
    }
    public GameStartData GetGameStart()
    {
        return ServerDataManager.singleton.serverData.gameStarts.Find(f => f.templateName == GetGamestartTemplateName());
    }
}
