using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Data;
using System.Xml;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public string seed = "mygameSeed";
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
        Application.targetFrameRate = 60;
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
        DebugConsole.ProcessCommandsList(configData.commands);

        isGameInitialized = true;
        DontDestroyOnLoad(this);
    }
    public static ConfigData LoadConfigData()
    {
        ConfigData ret = new ConfigData();
        XmlDocument doc = new XmlDocument();
        if (!File.Exists("main.config.xml"))
        {
            TextAsset textAsset = (TextAsset)Resources.Load("main.config");
            if (textAsset != null)
            {
                doc.LoadXml(textAsset.text);
            }
        }
        else
        {
            doc = XMLF.ReadFile("main.config.xml");
        }
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
                if (xmlAttribute.Name == "value")
                {
                    ret.commands.Add(xmlAttribute.Value);
                }
            }
        }
        return ret;
    }
    public void PreStartGame()
    {
        CharactersClientPanel chclpn = ClientPanelManager.GetPanel<CharactersClientPanel>();
        if (chclpn.selectedButton != null && chclpn.selectedButton.Selected)
        {
            ClientPanelManager.Close<CharactersClientPanel>();
            ClientPanelManager.Show<HudClientPanel>();
            NetClient.singleton.characterData = chclpn.selectedButton.characterData;
            //ServerDataManager.singleton.LoadGameStartObjects(NetClient.singleton.netId);
            SpaceManager.Init();
            SpaceManager.singleton.LoadGalaxies();
            SpaceManager.singleton.BuildSystem(LocalClient.GetGalaxyId(), LocalClient.GetSystemId());
            LocalClient.FixSpace();
            if (!LocalClient.GetIsGameStartDataLoaded())
            {
                LocalClient.SetSectorIndexes(LocalClient.GetSector().GetIndexes(), true);
                LocalClient.SetZoneIndexes(LocalClient.GetZone().GetIndexes(), true);
            }
            WarpData warpData = new WarpData();
            warpData.galaxyId = LocalClient.GetGalaxyId();
            warpData.systemId = LocalClient.GetSystemId();
            warpData.sectorId = LocalClient.GetSectorId();
            warpData.zoneId = LocalClient.GetZoneId();
            warpData.position = LocalClient.GetPosition();
            warpData.rotation = LocalClient.GetRotation();
            warpData.SetSectorIndexes(LocalClient.GetSectorIndexes());
            warpData.SetZoneIndexes(LocalClient.GetZoneIndexes());
            NetClient.singleton.WarpClient(warpData);
            /*
            textString += $"\n<color=green>||||||||||||||||||||||||||||||||||||||</color>\n\n";
            textString += $"<color=white>Galaxy id: </color><color=green>{LocalClient.GetGalaxyId()}</color>\n";
            textString += $"<color=white>System id: </color><color=green>{LocalClient.GetSystemId()}</color>\n";
            textString += $"<color=white>Sector id: </color><color=green>{LocalClient.GetSectorId()}</color>\n";
            textString += $"<color=white>Zone id: </color><color=green>{LocalClient.GetZoneId()}</color>\n";
            textString += $"<color=white>Sector indexes: </color><color=green>{LocalClient.GetSectorIndexes()}</color>\n";
            textString += $"<color=white>Zone indexes: </color><color=green>{LocalClient.GetZoneIndexes()}</color>\n";
            textString += $"<color=white>Zones count: </color><color=green>{SpaceManager.singleton.zones.Count}</color>\n";
            textString += $"\n<color=green>||||||||||||||||||||||||||||||||||||||</color>\n\n";
            */
            ServerDataManager.singleton.LoadSpaceObjects();
            if (!LocalClient.GetIsGameStartDataLoaded())
            {
                ServerDataManager.singleton.LoadGameStartObjects(NetClient.singleton.netId);
                LocalClient.SetIsGameStartDataLoaded(true);
            }
            SpaceObject.InvokeNetStart();
            TestCube testCube = GamePrefabsManager.singleton.LoadPrefab<TestCube>("TestCube");
            testCube = Instantiate(testCube);
            testCube.Color = new Color32(0, 255, 0, 150);
            testCube.transform.SetParent(SpaceManager.singleton.spaceContainer.transform);
            testCube.transform.localPosition = LocalClient.GetSector().GetPosition();
            testCube.name = "GreenTestCube";
        }
    }
    public void StartGame()
    {
        SpaceManager.Init();
        MapSpaceManager.Init();
        MapSpaceManager.anotherGalaxySelected = true;

        WarpData warpData = new WarpData();
        warpData.galaxyId = NetClient.singleton.characterData.galaxyId;
        warpData.systemId = NetClient.singleton.characterData.systemId;
        warpData.sectorId = NetClient.singleton.characterData.sectorId;
        warpData.zoneId = NetClient.singleton.characterData.zoneId;
        warpData.position = NetClient.singleton.characterData.GetPosition();
        warpData.rotation = NetClient.singleton.characterData.GetRotation();

        NetClient.singleton.WarpClient(warpData);

        SpaceManager.singleton.LoadGalaxies();
        SpaceManager.singleton.BuildSystem(LocalClient.GetGalaxyId(), LocalClient.GetSystemId());
        NetClient.singleton.FixSpace();

        MapSpaceManager.singleton.LoadGalaxies();
        MapSpaceManager.singleton.BuildSystem(MapSpaceManager.selectedGalaxyId, MapSpaceManager.selectedSystemId);

        TestCube testCube = GamePrefabsManager.singleton.LoadPrefab<TestCube>("TestCube");
        testCube = Instantiate(testCube);
        testCube.Color = new Color32(0, 255, 0, 150);
        testCube.transform.SetParent(SpaceManager.singleton.spaceContainer.transform);
        testCube.transform.localPosition = LocalClient.GetSector().GetPosition();
        LocalClient.SetSectorIndexes(LocalClient.GetSector().GetIndexes());
        SpaceObject.InvokeRender();
        SpaceObject.InvokeNetStart();
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