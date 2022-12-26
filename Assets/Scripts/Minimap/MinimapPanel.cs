using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MinimapPanel : MainPanel
{
    public static GameObject galaxiesContainer;
    public static GameObject systemsContainer;

    public static void Init(){
        galaxiesContainer = new GameObject();
        galaxiesContainer.transform.localPosition = Vector3.zero;
        galaxiesContainer.transform.localRotation = Quaternion.identity;
        galaxiesContainer.name = "galaxiesContainer";

        systemsContainer = new GameObject();
        systemsContainer.transform.localPosition = Vector3.zero;
        systemsContainer.transform.localRotation = Quaternion.identity;
        systemsContainer.name = "systemsContainer";
    }
}
