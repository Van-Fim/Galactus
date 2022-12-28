using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;

public class NetClient
{
    public static NetClient localClient;

    public string login;
    public string password;
    public string serverIdentity;

    public int galaxyId;
    public int systemId;
    public int sectorId;
    public int zoneId;

    [System.Serializable]
    public struct AccountData
    {
        public string accountlogin;
        public string serverIdentity;

        public int cash;
    }

    public static string StrToMD5(string str)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(str));

        byte[] result = md5.Hash;

        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            strBuilder.Append(result[i].ToString("x2"));
        }

        return strBuilder.ToString();
    }

    public string GenerateServerIdentity()
    {
        string ret = "";
        string dir = NetClient.GetDirPatch();
        string fileName = StrToMD5(ClientManager.accountLogin) + "_serverIdentity.data";
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
        accountData.accountlogin = ClientManager.accountLogin;
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
        ret = StrToMD5((GetLocalIPAddress() + accountData.accountlogin + ret + date) + rnd2.ToString());
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

    public static string GetDirPatch()
    {
        string dirPatch = Application.persistentDataPath + "/GameData";
        if (!Directory.Exists(dirPatch))
        {
            Directory.CreateDirectory(dirPatch);
        }
        return dirPatch;
    }
}
