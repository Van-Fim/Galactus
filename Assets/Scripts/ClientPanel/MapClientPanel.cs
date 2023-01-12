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

    public static byte currentLayer;
    public static byte minLayer = 0;

    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private TMPro.TMP_Dropdown layerDropdown;

    public static List<MapLayer> list = new List<MapLayer>();

    public class MapLayer
    {
        public MapLayer(int layer, string name)
        {
            this.layer = layer;
            this.name = name;
        }
        public int layer;
        public string name;
    }

    public override void Show()
    {
        base.Show();
        CameraManager.SwitchByCode(1);
        Controller.blocked = true;
        SpaceController.InvokeChangeLayer(minLayer);
    }
    public override void Hide()
    {
        base.Hide();
        CameraManager.SwitchByCode(0);
        Controller.blocked = false;
    }

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
        list.Clear();
        list.Add(new MapLayer(0, "Galaxies"));
        list.Add(new MapLayer(1, "Systems"));
        list.Add(new MapLayer(2, "Sectors"));
        list.Add(new MapLayer(3, "Zones"));
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].layer < minLayer)
            {
                list[i].layer = -1;
            }
        }
        layerDropdown.options.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].layer >= 0)
            {
                layerDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = list[i].name });
            }
        }
    }
}