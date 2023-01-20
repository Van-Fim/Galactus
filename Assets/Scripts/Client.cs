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
    public uint pilotId;
    public Ship ship;
    public NetworkTransform networkTransform;

    public static UnityAction<Zone> OnChangedZone;

    Galaxy currGalaxy;
    StarSystem currSystem;
    Sector currSector;
    Zone currZone;

    public Vector3 currentZonePos = Vector3.zero;

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

    void Update()
    {
        if (currZone == null)
        {
            return;
        }
        Vector3 znPos = currZone.GetPosition() + transform.localPosition;
        znPos = Space.RecalcPos(znPos, Zone.zoneStep) / Zone.zoneStep;

        if (currZone.GetIndexes() != znPos)
        {
            Zone fzn = SpaceManager.zones.Find(f => f.galaxyId == currGalaxy.id && f.systemId == currSystem.id && f.sectorId == currSector.id && f.GetIndexes() == znPos);
            if (fzn == null)
            {
                fzn = new Zone("Zone00");
                fzn.galaxyId = currGalaxy.id;
                fzn.systemId = currSystem.id;
                fzn.sectorId = currSector.id;
                fzn.SetIndexes(znPos);
                fzn.SetPosition(znPos * Zone.zoneStep);
                fzn.id = -1;
            }
            InvokeOnChangedZone(fzn);
        }
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            OnChangedZone += ZoneChanged;
            SpaceManager.Init();
            SpaceManager.singleton.Load();

            InitGamestart("Start01");
        }
    }

    [Command]
    public void InitGamestart(string data)
    {
        GameStartData gameStartData = GameStartData.Init(data);
        galaxyId = gameStartData.galaxyId;
        systemId = gameStartData.starSystemId;
        sectorId = gameStartData.sectorId;
        zoneId = gameStartData.zoneId;
        startGameStarted = true;
        gameStartData.LoadContent(this);

        PilotSpawned();
    }

    [ClientRpc]
    public void PilotSpawned()
    {
        if (isLocalPlayer)
        {
            localClient = this;
            Pilot pilot = NetworkClient.spawned[pilotId].GetComponent<Pilot>();
            pilot.rigidbodyMain = pilot.gameObject.AddComponent<Rigidbody>();
            pilot.rigidbodyMain.useGravity = false;
            pilot.rigidbodyMain.angularDrag = 2f;
            pilot.rigidbodyMain.drag = 2f;
            pilot.rigidbodyMain.mass = 10f;

            pilot.galaxyId = galaxyId;
            pilot.systemId = systemId;
            pilot.sectorId = sectorId;
            pilot.zoneId = zoneId;

            pilot.controller = pilot.gameObject.AddComponent<PlayerController>();
            pilot.controller.obj = pilot;
            CameraManager.mainCamera.enabled = false;
            CameraManager.mainCamera.transform.SetParent(pilot.transform);
            CameraManager.mainCamera.transform.localPosition = new Vector3(0, 1.6f, -3f);
            SpaceManager.singleton.WarpClient(pilot.galaxyId, pilot.systemId, pilot.sectorId, pilot.zoneId);
        }
        else
        {
            SPObject.InvokeRender();
        }
    }

    public void ZoneChanged(Zone zone)
    {
        //Debug.Log($"{zone.id} {zone.GetIndexes()}");
        transform.localPosition = -(Space.RecalcPos(transform.localPosition, Zone.zoneStep) - transform.localPosition);
        SpaceManager.spaceContainer.transform.localPosition = -Space.RecalcPos(currSector.GetPosition() + zone.GetPosition(), Zone.zoneStep);
        currZone = zone;
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
