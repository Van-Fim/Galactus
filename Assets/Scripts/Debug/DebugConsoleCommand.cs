using Mirror;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
using System.Linq;

public class DebugConsoleCommand
{
    public string command;
    public string description;
    public static string[] args;
    public Action action;
}
