using System.IO;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;
public class Client : MonoBehaviour
{
    public string login = "PlayerusChar01";
    public string accountLogin = "Playerus";
    public int galaxyId;
    public int systemId;
    public int sectorId;
    public int zoneId;

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

    public static void Init()
    {
        localClient = new GameObject().AddComponent<Client>();
        localClient.serverIdentity = localClient.GenerateAccountData();
        localClient.characterData = localClient.LoadCharacterData();
    }

    void Start()
    {
        OnChangedZone += ZoneChanged;
    }

    void Update()
    {
        Vector3 znPos = currZone.GetPosition() + transform.localPosition;
        znPos = Space.RecalcPos(znPos, Zone.zoneStep)/Zone.zoneStep;

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
