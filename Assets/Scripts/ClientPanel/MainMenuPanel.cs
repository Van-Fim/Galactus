using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using GameContent;

public class MainMenuPanel : ClientPanel
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button multiplayerGameButton;
    [SerializeField] private Button saveGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsGameButton;
    [SerializeField] private Button quitGameButton;
    public override void UpdateText()
    {
        TMPro.TMP_Text txt = newGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 1, 1);
        txt = multiplayerGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 1, 2);
        txt = saveGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 1, 3);
        txt = loadGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 1, 4);
        txt = settingsGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 1, 5);
        txt = quitGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 1, 6);
    }
    public override void Init()
    {
        multiplayerGameButton.onClick.AddListener(() =>
        {
            ClientPanelManager.GetPanel<MainMenuPanel>().Close();
            ClientPanelManager.Show<MultiplayerPanel>();
        });
        if (GameManager.isInMainMenu)
        {
            saveGameButton.gameObject.SetActive(false);
        }
        UpdateText();
    }
}
