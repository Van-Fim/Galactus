using System.IO;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Mirror;
using UnityEngine.Events;
public class Client : NetworkBehaviour
{
    [SyncVar]
    public string login = "PlayerusChar01";
    [SyncVar]
    public string accountLogin = "Playerus";
    [SyncVar]
    public int galaxyId;
    [SyncVar]
    public int systemId;
    [SyncVar]
    public int sectorId;
    [SyncVar]
    public int zoneId;

    [SyncVar]
    public bool startGameStarted;
    [SyncVar]
    public GameStartData gameStartData;
    [SyncVar]
    public uint targetId;

    [SyncVar]
    public Vector3 currZoneIndexes = Vector3.zero;
    [SyncVar]
    public Vector3 currSectorIndexes = Vector3.zero;
    public SPObject controllTarget;

    public NetworkTransform networkTransform;

    public static UnityAction<Zone> OnChangedZone;

    public Galaxy currGalaxy;
    public StarSystem currSystem;
    public Sector currSector;
    public Zone currZone;

    public static Client localClient;

    CharacterData characterData;
    string serverIdentity;

    [System.Serializable]
    public struct AccountData
    {
        public string accountlogin;
        public string serverIdentity;
        public int accountCash;
    }

    [System.Serializable]
    public struct CharacterData
    {
        public string login;
        public string serverIdentity;
        public int characterCash;
    }

    public void Init()
    {
        GenerateAccountData();
        LoadCharacterData();
    }

    void Start()
    {

    }

    [Command]
    public void UpdateGlobalPos()
    {
        ClientManager.singleton.UpdaeGlobalClientPos();
    }

    void Update()
    {
        UPD();
    }

    public void UPD(bool forceUpdate = false)
    {
        if (isLocalPlayer)
        {
            if (currZone == null || currSector == null)
            {
                return;
            }
            if (controllTarget == null)
            {
                return;
            }
            Vector3 znPos = Vector3.zero;
            znPos = currZone.GetPosition() + controllTarget.transform.localPosition;
            znPos = Space.RecalcPos(znPos, Zone.zoneStep) / Zone.zoneStep;

            int curGalaxyId = currZone.galaxyId;
            int curSystemId = currZone.systemId;
            int curSectorId = currZone.sectorId;
            int curZoneId = currZone.id;

            bool checkSpace = currZone.galaxyId == curGalaxyId && currZone.systemId == curSystemId && currZone.sectorId == curSectorId && currZone.id == curZoneId;
            if (currZone.GetIndexes() != znPos || !checkSpace || forceUpdate)
            {
                Zone fzn = SpaceManager.zones.Find(f => f.galaxyId == currGalaxy.id && f.systemId == currSystem.id && f.sectorId == currSector.id && f.GetIndexes() == znPos);

                if (fzn == null)
                {
                    fzn = SpaceManager.zones.Find(f => f.galaxyId == currGalaxy.id && f.systemId == currSystem.id && f.sectorId == currSector.id && f.id == 0);
                    fzn.SetIndexes(znPos);
                    fzn.SetPosition(znPos * Zone.zoneStep);
                    if (fzn.spaceController != null)
                    {
                        fzn.spaceController.transform.localPosition = fzn.GetPosition() / Zone.minimapDivFactor;
                    }
                }

                Client.localClient.galaxyId = curGalaxyId;
                Client.localClient.systemId = curSystemId;
                Client.localClient.sectorId = curSectorId;
                Client.localClient.zoneId = curZoneId;
                Client.localClient.ReadSpace();
                currSectorIndexes = currSector.GetIndexes();
                currZoneIndexes = currZone.GetIndexes();
                //UpdateGlobalPos();

                InvokeOnChangedZone(fzn);
            }
        }
        else
        {
            if (targetId > 0)
            {
                SPObject sp = NetworkClient.spawned[targetId].GetComponent<SPObject>();
                controllTarget = sp;
                if (sp.transform.parent == null && sp.clientContainer == null)
                {
                    GameObject container = new GameObject();
                    sp.transform.SetParent(container.transform);
                    sp.clientContainer = container;
                    container.name = $"Player_{netId}_container";
                }
                ReadSpace();
                if (sp.clientContainer != null)
                {
                    sp.clientContainer.transform.localPosition = SpaceManager.spaceContainer.transform.localPosition + currSector.GetPosition() + currZoneIndexes * Zone.zoneStep;
                }
            }
        }
    }

