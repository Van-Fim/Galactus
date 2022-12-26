using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayerPanel : MonoBehaviour
{
    public Button map;

    public void Start()
    {
        map.onClick.AddListener(() =>
        {
            if (CameraManager.currentCameraCode == 0)
            {
                CameraManager.SwitchByCode(1);
            }
            else if (CameraManager.currentCameraCode == 0)
            {
                CameraManager.SwitchByCode(0);
            }
        });
    }
}
