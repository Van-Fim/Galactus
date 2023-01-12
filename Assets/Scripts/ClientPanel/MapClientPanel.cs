using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapClientPanel : ClientPanel
{
    public static int selectedGalaxyId;
    public static int selectedSystemId;
    public static int selectedSectorId;
    public static int selectedZoneId;

    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;

    public override void Init()
    {
        backButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Show<HudClientPanel>();
        });
        closeButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Show<HudClientPanel>();
        });
    }
}