using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPObjectManager : MonoBehaviour
{
    public static SPObjectManager singleton;
    public List<Ship> ships = new List<Ship>();
    public List<Pilot> pilots = new List<Pilot>();

    public static void Init(){
        singleton = new GameObject().AddComponent<SPObjectManager>();
        singleton.name = "SPObjectManager";
    }
}
