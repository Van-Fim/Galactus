using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPanelManager : MonoBehaviour
{
    public static ClientPanelManager singleton;
    public static List<ClientPanel> panels = new List<ClientPanel>();
    public static List<ClientPanel> historyPanels = new List<ClientPanel>();
    static ClientPanel currPanel;

    public void Update()
    {

    }
    public static T GetPanel<T>() where T : ClientPanel
    {
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i] is T p)
            {
                return p;
            }
        }
        return null;
    }

    public static void Show<T>() where T : ClientPanel
    {
        T panel = ClientPanelManager.GetPanel<T>();
        Show(panel);
    }

    public static void Show(ClientPanel panel)
    {
        if (currPanel != null && currPanel != panel)
        {

        }
        if (historyPanels.Contains(panel))
        {
            historyPanels.Remove(panel);
        }
        historyPanels.Add(panel);
        currPanel = panel;

        panel.Open();
        panel.SetIndex(historyPanels.Count);
    }

    public static void Close<T>() where T : ClientPanel
    {
        T panel = ClientPanelManager.GetPanel<T>();
        panel.Close();
    }

    public static void Init()
    {
        singleton = new GameObject().AddComponent<ClientPanelManager>();
        DontDestroyOnLoad(singleton.gameObject);

        ClientPanelManager.InitByPrefab(GamePrefabsManager.singleton.LoadPrefab<MainMenuPanel>());
        ClientPanelManager.InitByPrefab(GamePrefabsManager.singleton.LoadPrefab<HudClientPanel>());
        ClientPanelManager.InitByPrefab(GamePrefabsManager.singleton.LoadPrefab<MapClientPanel>());
        ClientPanelManager.InitByPrefab(GamePrefabsManager.singleton.LoadPrefab<CharactersClientPanel>());
        ClientPanelManager.InitByPrefab(GamePrefabsManager.singleton.LoadPrefab<NewCharacterClientPanel>());
        ClientPanelManager.InitByPrefab(GamePrefabsManager.singleton.LoadPrefab<DebugClientPanel>());
        ClientPanelManager.InitByPrefab(GamePrefabsManager.singleton.LoadPrefab<MultiplayerPanel>());
        singleton.gameObject.name = "ClientPanelManager";
        DontDestroyOnLoad(singleton.gameObject);
    }
    public static ClientPanel InitByPrefab(ClientPanel prefab)
    {
        ClientPanel panel = Instantiate(prefab, CanvasManager.canvas.transform);
        panel.gameObject.name = panel.GetType().ToString();
        panel.Init();
        panel.Close();
        panels.Add(panel);
        return panel;
    }
    public static void UpdateLangText()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            ClientPanel pn = panels[i];
            pn.UpdateText();
        }
    }
}