using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using GameContent.GameManager;

public class MultiplayerPanel : ClientPanel
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;

    private int startType = -1;
    public override void UpdateText()
    {
        TMPro.TMP_Text txt = startHostButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 2, 1);
        txt = startServerButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 2, 2);
        txt = startClientButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 2, 3);
        txt = startGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(900, 2, 4);
    }
    public void SelectButton(Button button)
    {
        TMPro.TMP_Text txt = startHostButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.color = new Color(1, 1, 1);
        txt = startServerButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.color = new Color(1, 1, 1);
        txt = startClientButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.color = new Color(1, 1, 1);
        txt = startGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.color = new Color(1, 1, 1);

        txt = button.GetComponentInChildren<TMPro.TMP_Text>();
        txt.color = new Color(0, 1, 0);
    }
    public override void Init()
    {
        backButton.onClick.AddListener(() =>
        {
            if (GameManager.isInMainMenu)
            {
                Close();
                ClientPanelManager.GetPanel<MainMenuPanel>().Open();
            }
            else
            {
                Back();
            }
        });
        closeButton.onClick.AddListener(() =>
        {
            Close();
        });
        if (GameManager.isInMainMenu)
        {
            closeButton.gameObject.SetActive(false);
        }
        startHostButton.onClick.AddListener(() =>
        {
            SelectButton(startHostButton);
            startType = 0;
        });
        startServerButton.onClick.AddListener(() =>
        {
            SelectButton(startServerButton);
            startType = 1;
        });
        startClientButton.onClick.AddListener(() =>
        {
            SelectButton(startClientButton);
            startType = 2;
        });
        startGameButton.onClick.AddListener(() =>
        {
            if (startType == 0)
            {
                NetSystem.singleton.StartHost();
            } else if (startType == 1)
            {
                NetSystem.singleton.StartServer();
            } else if (startType == 2)
            {
                NetSystem.singleton.StartClient();
            }
            Close();
            ClientPanelManager.Show<CharactersClientPanel>();
        });
        UpdateText();
    }
}
