using Mirror;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

[System.Serializable]
public class LogPage
{
    public string name = "main";
    public List<String> list = new List<string>();
    public List<String> history = new List<string>();
    public int maxLinesCount = 100;
    public int historyLinesCount = 20;
    public string logFilename;

    public LogPage() { }
    public LogPage(string name = "main", int maxLinesCount = 100, int historyLinesCount = 20)
    {
        this.name = name;
        this.maxLinesCount = maxLinesCount;
        this.historyLinesCount = historyLinesCount;
        LogPage fPage = DebugConsole.pages.Find(f => f.name == name);
        if (fPage == null)
        {
            logFilename = $"{name}_log.log";
            DebugConsole.pages.Add(this);
        }
        else
        {
            DebugConsole.Log($"Page {name} already exists", "error");
        }
    }
    public void ClearLogFile()
    {
        string logDir = Application.persistentDataPath + "/" + DebugConsole.logdir;
        if (Directory.Exists(logDir))
        {
            File.Delete(logDir + "/" + logFilename);
        }
    }
}