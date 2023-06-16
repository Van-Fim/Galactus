using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DebugClientPanel : ClientPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    public TMPro.TMP_Text consoleText;
    public TMPro.TMP_InputField consoleInput;

    public override void UpdateText()
    {
        //TMPro.TMP_Text txt = warpButton.GetComponentInChildren<TMPro.TMP_Text>();
        //txt.text = LangSystem.ShowText(1000, 2, 1);
    }

    public override void Init()
    {
        backButton.onClick.AddListener(() =>
        {
            Back();
        });
        closeButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<DebugClientPanel>();
        });
        consoleInput.onSubmit.AddListener((string text) =>
        {
            bool comRezult = DebugConsole.ProcessCommand(text);
        });
    }
}