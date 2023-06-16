using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCharacterClientPanel : ClientPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMPro.TMP_InputField loginInput;
    [SerializeField] private TMPro.TMP_Text stateField;
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
    }
    public override void Open()
    {
        base.Open();
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