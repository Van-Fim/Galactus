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
    int num = 1;

    public void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                LogPage page = DebugConsole.pages.Find(f => f.name == DebugConsole.currentPage);
                if (page == null || page.history.Count == 0)
                {
                    return;
                }
                int index = page.history.Count - num;
                if (index < 0)
                {
                    num = 1;
                    index = page.history.Count - num;
                }
                else if (index > page.history.Count - 1)
                {
                    index = num = page.history.Count - 1;
                }
                consoleInput.text = page.history[index];
                num++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                LogPage page = DebugConsole.pages.Find(f => f.name == DebugConsole.currentPage);
                if (page == null || page.history.Count == 0)
                {
                    return;
                }
                int index = num-1;
                if (index > page.history.Count - 1)
                {
                    num = page.history.Count;
                    index = num-1;
                }
                if (index < 0)
                {
                    num = 1;
                    index = 0;
                }
                consoleInput.text = page.history[index];
                num--;
            }
        }
    }
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
        Controller.blocked = true;
    }
    public override void Close()
    {
        gameObject.SetActive(false);
        Controller.blocked = false;
    }
}
