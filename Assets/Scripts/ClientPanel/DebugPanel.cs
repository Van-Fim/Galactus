using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DebugPanel : ClientPanel
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    public TMPro.TMP_Text consoleText;
    public TMPro.TMP_InputField consoleInput;
    public override void Init()
    {
        backButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Show<HudClientPanel>();
        });
        closeButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<DebugPanel>();
        });
        consoleInput.onSubmit.AddListener((string text) =>
        {
            bool comRezult = DebugConsole.ProcessCommand(text);
        });
    }
    public override void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public override void Close()
    {
        gameObject.SetActive(false);
    }
}
