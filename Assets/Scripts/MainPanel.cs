using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    public Button map;

    public void Start()
    {
        map.onClick.AddListener(() =>
        {
            if (CameraManager.currentCameraCode == 0)
            {
                CameraManager.SwitchByCode(1);
                SpaceController.InvokeChangeLayer(1);
                UiManager.minimapPanel.gameObject.SetActive(true);
                UiManager.mainPlayerPanel.gameObject.SetActive(false);
            }
            else if (CameraManager.currentCameraCode == 1)
            {
                CameraManager.SwitchByCode(0);
                UiManager.minimapPanel.gameObject.SetActive(false);
                UiManager.mainPlayerPanel.gameObject.SetActive(true);
            }
        });
    }
}
