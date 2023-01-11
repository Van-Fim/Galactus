using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPanelManager : MonoBehaviour
{
    public static List<ClientPanel> panels = new List<ClientPanel>();
    static ClientPanel currentClientPanel;
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
        if (currentClientPanel != null)
        {
            currentClientPanel.Hide();
        }
        T panel = ClientPanelManager.GetPanel<T>();
        currentClientPanel = panel;
        currentClientPanel.Show();
    }

    public static void Init()
    {
        InitByPrefab(GameContentManager.hudClientPanelPrefab);
        InitByPrefab(GameContentManager.mapClientPanelPrefab);
    }
    public static ClientPanel InitByPrefab(ClientPanel prefab)
    {
        ClientPanel panel = Instantiate(prefab, CanvasManager.canvas.transform);
        panel.gameObject.name = panel.GetType().ToString();
        panel.Init();
        panel.Hide();
        panels.Add(panel);
        return panel;
    }
}