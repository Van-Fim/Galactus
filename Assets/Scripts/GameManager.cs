using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Data;
using System.Xml;

namespace GameContent
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager singleton;
        private string seed = "mygameSeed";
        public bool isGameInitialized;
        public static bool isInMainMenu = true;
        public ConfigData configData = new ConfigData();
        void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
            }
        }
        void Start()
        {
            if (!singleton.isGameInitialized)
            {
                Init();
            }
        }
        void Init()
        {
            GamePrefabsManager.Init();
            GamePrefabsManager.singleton.InitContent();

            configData = LoadConfigData();
            if (true)
            {
                LangSystem.language = configData.language;
            }

            CanvasManager.Init();
            CameraManager.Init();
            ClientInputManager.Init();
            ClientPanelManager.Init();
            ClientPanelManager.Show<MainMenuPanel>();

            DebugConsole.Init();

            isGameInitialized = true;
            DontDestroyOnLoad(this);
        }
        public static ConfigData LoadConfigData()
        {
            ConfigData ret = new ConfigData();
            XmlDocument doc = XMLF.ReadFile("main.config.xml");
            XmlElement xRoot = doc.DocumentElement;
            XmlNodeList nodes = xRoot.SelectNodes("//" + "param");
            if (nodes == null)
                nodes = xRoot.SelectNodes("/" + "param");
            if (nodes == null)
                return ret;
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode nodeItem = nodes[i];
                for (int a = 0; a < nodeItem.Attributes.Count; a++)
                {
                    XmlAttribute xmlAttribute = nodeItem.Attributes[a];
                    if (xmlAttribute.Name == "login")
                    {
                        ret.login = xmlAttribute.Value;
                    }
                    else if (xmlAttribute.Name == "language")
                    {
                        ret.language = xmlAttribute.Value;
                    }
                    else
                    {
                        ConfigData.XMLAttribute attr = new ConfigData.XMLAttribute();
                        attr.name = xmlAttribute.Name;
                        attr.value = xmlAttribute.Value;
                        ret.atrParams.Add(attr);
                    }
                }
            }
            xRoot = doc.DocumentElement;
            nodes = xRoot.SelectNodes("//" + "command");
            if (nodes == null)
                nodes = xRoot.SelectNodes("/" + "command");
            if (nodes == null)
                return ret;

            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode nodeItem = nodes[i];
                for (int a = 0; a < nodeItem.Attributes.Count; a++)
                {
                    XmlAttribute xmlAttribute = nodeItem.Attributes[a];
                    if (xmlAttribute.Name == "name")
                    {
                        ret.commands.Add(xmlAttribute.Value);
                    }
                }
            }
            return ret;
        }
        public void StartGame()
        {
            SpaceManager.Init();
        }
        public void RunConfigCommands()
        {
            for (int i = 0; i < configData.commands.Count; i++)
            {
                string command = configData.commands[i];
                DebugConsole.ProcessCommand(command, true);
            }
        }
        public static int GetSeed()
        {
            return singleton.seed.GetHashCode();
        }
    }
}