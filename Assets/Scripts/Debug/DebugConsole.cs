using System;
using Mirror;
using System.Collections.Generic;
using System.Linq;
public class DebugConsoleCommand
{
    public string command;
    public Action action;
}
public class LogPage
{
    public string name = "main";
    public List<String> list = new List<string>();
    public int listMaxLines = 20;
}
public class DebugConsole
{
    public static List<DebugConsoleCommand> commands = new List<DebugConsoleCommand>();
    public static List<LogPage> pages = new List<LogPage>();
    public static string currentPage = "main";

    public static void Init()
    {
        DebugConsoleCommand consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "StartHost";
        consoleCommand.action = delegate ()
        {
            NetManager.singleton.StartHost();
            DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
            panel.Close();
        };
        commands.Add(consoleCommand);
        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "StartServer";
        consoleCommand.action = delegate ()
        {
            NetManager.singleton.StartServer();
            DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
            panel.Close();
        };
        commands.Add(consoleCommand);
        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "StartClient";
        consoleCommand.action = delegate ()
        {
            NetManager.singleton.StartClient();
            DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
            panel.Close();
        };
        commands.Add(consoleCommand);

        LogPage curPage = new LogPage();
        curPage.name = "main";
        pages.Add(curPage);
    }
    public static bool ProcessCommand(string inputValue, string prefix = "/")
    {
        DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
        panel.consoleInput.text = "";
        if (!inputValue.StartsWith(prefix)) { return false; }
        inputValue = inputValue.Remove(0, prefix.Length);
        DebugConsoleCommand command = commands.Find(f => f.command == inputValue);
        if (command == null)
        {
            ShowMessage($"Command {inputValue} not found", false, "error");
            return false;
        }

        command.action();
        return true;
    }
    public static void ShowMessage(Object message, bool collapse = false, string type = null, string page = null)
    {
        string text = message.ToString();
        if (page == null)
        {
            page = "main";
        }
        if (type == "error")
        {
            text = $"<color=red>{text}</color>";
        }
        LogPage curPage = pages.Find(f => f.name == page);
        if (curPage == null)
        {
            curPage = new LogPage();
            curPage.name = page;
            pages.Add(curPage);
        }
        if (collapse)
        {
            curPage.list = new List<string>();
            curPage.list.Add(text);
        }
        else
        {
            curPage.list.Add(text);
        }

        while (curPage.list.Count > curPage.listMaxLines)
        {
            curPage.list.RemoveAt(0);
        }

        DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
        panel.consoleText.text = "";
        for (int i = 0; i < curPage.list.Count; i++)
        {
            panel.consoleText.text += curPage.list[i] + "\n";
        }
    }
}
