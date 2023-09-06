using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Data;
using GameContent;
using Unity.VisualScripting;

public class NetClient : NetworkBehaviour
{
    public string login;
    string password = "";
    public AccountData accountData;
    public CharacterData characterData;
    public static NetClient singleton;
    public uint controlledObjectNetId;
    private SpaceObject controlledObject;
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
    public SpaceObject ControlledObject
    {
        get => controlledObject; set
        {
            controlledObject = value;
            controlledObjectNetId = value.netId;
        }
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            FixSpace();
            ConfigData cdata = GameManager.singleton.configData;
            ServerDataManager.singleton.CheckAccount(netId, cdata.login, password);
            CharacterData chd = ServerDataManager.singleton.ServerData.GetCharacterByLogin(cdata.login);
            if (chd != null)
            {

            }
            DebugConsole.Log(characterData.GetZoneIndexes());
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
    public void RenderLocal(SpaceObjectData spaceObjectData)
    {
        if (NetClient.singleton == null || isServer)
        {
            return;
        }
        NetClient.singleton.controlledObjectNetId = spaceObjectData.netId;
        SpaceObject spaceObject = NetworkClient.spawned[spaceObjectData.netId].GetComponent<SpaceObject>();
        spaceObject.galaxyId = spaceObjectData.galaxyId;
        spaceObject.systemId = spaceObjectData.systemId;
        spaceObject.sectorId = spaceObjectData.sectorId;
        spaceObject.zoneId = spaceObjectData.zoneId;
        spaceObject.SetSectorIndexes(spaceObjectData.GetSectorIndexes());
        spaceObject.SetZoneIndexes(spaceObjectData.GetZoneIndexes());
        spaceObject.templateName = spaceObjectData.templateName;
        spaceObject.LoadValues();
        spaceObject.LoadHardpoints();
        spaceObject.isPlayerControll = true;
        SpaceObject.InvokeRender();
        SpaceObject.InvokeNetStart();
    }
    [TargetRpc]
    public void UpdateCharactersRpc(ServerData serverData)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        ServerDataManager.singleton.ServerData = serverData;
        if (characterData != null)
        {
            characterData = ServerDataManager.singleton.ServerData.GetCharacterByLogin(characterData.login);
            if (characterData != null)
            {
                NetClient.singleton.characterData = characterData;
                NetClient.singleton.SetZoneIndexes(NetClient.singleton.characterData.GetZoneIndexes());
                NetClient.singleton.SetSectorIndexes(NetClient.singleton.characterData.GetSectorIndexes());
            }
        }
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        pn.UpdateCharacters(ServerDataManager.singleton.ServerData);
    }
    [TargetRpc]
    public void UpdateAccountRpc(ServerData serverData)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        ServerDataManager.singleton.ServerData = serverData;
        accountData = ServerDataManager.singleton.ServerData.GetAccountByLogin(accountData.login);
        CharactersClientPanel pn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        pn.UpdateAccount(ServerDataManager.singleton.ServerData);
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
        characterData.SetSectorIndexes(warpData.GetSectorIndexes());
        characterData.SetZoneIndexes(warpData.GetZoneIndexes());

        if (ControlledObject)
        {
            ControlledObject.transform.localPosition = warpData.position;
            ControlledObject.transform.localEulerAngles = warpData.rotation;
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
            ControlledObject.galaxyId = warpData.galaxyId;
            ControlledObject.systemId = warpData.systemId;
            ControlledObject.sectorId = warpData.sectorId;
            ControlledObject.zoneId = warpData.zoneId;
            ControlledObject.SetSectorIndexes(warpData.GetSectorIndexes());
            ControlledObject.SetZoneIndexes(warpData.GetZoneIndexes());
            FixSpace();
            if (Zone.id == 0)
            {
                Zone.SetPosition(warpData.GetZoneIndexes() * Zone.zoneStep);
            }
            FixPos(true);
        }
        SpaceObject.InvokeRender();
        Planet.InvokeRender();
        Sun.InvokeRender();

        Color32 color = system.GetBgColor();
        color = new Color32((byte)(color.r), (byte)(color.g), (byte)(color.b), color.a);
        RenderSettings.skybox.SetColor("_Tint", color);
        RenderSettings.skybox.ComputeCRC();
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
                //ServerDataManager.singleton.SendCharacterData(netId, characterData);
            }
        }
    }
    public Vector3 GetZoneIndexes()
    {
        if (characterData != null)
        {
            return characterData.GetZoneIndexes();
        }
        else
        {
            return new Vector3((int)this.ZoneIndexes[0], (int)this.ZoneIndexes[1], (int)this.ZoneIndexes[2]);
        }
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
        if (characterData != null)
        {
            return characterData.GetSectorIndexes();
        }
        else
        {
            return new Vector3((int)this.SectorIndexes[0], (int)this.SectorIndexes[1], (int)this.SectorIndexes[2]);
        }
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
        if (ControlledObject)
        {
            playerPos = ControlledObject.transform.localPosition;
        }
        ret = (sectorPosition + zonePosition) * Zone.zoneStep;
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
            if (!ControlledObject)
            {
                return;
            }
            Vector3 curSIndexes = GetSectorIndexes();
            Vector3 curZIndexes = GetZoneIndexes();

            Vector3 znPos = Vector3.zero;
            Vector3 rzn = Vector3.zero;
            znPos = Zone.GetPosition() + ControlledObject.transform.localPosition;
            znPos = GameContent.Space.RecalcPos(znPos, Zone.zoneStep) / Zone.zoneStep;
            Vector3 secPos = Vector3.zero;
            secPos = Sector.GetPosition();
            secPos = GameContent.Space.RecalcPos(secPos, Sector.sectorStep) / Sector.sectorStep;

            if (curZIndexes != znPos || force)
            {
                Sector findSector = SpaceManager.singleton.sectors.Find(f => f.GetIndexes() == GetSectorIndexes() && f.galaxyId == GetGalaxyId() && f.systemId == GetSystemId());
                Zone findZone = SpaceManager.singleton.zones.Find(f => f.GetIndexes() == znPos && f.galaxyId == GetGalaxyId() && f.systemId == GetSystemId() && findSector == GetSector());
                if (findZone == null)
                {
                    Zone = SpaceManager.singleton.GetZoneByID(GetGalaxyId(), GetSystemId(), GetSectorId(), 0);
                    characterData.zoneId = 0;
                }
                if (Zone.id == 0)
                {
                    Zone.SetIndexes(znPos);
                    Zone.SetPosition(znPos * Zone.zoneStep);
                }
                
                ControlledObject.transform.localPosition = -(GameContent.Space.RecalcPos(ControlledObject.transform.localPosition, Zone.zoneStep) - ControlledObject.transform.localPosition);
                SpaceManager.singleton.spaceContainer.transform.localPosition = -GameContent.Space.RecalcPos(secPos * Sector.sectorStep + znPos * Zone.zoneStep, Zone.zoneStep);
                DebugConsole.Log($"{secPos} {znPos} {SpaceManager.singleton.spaceContainer.transform.localPosition}");
                SetSectorIndexes(curSIndexes, true);
                SetZoneIndexes(znPos, true);
                ControlledObject.SetZoneIndexes(znPos);
            }
        }
    }
    public GameStartData GetGameStart()
    {
        return ServerDataManager.singleton.ServerData.gameStarts.Find(f => f.templateName == GetGamestartTemplateName());
    }
}
