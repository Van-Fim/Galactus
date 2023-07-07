using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogClientPanel : ClientPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private TMPro.TMP_Text messageField;
    public void SetState(string state)
    {
        messageField.text = state;
    }
    public override void UpdateText()
    {
        TMPro.TMP_Text txt = yesButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 5, 1);

        txt = noButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 5, 2);

        messageField.text = LangSystem.ShowText(1000, 5, 3);
    }
    public override void Open()
    {
        base.Open();
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
            ClientPanelManager.Close<DialogClientPanel>();
        });
        yesButton.onClick.AddListener(() =>
        {
            NetClient cl = NetClient.singleton;
            CharactersClientPanel cpl = ClientPanelManager.GetPanel<CharactersClientPanel>();
            if (cpl.selectedButton == null)
            {
                return;
            }
            cl.DeleteCharacter(cpl.selectedButton.characterData.login);
            cpl.selectedButton = null;
            ClientPanelManager.Close<DialogClientPanel>();
        });
        noButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<DialogClientPanel>();
        });
    }
    public override void Back()
    {
        base.Back();
    }
}