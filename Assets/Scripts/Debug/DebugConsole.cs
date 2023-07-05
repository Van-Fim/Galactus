using Mirror;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using UnityEngine;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using GameContent;
using Data;

public class DebugConsole
{
    public static List<DebugConsoleCommand> commands = new List<DebugConsoleCommand>();
    public static List<LogPage> pages = new List<LogPage>();
    public static string currentPage = "main";
    public static bool collapse = false;
    public static bool showFileLineInfo = false;
    public static bool saveLoadData = false;
    public static bool clearFileLogs = false;
    public static string logdir;
    static int historyNum;
    public static string ReplaceRich(string text)
    {
        Regex rich = new Regex(@"<[^>]*>");
        string ret = text;

        if (rich.IsMatch(text))
        {
            ret = rich.Replace(text, string.Empty);
        }
        return ret;
    }
    public static void AddConsoleCommands()
    {
        AddConsoleCommand("StartHost", delegate ()
        {
            //NetManager.singleton.StartHost();
            DebugClientPanel panel = ClientPanelManager.GetPanel<DebugClientPanel>();
            panel.Close();
            string textString = "";
            textString += $"<color=green>|||||||||||||</color>\n";
            //textString += $"<color=green>Local ip:</color> {ClientAuthenticator.GetLocalIPAddress()}\n";
            textString += $"<color=green>Server state:</color> OK\n";
            textString += $"<color=green>|||||||||||||</color>\n\n";
            DebugConsole.Log(textString, null, "server");
        });
        AddConsoleCommand("StartServer", delegate ()
        {
            //NetManager.singleton.StartServer();
            DebugClientPanel panel = ClientPanelManager.GetPanel<DebugClientPanel>();
            panel.Close();
            string textString = "";
            textString += $"<color=green>|||||||||||||</color>\n";
            //textString += $"<color=green>Local ip:</color> {ClientAuthenticator.GetLocalIPAddress()}\n";
            textString += $"<color=green>Server state:</color> OK\n";
            textString += $"<color=green>|||||||||||||</color>\n\n";
            DebugConsole.Log(textString, null, "server");
        });
        AddConsoleCommand("StartClient", delegate ()
        {
            //NetManager.singleton.StartClient();
            DebugClientPanel panel = ClientPanelManager.GetPanel<DebugClientPanel>();
            panel.Close();
            string textString = "";
            textString += $"<color=green>|||||||||||||</color>\n";
            //textString += $"<color=green>Local ip:</color> {ClientAuthenticator.GetLocalIPAddress()}\n";
            textString += $"<color=green>Server state:</color> OK\n";
            textString += $"<color=green>|||||||||||||</color>\n\n";
            DebugConsole.Log(textString, null, "server");
        });
        AddConsoleCommand("ClearPage", delegate ()
        {
            LogPage page = pages.Find(f => f.name == DebugConsoleCommand.args[0]);
            DebugConsole.ShowErrorIsNull(page, $"Page {currentPage} not found");
            page.list = new List<string>();
        });
        AddConsoleCommand("SetAccountResource", delegate ()
        {
            if (DebugConsoleCommand.args.Length > 2)
            {
                string login = DebugConsoleCommand.args[0];
                string resName = DebugConsoleCommand.args[1];
                string sval = DebugConsoleCommand.args[2];
                float value = XMLF.FloatVal(sval);
                NetClient.singleton.SetResourceValue(login, resName, "0", value);
                NetClient.singleton.UpdateCharacters();
            }
        });
        AddConsoleCommand("SetClientResource", delegate ()
        {
            if (DebugConsoleCommand.args.Length > 2)
            {
                string login = DebugConsoleCommand.args[0];
                string resName = DebugConsoleCommand.args[1];
                string sval = DebugConsoleCommand.args[2];
                float value = XMLF.FloatVal(sval);
                NetClient.singleton.SetResourceValue(login, resName, "0", value);
                NetClient.singleton.UpdateCharacters();
            }
        });
        AddConsoleCommand("AddAccountResource", delegate ()
        {
            if (DebugConsoleCommand.args.Length > 2)
            {
                string login = DebugConsoleCommand.args[0];
                string resName = DebugConsoleCommand.args[1];
                string sval = DebugConsoleCommand.args[2];
                float value = XMLF.FloatVal(sval);
                NetClient.singleton.AddResourceValue(login, resName, "0", value);
                NetClient.singleton.UpdateCharacters();
            }
        });
        AddConsoleCommand("AddClientResource", delegate ()
        {
            if (DebugConsoleCommand.args.Length > 2)
            {
                string login = DebugConsoleCommand.args[0];
                string resName = DebugConsoleCommand.args[1];
                string sval = DebugConsoleCommand.args[2];
                float value = XMLF.FloatVal(sval);
                NetClient.singleton.AddResourceValue(login, resName, "0", value);
                NetClient.singleton.UpdateCharacters();
            }
        });
        AddConsoleCommand("SetPageMaxLines", delegate ()
        {
            if (DebugConsoleCommand.args.Length > 1)
            {
                string pageName = DebugConsoleCommand.args[0];
                LogPage pg = pages.Find(f => f.name == pageName);
                if (pg == null)
                {
                    DebugConsole.Log($"Page {pageName} not found", "error");
                    return;
                }
                pg.maxLinesCount = int.Parse(DebugConsoleCommand.args[1]);
            }
        });
        AddConsoleCommand("CreatePage", delegate ()
        {
            string page = "main";
            int mlc = 200;
            int hlc = 30;
            if (DebugConsoleCommand.args.Length > 0)
            {
                page = DebugConsoleCommand.args[0];
            }
            if (DebugConsoleCommand.args.Length > 1)
            {
                mlc = int.Parse(DebugConsoleCommand.args[1]);
            }
            if (DebugConsoleCommand.args.Length > 2)
            {
                hlc = int.Parse(DebugConsoleCommand.args[2]);
            }
            new LogPage(page, mlc, hlc);
        });
        AddConsoleCommand("SetPageSettings", delegate ()
        {
            if (DebugConsoleCommand.args.Length > 0)
            {
                collapse = bool.Parse(DebugConsoleCommand.args[0]);
            }
            if (DebugConsoleCommand.args.Length > 1)
            {
                showFileLineInfo = bool.Parse(DebugConsoleCommand.args[1]);
            }
            if (DebugConsoleCommand.args.Length > 2)
            {
                saveLoadData = bool.Parse(DebugConsoleCommand.args[2]);
            }
            if (DebugConsoleCommand.args.Length > 3)
            {
                clearFileLogs = bool.Parse(DebugConsoleCommand.args[3]);
            }
        });
        AddConsoleCommand("ShowPage", delegate ()
        {
            string page = "main";
            if (DebugConsoleCommand.args.Length > 0)
            {
                page = DebugConsoleCommand.args[0];
            }
            currentPage = page;
            ShowPage(page);
        });
        AddConsoleCommand("SaveAccountData", delegate ()
        {
            DebugConsole.Log($"OK\n", "success");
        });
        AddConsoleCommand("ShowClientsList", delegate ()
        {
        });
    }
    public static void ShowErrorIsNull(System.Object checkobject, string message, bool stop = false)
    {
        if (checkobject == null)
        {
            DebugConsole.Log(message, "error");
        }
    }
    public static LogPage GetLogPage(string page)
    {
        LogPage curPage = pages.Find(f => f.name == page);
        DebugConsole.ShowErrorIsNull(curPage, $"Page {page} not found");
        return curPage;
    }
    public static void AddConsoleCommand(string command, Action action)
    {
        DebugConsoleCommand consoleCommand = new DebugConsoleCommand();
        consoleCommand.command = command;
        consoleCommand.action = action;
        commands.Add(consoleCommand);
    }
    public static void Init()
    {
        AddConsoleCommands();
        ConfigData cfg = GameManager.singleton.configData;
        string rCfgVal = cfg.GetParamValue("log_dir");
        if (rCfgVal.Length > 0)
        {
            logdir = rCfgVal;
        }
        new LogPage("main", 200, 30);
        new LogPage("server", 200, 30);
    }
    public static bool ProcessCommand(string inputValue, bool autoAddPrefix = false, string prefix = "")
    {
        string strValue = inputValue;
        if (autoAddPrefix)
        {
            strValue = prefix + inputValue;
        }
        if (strValue.Length == 0)
        {
            return false;
        }
        DebugClientPanel panel = ClientPanelManager.GetPanel<DebugClientPanel>();
        string[] inputSplit = strValue.Split(' ');
        string[] args = inputSplit.Skip(1).ToArray();

        string commandStr = inputSplit[0];
        panel.consoleInput.text = "";
        if (!commandStr.StartsWith(prefix)) { return false; }
        commandStr = commandStr.Remove(0, prefix.Length);
        DebugConsoleCommand command = commands.Find(f => f.command == commandStr);
        if (command == null)
        {
            Log($"Command {commandStr} not found", "error", currentPage);
            return false;
        }

        LogPage curPage = pages.Find(f => f.name == currentPage);
        DebugConsole.ShowErrorIsNull(curPage, $"Page {currentPage} not found");
        if (collapse)
        {
            curPage.list = new List<string>();
        }
        Log(strValue, null, curPage.name);
        curPage.history.Add(strValue);
        curPage = CheckLines(curPage);
        curPage = CheckHistory(curPage);
        DebugConsoleCommand.args = args;
        command.action();
        ShowPage(currentPage);
        if (!collapse)
        {
            bool shw = showFileLineInfo;
            showFileLineInfo = false;
            showFileLineInfo = shw;
        }
        return true;
    }
    public static LogPage CheckLines(LogPage page)
    {
        while (page.list.Count > page.maxLinesCount)
        {
            page.list.RemoveAt(0);
        }
        return page;
    }
    public static LogPage CheckHistory(LogPage page)
    {
        while (page.history.Count > page.historyLinesCount)
        {
            page.history.RemoveAt(0);
        }
        return page;
    }
    public static void ClearAllLogFiles()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            LogPage pg = pages[i];
            pg.ClearLogFile();
        }
    }
    public static void Log(System.Object message, string type = null, string page = null, [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string callerFilePatch = null)
    {
        string text = $"null";
        if (message != null)
        {
            text = $"{message.ToString()}";
        }
        if (showFileLineInfo)
        {
            text = $"[{callerFilePatch}] {lineNumber}::\n{text}";
        }

        if (page == null)
        {
            page = currentPage;
        }
        if (page == null)
        {
            return;
        }
        if (type == "error")
        {
            text = $"<color=red>{text}</color>";
        }
        else if (type == "success")
        {
            text = $"<color=green>{text}</color>";
        }
        LogPage curPage = pages.Find(f => f.name == page);
        if (curPage == null)
        {
            if (page == "main")
            {
                new LogPage("main", 200, 30);
                Debug.Log($"Main page not found and recreated!!!");
            }
            else
            {
                Debug.Log($"Page {page} not found");
            }
            return;
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

        while (curPage.list.Count > curPage.maxLinesCount)
        {
            curPage.list.RemoveAt(0);
        }
        string logDir = Application.persistentDataPath + "/" + logdir;
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }
        File.AppendAllText(Application.persistentDataPath + "/" + logdir + "/" + curPage.logFilename, $"{ReplaceRich(text)}\n");
        ShowPage(currentPage);
    }
    public static void ShowPage(string page)
    {
        DebugClientPanel panel = ClientPanelManager.GetPanel<DebugClientPanel>();
        panel.consoleText.text = "";
        LogPage curPage = pages.Find(f => f.name == page);
        DebugConsole.ShowErrorIsNull(curPage, $"Page {page} not found");
        for (int i = 0; i < curPage.list.Count; i++)
        {
            panel.consoleText.text += curPage.list[i] + "\n";
        }
    }
    public static void PrevCommand()
    {
        LogPage page = DebugConsole.pages.Find(f => f.name == DebugConsole.currentPage);
        DebugClientPanel panel = ClientPanelManager.GetPanel<DebugClientPanel>();
        if (page == null || page.history.Count == 0)
        {
            return;
        }
        int index = page.history.Count - historyNum;
        if (index < 0)
        {
            historyNum = 1;
            index = page.history.Count - historyNum;
        }
        else if (index > page.history.Count - 1)
        {
            index = historyNum = page.history.Count - 1;
        }
        panel.consoleInput.text = page.history[index];
        historyNum++;
    }
    public static void NextCommand()
    {
        LogPage page = DebugConsole.pages.Find(f => f.name == DebugConsole.currentPage);
        DebugClientPanel panel = ClientPanelManager.GetPanel<DebugClientPanel>();
        if (page == null || page.history.Count == 0)
        {
            return;
        }
        int index = historyNum - 1;
        if (index > page.history.Count - 1)
        {
            historyNum = page.history.Count;
            index = historyNum - 1;
        }
        if (index < 0)
        {
            historyNum = 1;
            index = 0;
        }
        panel.consoleInput.text = page.history[index];
        historyNum--;
    }
}
