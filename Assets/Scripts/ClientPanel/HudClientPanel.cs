using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HudClientPanel : ClientPanel
{
    [SerializeField] private Button mapButton;
    [SerializeField] private RawImage rawImage;
    public override void UpdateText()
    {
        TMPro.TMP_Text txt = mapButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000,1,1);
    }
    public override void Init()
    {
        mapButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Show<MapClientPanel>();
        });
        UpdateText();
    }
}
