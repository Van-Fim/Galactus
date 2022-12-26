using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
public class XMLF
{
    public static float FloatVal(string text)
    {
        text = text.Replace('.', ',');
        return float.Parse(text);
    }
}