using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePrefabsManager : MonoBehaviour
{
    public static GamePrefabsManager prefab;
    public static GamePrefabsManager singleton;
    public List<string> list;
    public List<GameObject> prefabs;

    public static void Init(){
        prefab = Resources.Load<GamePrefabsManager>("Prefabs/GamePrefabsManagerPrefab");
        singleton = Instantiate<GamePrefabsManager>(prefab);
        singleton.gameObject.name = "GamePrefabsManager";
        DontDestroyOnLoad(singleton.gameObject);
    }
    public void InitContent()
    {
        list = new List<string>();
        list.Add("Content");

        for (int i = 0; i < list.Count; i++)
        {
            TemplateManager.LoadTemplates("planet", list[i] + "/Planets");
            TemplateManager.LoadTemplates("sun", list[i] + "/Suns");
            TemplateManager.LoadTemplates("zone", list[i] + "/Zones");
            TemplateManager.LoadTemplates("sector", list[i] + "/Sectors");
            TemplateManager.LoadTemplates("system", list[i] + "/Systems");
            TemplateManager.LoadTemplates("galaxy", list[i] + "/Galaxies");
            TemplateManager.LoadTemplates("universe", list[i] + "/Universe");

            TemplateManager.LoadTemplates("start", list[i] + "/GameStarts");
            TemplateManager.LoadTemplates("loadouts", list[i] + "/Loadouts");
            TemplateManager.LoadTemplates("hardpoints", list[i] + "/Hardpoints");
            TemplateManager.LoadTemplates("hardpoint_type", list[i] + "/HardpointTypes");
            TemplateManager.LoadTemplates("ship", list[i] + "/Ships");
            TemplateManager.LoadTemplates("asteroid", list[i] + "/Asteroids");
            TemplateManager.LoadTemplates("pilot", list[i] + "/Pilots");
            TemplateManager.LoadTemplates("object", list[i] + "/Objects");
            TemplateManager.LoadTemplates("resource_type", list[i] + "/ResourceTypes");
        }
    }
    public T LoadPrefab<T>(string name = null)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            if (prefabs[i].GetComponent<T>() != null && name != null && prefabs[i].gameObject.name == name)
            {
                return prefabs[i].GetComponent<T>();
            }
            else if (prefabs[i].GetComponent<T>() != null && name == null)
            {
                return prefabs[i].GetComponent<T>();
            }
        }
        return default(T);
    }
}
