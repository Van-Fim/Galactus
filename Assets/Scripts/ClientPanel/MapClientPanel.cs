using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapClientPanel : ClientPanel
{
    public static byte currentLayer;
    public static byte minLayer = 0;

    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button warpButton;

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
    public override void UpdateText()
    {
        TMPro.TMP_Text txt = warpButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 2, 1);

        layerDropdown.options[0].text = LangSystem.ShowText(1000, 2, 2);
        layerDropdown.options[1].text = LangSystem.ShowText(1000, 2, 3);
        layerDropdown.options[2].text = LangSystem.ShowText(1000, 2, 4);
        layerDropdown.options[3].text = LangSystem.ShowText(1000, 2, 5);
    }
    public void ChangeLayer(byte layer)
    {
        currentLayer = layer;
        SpaceUiObj.InvokeDestroyAll();
        if (layer == 0)
        {
            Galaxy.InvokeRender();
            CameraManager.UpdateCamBoost(CameraManager.minimapCamera, 3.5f);
        }
        else if (layer == 1)
        {
            Galaxy currentGalaxy = MapSpaceManager.singleton.GetGalaxyByID(MapSpaceManager.selectedGalaxyId);
            GalaxyBuilder.Build(MapSpaceManager.singleton, currentGalaxy);
            StarSystem.InvokeRender();
            CameraManager.UpdateCamBoost(CameraManager.minimapCamera, 3.5f);
        }
        else if (layer == 2)
        {
            Galaxy currentGalaxy = MapSpaceManager.singleton.GetGalaxyByID(MapSpaceManager.selectedGalaxyId);
            GalaxyBuilder.Build(MapSpaceManager.singleton, currentGalaxy);
            StarSystem currentSystem = MapSpaceManager.singleton.GetSystemByID(MapSpaceManager.selectedGalaxyId, MapSpaceManager.selectedSystemId);
            Sector.InvokeRender();
            PlanetsBuilder.Build(MapSpaceManager.singleton, currentSystem);
            Planet.InvokeRender();
            Sun.InvokeRender();
            CameraManager.UpdateCamBoost(CameraManager.minimapCamera, 2.0f);
        }
        else if (layer == 3)
        {
            CameraManager.UpdateCamBoost(CameraManager.minimapCamera, -2.5f);
        }
        CameraManager.minimapCamera.SetPositionByLayer(layer);
        GameContent.Space.InvokeDrawUi();
    }
    public override void Open()
    {
        CameraManager.SwitchByCode(1);
        MapSpaceManager.singleton.LoadGalaxies();
        ChangeLayer(0);

        if (MapSpaceManager.anotherGalaxySelected)
        {
            GameContent.Space space = MapSpaceManager.singleton.GetGalaxyByID(MapSpaceManager.selectedGalaxyId);
            MapSpaceManager.InvokeAnotherSpaceSelected(space);
        }
        base.Open();
    }
    public override void Close()
    {
        base.Close();
        CameraManager.SwitchByCode(0);
    }
    public override void Init()
    {
        backButton.onClick.AddListener(() =>
        {
            Back();
        });
        closeButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<MapClientPanel>();
        });
        warpButton.onClick.AddListener(() =>
        {
            Data.WarpData warpData = new Data.WarpData();
            warpData.galaxyId = MapSpaceManager.selectedGalaxyId;
            warpData.systemId = MapSpaceManager.selectedSystemId;
            warpData.sectorId = MapSpaceManager.selectedSectorId;
            warpData.zoneId = MapSpaceManager.selectedZoneId;
            NetClient.singleton.WarpClient(warpData);
        });

        list.Clear();
        list.Add(new MapLayer(0, LangSystem.ShowText(1000, 2, 2)));
        list.Add(new MapLayer(1, LangSystem.ShowText(1000, 2, 3)));
        list.Add(new MapLayer(2, LangSystem.ShowText(1000, 2, 4)));
        list.Add(new MapLayer(3, LangSystem.ShowText(1000, 2, 5)));
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
        currentLayer = minLayer;
        UpdateText();
    }
    public override void Back()
    {
        base.Back();
    }
}