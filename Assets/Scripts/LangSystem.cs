using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class LangSystem
{
    public static string language = "RU";
    public static void Load()
    {

    }
    public static string ShowText(int section, int page, int id)
    {
        string ret = $"<color=red>[{section}, {page}, {id}]</color>";
        GamePrefabsManager gpm = GamePrefabsManager.singleton;
        for (int i = 0; i < gpm.list.Count; i++)
        {
            string patch = $"{gpm.list[i]}/Languages/{language}/{section}";
            //XmlDocument doc = XMLF.ReadFile(patch);
            TextAsset textAsset = (TextAsset)Resources.Load(patch);

            XmlDocument xDoc = new XmlDocument();
            if (textAsset != null)
            {
                xDoc.LoadXml(textAsset.text);
            }
            else
            {
                xDoc = XMLF.ReadFile(patch);
            }

            XmlElement xRoot = xDoc.DocumentElement;
            XmlNode node = xRoot.SelectSingleNode($"descendant::page[@id='{page}']/text[@id='{id}']");
            if (node != null)
            {
                ret = (node.Attributes["text"].Value);
            }
        }
        return ret;
    }
    public static string ShowText(string text)
    {
        string ret = null;
        Regex regex = new Regex(@".*?\[([^)]*)\].*");
        MatchCollection matches = regex.Matches(text);
        if (matches.Count == 0)
        {
            return ret;
        }
        if (matches[0].Groups.Count < 2)
        {
            return ret;
        }
        ret = $"{matches[0].Groups[1]}";
        string[] arr = ret.Split(",");
        if (arr.Length < 3)
        {
            return ret;
        }
        if (arr.Length == 3)
        {
            ret = ShowText(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
        }
        else
        {
            ret = $"<color=red>[Error! Wrong format!]</color>";
        }
        return ret;
    }
}