    [Command]
    public void WarpCompleted(uint netId, int galaxyId, int systemId, int sectorId, int zoneid)
    {
        SPObjectManager.singleton.InvokeRenderClient(netId, galaxyId, systemId, sectorId, zoneId);
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            localClient = this;
            OnChangedZone += ZoneChanged;
            if (!(isClient && isServer))
            {
                SpaceManager.Init();
                SpaceManager.singleton.Load();
            }
            InitGamestart("Start01");
        }
        else
        {
            ReadSpace();
        }
        gameObject.name = $"Player_{netId}";
    }

    [Command]
    public void InitGamestart(string data)
    {
        gameStartData = GameStartData.Init(data);
        galaxyId = gameStartData.galaxyId;
        systemId = gameStartData.starSystemId;
        sectorId = gameStartData.sectorId;
        zoneId = gameStartData.zoneId;
        startGameStarted = true;
        gameStartData.LoadContent(this);
        ControllTargetSpawned(gameStartData, targetId);
    }

    public void ContinueInit()
    {
        ClientPanelManager.Show<HudClientPanel>();

        WarpClient(galaxyId, systemId, sectorId, zoneId);
        UPD(true);
    }

    [ClientRpc]
    public void ControllTargetSpawned(GameStartData gameStartData, uint netId)
    {
        if (netId > 0)
        {
            controllTarget = NetworkClient.spawned[netId].GetComponent<SPObject>();
            if (isLocalPlayer)
            {
                this.gameStartData = gameStartData;

                localClient = this;
                galaxyId = gameStartData.galaxyId;
                systemId = gameStartData.starSystemId;
                sectorId = gameStartData.sectorId;
                zoneId = gameStartData.zoneId;
                this.ReadSpace();
                controllTarget.SetPlayerControll();

                ContinueInit();
            }
        }
        SPObject.InvokeRender();
    }

    public void WarpClient(int galaxyId, int systemId, int sectorId, int zoneId)
    {
        if (sectorId == -1)
        {
            sectorId = 0;
        }
        if (zoneId == -1)
        {
            zoneId = 0;
        }

        Galaxy galaxy = SpaceManager.GetGalaxyByID(galaxyId);
        if (galaxy == null)
        {
            galaxyId = 0;
            galaxy = SpaceManager.GetGalaxyByID(galaxyId);
        }
        StarSystem system = SpaceManager.GetSystemByID(galaxyId, systemId);
        if (system == null)
        {
            systemId = 0;
            system = SpaceManager.GetSystemByID(galaxyId, systemId);
        }
        Sector sector = SpaceManager.GetSectorByID(galaxyId, systemId, sectorId);
        if (sector == null)
        {
            sectorId = 0;
            sector = SpaceManager.GetSectorByID(galaxyId, systemId, sectorId);
        }
        Zone zone = SpaceManager.GetZoneByID(galaxyId, systemId, sectorId, zoneId);
        if (sector == null)
        {
            zoneId = 0;
            zone = SpaceManager.GetZoneByID(galaxyId, systemId, sectorId, zoneId);
        }

        Material mat = Resources.Load<Material>($"Materials/{system.skyboxName}");
        RenderSettings.skybox = mat;

        Color32 color = system.GetBgColor();
        // float cdiv = 1f;
        // color = new Color32((byte)(color.r / cdiv), (byte)(color.g / cdiv), (byte)(color.b / cdiv), color.a);
        color = new Color32((byte)(color.r), (byte)(color.g), (byte)(color.b), color.a);
        RenderSettings.skybox.SetColor("_Tint", color);
        //RenderSettings.skybox.SetColor("_Color", color);
        RenderSettings.skybox.ComputeCRC();

        Client.localClient.galaxyId = galaxyId;
        Client.localClient.systemId = systemId;
        Client.localClient.sectorId = sectorId;
        Client.localClient.zoneId = zoneId;

        if (currZone.id == 0)
        {
            currZone.SetPosition(Vector3.zero);
        }

        if (Client.localClient.targetId > 0)
        {
            SPObject target = NetworkClient.spawned[Client.localClient.targetId].GetComponent<Ship>();
            if (target == null)
            {
                target = NetworkClient.spawned[Client.localClient.targetId].GetComponent<Pilot>();
            }
            target.galaxyId = galaxyId;
            target.systemId = systemId;
            target.sectorId = sectorId;
            target.zoneId = zoneId;

            target.ReadSpace();
        }

        Client.localClient.ReadSpace();

        Vector3 ralPos = sector.GetPosition() + zone.GetPosition();
        Vector3 recPos = Space.RecalcPos(sector.GetPosition() + zone.GetPosition(), Zone.zoneStep);

        SpaceManager.spaceContainer.transform.localPosition = -recPos;

        WarpCompleted(netId, galaxyId, systemId, sectorId, zoneId);
    }

    public void ZoneChanged(Zone zone)
    {
        if (controllTarget != null)
        {
            controllTarget.transform.localPosition = -(Space.RecalcPos(controllTarget.transform.localPosition, Zone.zoneStep) - controllTarget.transform.localPosition);
            SpaceManager.spaceContainer.transform.localPosition = -Space.RecalcPos(currSector.GetPosition() + zone.GetPosition(), Zone.zoneStep);
            currZone = zone;
        }
    }

    public void ReadSpace()
    {
        currGalaxy = SpaceManager.GetGalaxyByID(galaxyId);
        currSystem = SpaceManager.GetSystemByID(galaxyId, systemId);
        currSector = SpaceManager.GetSectorByID(galaxyId, systemId, sectorId);
        currZone = SpaceManager.GetZoneByID(galaxyId, systemId, sectorId, zoneId);
    }

    public CharacterData LoadCharacterData()
    {
        CharacterData ret = new CharacterData();
        ret.serverIdentity = serverIdentity;
        ret.login = login;

        string dir = XMLF.GetDirPatch();
        string fileName = "char.data";
        string dirPatch = dir + "/" + serverIdentity + "/" + login + "_data";
        bool newFileAdded = false;
        FileStream file = null;
        if (!Directory.Exists(dirPatch))
        {
            DirectoryInfo di = Directory.CreateDirectory(dirPatch);
        }
        if (File.Exists(dirPatch + "/" + fileName))
        {
            file = File.Open(dir + "/" + fileName, FileMode.Open);
        }
        else
        {
            file = File.Create(dir + "/" + fileName);
            newFileAdded = true;
        }

        BinaryFormatter bf = new BinaryFormatter();

        if (!newFileAdded)
        {
            ret = (CharacterData)bf.Deserialize(file);
            file.Close();
            return ret;
        }

        bf.Serialize(file, ret);
        return ret;
    }

    public string GenerateAccountData()
    {
        string ret = "";
        string dir = XMLF.GetDirPatch();
        string fileName = XMLF.StrToMD5(accountLogin) + "_serverIdentity.data";
        bool newFileAdded = false;
        FileStream file = null;
        if (File.Exists(dir + "/" + fileName))
        {
            file = File.Open(dir + "/" + fileName, FileMode.Open);
            newFileAdded = false;
        }
        else
        {
            file = File.Create(dir + "/" + fileName);
            newFileAdded = true;
        }
        AccountData accountData = new AccountData();
        accountData.accountlogin = accountLogin;
        BinaryFormatter bf = new BinaryFormatter();

        if (!newFileAdded)
        {
            accountData = (AccountData)bf.Deserialize(file);
            file.Close();
            ret = accountData.serverIdentity;
            return ret;
        }

        int rnd = UnityEngine.Random.Range(0, 10);
        int rnd2 = UnityEngine.Random.Range(0, 10000000);
        int count = UnityEngine.Random.Range(5, 10);
        string list = "aeioyuqwrtpsdfghjklzxcvbnm";

        for (int o = 0; o < count; o++)
        {
            int j = UnityEngine.Random.Range(0, list.Length);
            char v1 = list[j];

            ret += v1;
            ret += rnd.ToString();
        }
        string date = DateTime.Now.ToString("MM.dd.yyyy");
        ret = XMLF.StrToMD5((GetLocalIPAddress() + accountData.accountlogin + ret + date) + rnd2.ToString());
        accountData.serverIdentity = ret;

        bf.Serialize(file, accountData);
        return ret;
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public static void InvokeOnChangedZone(Zone zone)
    {
        OnChangedZone?.Invoke(zone);
    }
}
