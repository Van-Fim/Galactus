using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager
{
    public static MainPlayerPanel mainPlayerPanel;
    public static MinimapPanel minimapPanel;

    public static void Init()
    {
        mainPlayerPanel = GameObject.Instantiate(GameContentManager.mainPlayerPanelPrefab, CanvasManager.canvas.transform);
        GameObject.DontDestroyOnLoad(mainPlayerPanel);

        minimapPanel = GameObject.Instantiate(GameContentManager.minimapPanelPrefab, CanvasManager.canvas.transform);
        GameObject.DontDestroyOnLoad(minimapPanel);
        minimapPanel.gameObject.SetActive(false);
    }
}
