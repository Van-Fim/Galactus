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

    public void ChangeLayer(byte layer)
    {
        currentLayer = layer;
        Vector3 addPos = Vector3.zero;
        if (layer == 0)
        {
            Galaxy sp = SpaceManager.galaxies.Find(f=>f.id == selectedGalaxyId);
            if (sp != null)
            {
                addPos = sp.GetPosition();
            }
        }
        else if (layer == 1)
        {
            StarSystem sys = SpaceManager.starSystems.Find(f=>f.galaxyId == selectedGalaxyId && f.id == selectedSystemId);
            if (sys != null)
            {
                addPos = sys.GetPosition();
            }
        }
        else if (layer == 2)
        {
            Sector sp = SpaceManager.sectors.Find(f=>f.galaxyId == selectedGalaxyId && f.systemId == selectedSystemId && f.id == selectedSectorId);
            if (sp != null)
            {
                addPos = sp.GetPosition()/Sector.minimapDivFactor;
            }
        }
        else if (layer == 3)
        {
            Zone sp = SpaceManager.zones.Find(f=>f.galaxyId == selectedGalaxyId && f.systemId == selectedSystemId && f.sectorId == selectedSectorId && f.id == selectedZoneId);
            if (sp != null)
            {
                addPos = sp.GetPosition()/Zone.minimapDivFactor;
            }
        }
        Vector3 pos = CameraController.startCamPositions[layer];
        CameraManager.minimapCamera.enabled = false;
        CameraManager.minimapCamera.transform.localPosition = pos + addPos;
        CameraManager.minimapCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
        CameraManager.minimapCamera.enabled = true;
        Space.InvokeRender();
        Space.InvokeDrawUi();
    }

    public override void Show()
    {
        base.Show();
        CameraManager.SwitchByCode(1);
        Controller.blocked = true;

        selectedGalaxyId = Client.localClient.galaxyId;
        selectedSystemId = Client.localClient.systemId;
        selectedSectorId = Client.localClient.sectorId;
        selectedZoneId = Client.localClient.zoneId;

        ChangeLayer(minLayer);
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
        layerDropdown.onValueChanged.AddListener((int layer) =>
        {
            ChangeLayer((byte)layer);
        });
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