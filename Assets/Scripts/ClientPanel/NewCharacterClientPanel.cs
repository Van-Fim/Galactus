using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterClientPanel : ClientPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMPro.TMP_InputField loginInput;
    [SerializeField] private TMPro.TMP_Text label;
    [SerializeField] private TMPro.TMP_Dropdown gameStartDropdown;
    [SerializeField] private TMPro.TMP_Text stateField;
    [SerializeField] private string gameStart;
    public void SetState(string state)
    {
        stateField.text = state;
    }
    public void SetText(string text)
    {
        loginInput.text = text;
    }
    public override void UpdateText()
    {
        TMPro.TMP_Text txt = okButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 4, 1);

        txt = cancelButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 4, 2);
        loginInput.placeholder.GetComponent<TMPro.TMP_Text>().text = LangSystem.ShowText(1000, 4, 3);

        label.text = LangSystem.ShowText(1000, 4, 6);
    }
    public void GetGameStarts()
    {
        ServerData serverData = ServerDataManager.singleton.serverData;
        gameStartDropdown.options.Clear();
        for (int i = 0; i < serverData.gameStarts.Count; i++)
        {
            GameStartData gm = serverData.gameStarts[i];
            gameStartDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData() { text = LangSystem.ShowText(gm.name) });
        }
        if (serverData.gameStarts.Count > 0)
        {
            gameStart = serverData.gameStarts[0].templateName;
        }
        gameStartDropdown.onValueChanged.AddListener((int num) =>
        {
            ServerData serverData = ServerDataManager.singleton.serverData;
            gameStart = serverData.gameStarts[num].templateName;
        });
    }
    public override void Open()
    {
        base.Open();
        GetGameStarts();
        UpdateText();
    }
    public override void Close()
    {
        base.Close();
    }

    public override void Init()
    {
        base.Init();
        backButton.onClick.AddListener(() =>
        {
            Back();
        });
        closeButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<NewCharacterClientPanel>();
        });
        okButton.onClick.AddListener(() =>
        {
            NetClient cl = NetClient.singleton;
            cl.CheckLogin(loginInput.text, gameStart);
        });
        cancelButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<NewCharacterClientPanel>();
        });
    }
    public override void Back()
    {
        base.Back();
    }
}