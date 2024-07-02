using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
public class XMLF
{
    public static float FloatVal(string text)
    {
        text = text.Replace('.', ',');
        return float.Parse(text);
    }
    public static XmlDocument ReadFile(string Patch)
    {
        string FullPatch = Patch;
        if (!File.Exists(FullPatch))
        {
            Debug.LogError("File not found " + FullPatch);
            return null;
        }


        XmlDocument Doc = new XmlDocument();
        Doc.Load(FullPatch);

        return Doc;
    }
    public static string StrToMD5(string str)
    {
        if (str == null || str.Length == 0)
        {
            return null;
        }
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