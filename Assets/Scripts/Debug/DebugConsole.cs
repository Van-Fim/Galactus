using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class DebugConsoleCommand
{
    public string command;
    public int descId;
    public static string[] args;
    public Action action;
}
public class LogPage
{
    public string name = "main";
    public List<String> list = new List<string>();
    public List<String> history = new List<string>();
    public int listMaxLines = 100;
    public int historyCount = 20;
}
public class DebugConsole
{
    public static List<DebugConsoleCommand> commands = new List<DebugConsoleCommand>();
    public static List<LogPage> pages = new List<LogPage>();
    public static string currentPage = "main";
    public static bool collapse = false;

    public static void Init()
    {
        DebugConsoleCommand consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "StartHost";
        consoleCommand.action = delegate ()
        {
            NetManager.singleton.StartHost();
            DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
            panel.Close();
            string textString = "";
            textString += $"<color=green>|||||||||||||</color>\n";
            textString += $"<color=green>Local ip:</color> {Client.GetLocalIPAddress()}\n";
            textString += $"<color=green>Server state:</color> OK\n";
            textString += $"<color=green>|||||||||||||</color>\n\n";
            DebugConsole.Log(textString);
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "StartServer";
        consoleCommand.action = delegate ()
        {
            NetManager.singleton.StartServer();
            DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
            string textString = "";
            textString += $"<color=green>|||||||||||||</color>\n";
            textString += $"<color=green>Server local ip:</color> {Client.GetLocalIPAddress()}\n";
            textString += $"<color=green>Server state:</color> OK\n";
            textString += $"<color=green>|||||||||||||</color>\n\n";
            DebugConsole.Log(textString);
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "StartClient";
        consoleCommand.action = delegate ()
        {
            NetManager.singleton.StartClient();
            DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
            panel.Close();
            string textString = "";
            textString += $"<color=green>|||||||||||||</color>\n";
            textString += $"<color=green>Local ip:</color> {Client.GetLocalIPAddress()}\n";
            textString += $"<color=green>Server state:</color> OK\n";
            textString += $"<color=green>|||||||||||||</color>\n\n";
            DebugConsole.Log(textString);
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "Help";
        consoleCommand.action = delegate ()
        {
            string textString = "";
            for (int i = 0; i < commands.Count; i++)
            {
                textString += $"<color=green>Command:</color> {commands[i].command}\n";
                textString += $"<color=green>Description:</color> {i}\n";
                textString += $"<color=green>|||||||||||||</color>\n\n";
            }
            DebugConsole.Log(textString);
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "ClearCurrPage";
        consoleCommand.action = delegate ()
        {
            LogPage curPage = pages.Find(f => f.name == currentPage);
            curPage.list = new List<string>();
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "GetObjInfo";
        consoleCommand.action = delegate ()
        {
            string objName = "object";
            if (DebugConsoleCommand.args.Length > 0)
            {
                objName = DebugConsoleCommand.args[0];
            }
            string textString = "";
            GameObject go = GameObject.Find(objName);
            if (go == null)
            {
                Log($"Object {objName} not found", false, "error", currentPage);
                return;
            }
            textString += $"-----\n<color=green>name:</color> {go.name}";
            textString += $"\n<color=green>position:</color> {go.transform.localPosition}\n<color=green>rotation:</color> {go.transform.localEulerAngles}";
            textString += $"\n-----";
            Log(textString, false, null, currentPage);
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "GetLocalClientInfo";
        consoleCommand.action = delegate ()
        {
            string textString = "";
            if (Client.localClient == null)
            {
                Log($"Local client not found", false, "error", currentPage);
                return;
            }

            textString += $"<color=blue>=========</color>\n<color=green>name:</color> {Client.localClient.name}";
            textString += $"\n<color=green>position:</color> {Client.localClient.transform.localPosition}\n<color=green>rotation:</color> {Client.localClient.transform.localEulerAngles}";
            if (Client.localClient.controllTarget != null)
            {
                SPObject trgt = Client.localClient.controllTarget;
                textString += $"\n-----\n<color=green>name:</color> {trgt.name}";
                textString += $"\n<color=green>position:</color> {trgt.transform.localPosition}\n<color=green>rotation:</color> {trgt.transform.localEulerAngles}";
                textString += $"\n-----";
            }
            textString += $"\n<color=blue>=========</color>\n\n";
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "GetClientsInfo";
        consoleCommand.action = delegate ()
        {
            string textString = "";
            for (int i = 0; i < ClientManager.singleton.clientIds.Count; i++)
            {
                Client cl = NetworkClient.spawned[ClientManager.singleton.clientIds[i]].GetComponent<Client>();
                string localPlayer = "false";
                if (Client.localClient == cl)
                {
                    localPlayer = "true";
                }

                textString += $"<color=blue>=========</color>\n<color=green>Local player:</color> {localPlayer}\n<color=green>name:</color> {cl.name}";
                textString += $"\n<color=green>position:</color> {cl.transform.localPosition}\n<color=green>rotation:</color> {cl.transform.localEulerAngles}";
                if (cl.controllTarget != null)
                {
                    SPObject trgt = cl.controllTarget;
                    textString += $"\n-----\n<color=green>name:</color> {trgt.name}";
                    textString += $"\n<color=green>position:</color> {trgt.transform.localPosition}\n<color=green>rotation:</color> {trgt.transform.localEulerAngles}";
                    textString += $"\n-----";
                }
                textString += $"\n<color=blue>=========</color>\n\n";
            }
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "GetAllObjs";
        consoleCommand.action = delegate ()
        {
            Type ftype = Type.GetType("GameObject");
            if (DebugConsoleCommand.args.Length > 0)
            {
                ftype = Type.GetType(DebugConsoleCommand.args[0]);
                if (ftype == null)
                {
                    Log($"Error type {DebugConsoleCommand.args[0]} not exists", false, null, currentPage);
                    return;
                }
            }
            LogPage curPage = pages.Find(f => f.name == currentPage);
            if (curPage == null)
            {
                curPage = new LogPage();
                curPage.name = currentPage;
                pages.Add(curPage);
            }
            UnityEngine.Object[] allObjects = GameObject.FindObjectsOfType(ftype);
            string textString = "";
            foreach (UnityEngine.Object go in allObjects)
            {
                Client gm = go as Client;
                textString += $"-----\n<color=green>name:</color> {go.name}";
                if (gm != null)
                {
                    textString += $"\n<color=green>position:</color> {gm.transform.localPosition}\n<color=green>rotation:</color> {gm.transform.localEulerAngles}";
                }
                textString += $"\n-----";
            }
            Log(textString, false, null, currentPage);
        };
        commands.Add(consoleCommand);

        consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = "ShowPage";
        consoleCommand.action = delegate ()
        {
            string page = "main";
            if (DebugConsoleCommand.args.Length > 0)
            {
                page = DebugConsoleCommand.args[0];
            }
            currentPage = page;
            ShowPage(page);
        };
        commands.Add(consoleCommand);

        LogPage curPage = new LogPage();
        curPage.name = "main";
        pages.Add(curPage);
    }
    public static bool ProcessCommand(string inputValue, string prefix = "/")
    {
        string strValue = inputValue;
        if (strValue.Length == 0)
        {
            return false;
        }
        DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
        string[] inputSplit = strValue.Split(' ');
        string[] args = inputSplit.Skip(1).ToArray();

        string commandStr = inputSplit[0];
        panel.consoleInput.text = "";
        if (!commandStr.StartsWith(prefix)) { return false; }
        commandStr = commandStr.Remove(0, prefix.Length);
        DebugConsoleCommand command = commands.Find(f => f.command == commandStr);
        if (command == null)
        {
            Log($"Command {commandStr} not found", false, "error", currentPage);
            return false;
        }

        LogPage curPage = pages.Find(f => f.name == currentPage);
        if (curPage == null)
        {
            curPage = new LogPage();
            curPage.name = currentPage;
            pages.Add(curPage);
        }
        if (collapse)
        {
            curPage.list = new List<string>();
        }
        curPage.list.Add(strValue);
        curPage.history.Add(strValue);
        while (curPage.history.Count > curPage.historyCount)
        {
            curPage.history.RemoveAt(0);
        }
        DebugConsoleCommand.args = args;
        command.action();
        ShowPage(currentPage);
        return true;
    }
    public static void Log(System.Object message, bool collapse = false, string type = null, string page = null)
    {
        string text = message.ToString();
        if (page == null)
        {
            page = currentPage;
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
        ShowPage(currentPage);
    }
    public static void ShowPage(string page)
    {
        DebugPanel panel = ClientPanelManager.GetPanel<DebugPanel>();
        panel.consoleText.text = "";
        LogPage curPage = pages.Find(f => f.name == page);
        if (curPage == null)
        {
            curPage = new LogPage();
            curPage.name = page;
            pages.Add(curPage);
        }
        for (int i = 0; i < curPage.list.Count; i++)
        {
            panel.consoleText.text += curPage.list[i] + "\n";
        }
    }
}
