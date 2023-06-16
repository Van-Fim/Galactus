using System.Collections;
using System.Collections.Generic;
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
        string ret = "";
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
}