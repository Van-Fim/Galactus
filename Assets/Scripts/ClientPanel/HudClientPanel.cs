using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudClientPanel : ClientPanel
{
    [SerializeField] private Button mapButton;

    public override void Init()
    {
        mapButton.onClick.AddListener(() => {
            ClientPanelManager.Show<MapClientPanel>();
        });
    }
    public override void Show()
    {
        base.Show();
    }
}
