using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MinimapPanel : MainPanel
{
    public static GameObject galaxiesContainer;
    public static GameObject systemsContainer;

    public static int selectedGalaxyId = -1;
    public static int selectedSystemId = -1;

    public static int currentGalaxyId;
    public static int currentSystemId;

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
            SpaceManager.Warp(MinimapPanel.selectedGalaxyId, MinimapPanel.selectedSystemId);
            CameraManager.SwitchByCode(0);
            SpaceController.InvokeChangeLayer(1);
            UiManager.minimapPanel.gameObject.SetActive(false);
            UiManager.mainPlayerPanel.gameObject.SetActive(true);
        });
    }
}
