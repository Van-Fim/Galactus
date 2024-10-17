using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarController : MonoBehaviour
{
    public static Vector3 zoneIndexes = Vector3.zero;
    public static Vector3 currentZoneIndexes = Vector3.zero;
    public static int stepSize = 25000;
    public static Vector3 cameraPos = Vector3.zero;
    public SolarObject solarObject;
}