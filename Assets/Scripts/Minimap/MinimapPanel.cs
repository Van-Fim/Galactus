using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MinimapPanel : MainPanel
{
    public static GameObject galaxiesContainer;
    public static GameObject systemsContainer;
    public static GameObject sectorsContainer;
    public static GameObject zonesContainer;

    public static int selectedGalaxyId = -1;
    public static int selectedSystemId = -1;
    public static int selectedSectorId = -1;
    public static int selectedZoneId = -1;

    public static int currentGalaxyId;
    public static int currentSystemId;
    public static int currentSectorId;
    public static int currentZoneId;

    public TMP_Dropdown selectLayerDropdown;
    public Button warp;

    public static byte layer = 1;

    public static void Init(){
        galaxiesContainer = new GameObject();
        galaxiesContainer.transform.localPosition = Vector3.zero;
        galaxiesContainer.transform.localRotation = Quaternion.identity;
        galaxiesContainer.name = "galaxiesContainer";

        systemsContainer = new GameObject();
        systemsContainer.transform.localPosition = Vector3.zero;
        systemsContainer.transform.localRotation = Quaternion.identity;
        systemsContainer.name = "systemsContainer";

        sectorsContainer = new GameObject();
        sectorsContainer.transform.localPosition = Vector3.zero;
        sectorsContainer.transform.localRotation = Quaternion.identity;
        sectorsContainer.name = "sectorsContainer";

        zonesContainer = new GameObject();
        zonesContainer.transform.localPosition = Vector3.zero;
        zonesContainer.transform.localRotation = Quaternion.identity;
        zonesContainer.name = "zonesContainer";
    }

    void Start()
    {
        base.Start();
        selectLayerDropdown.value = layer;
        selectLayerDropdown.onValueChanged.AddListener((int level) =>
        {
            SpaceController.InvokeChangeLayer((byte)level);
        });

        warp.onClick.AddListener(() =>
        {
            SpaceManager.singleton.Warp(MinimapPanel.selectedGalaxyId, MinimapPanel.selectedSystemId, MinimapPanel.selectedSectorId, MinimapPanel.selectedZoneId);
            CameraManager.SwitchByCode(0);
            SpaceController.InvokeChangeLayer(1);
            UiManager.minimapPanel.gameObject.SetActive(false);
            UiManager.mainPlayerPanel.gameObject.SetActive(true);
        });
    }
}
