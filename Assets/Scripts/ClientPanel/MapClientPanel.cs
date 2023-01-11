using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapClientPanel : ClientPanel
{

    [SerializeField] private Button backButton;

    public override void Init()
    {
        backButton.onClick.AddListener(() => {
            ClientPanelManager.Show<HudClientPanel>();
        });
    }
}