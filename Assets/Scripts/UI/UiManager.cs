using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager
{
    public static MainPlayerPanel mainPlayerPanel;

    public static void Init()
    {
        mainPlayerPanel = GameObject.Instantiate(GameContentManager.mainPlayerPanelPrefab);
    }
}
