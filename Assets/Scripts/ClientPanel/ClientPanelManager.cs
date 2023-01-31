using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPanelManager : MonoBehaviour
{
    public static ClientPanelManager singleton;
    public static List<ClientPanel> panels = new List<ClientPanel>();

    public void Update(){
        if (panels.Count > 0 && Input.GetKeyDown("`"))
        {
            ClientPanelManager.Show<DebugPanel>();
        }
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
        panel.Show();
    }

    public static void Close<T>() where T : ClientPanel
    {
        T panel = ClientPanelManager.GetPanel<T>();
        panel.Close();
    }

    public static void Init()
    {
        InitByPrefab(GameContentManager.hudClientPanelPrefab);
        InitByPrefab(GameContentManager.mapClientPanelPrefab);
        InitByPrefab(GameContentManager.debugPanelPrefab);
        singleton = new GameObject().AddComponent<ClientPanelManager>();
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
}