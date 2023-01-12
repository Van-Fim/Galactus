using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPanelManager : MonoBehaviour
{
    public static List<ClientPanel> panels = new List<ClientPanel>();
    static ClientPanel currentClientBody;
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

    public static void Show<T>(bool headerToo = true) where T : ClientPanel
    {
        if (currentClientBody != null)
        {
            currentClientBody.Hide();
        }
        T panel = ClientPanelManager.GetPanel<T>();
        currentClientBody = panel;
        currentClientBody.Show();
    }

    public static void Hide<T>(bool headerToo = true) where T : ClientPanel
    {
        if (currentClientBody != null)
        {
            currentClientBody.Hide();
        }
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