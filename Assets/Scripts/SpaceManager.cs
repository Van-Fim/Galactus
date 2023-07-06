using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceManager : MonoBehaviour
{
    public static SpaceManager singleton;
    public GameObject spaceContainer;
    public GameObject solarContainer;

    public static void Init()
    {
        singleton = new GameObject().AddComponent<SpaceManager>();

        singleton.spaceContainer = new GameObject();
        singleton.spaceContainer.name = "SpaceContainer";
        singleton.spaceContainer.transform.position = Vector3.zero;
        singleton.spaceContainer.transform.rotation = Quaternion.identity;
        DontDestroyOnLoad(singleton.spaceContainer);

        singleton.solarContainer = new GameObject();
        singleton.solarContainer.name = "SolarContainer";
        singleton.solarContainer.transform.position = Vector3.zero;
        singleton.solarContainer.transform.rotation = Quaternion.identity;
        DontDestroyOnLoad(singleton.solarContainer);

        singleton.gameObject.name = "SpaceManager";
        DontDestroyOnLoad(singleton.gameObject);
    }
}
