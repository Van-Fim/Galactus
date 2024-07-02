using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePrefabsManager : MonoBehaviour
{
    public static GamePrefabsManager prefab;
    public static GamePrefabsManager singleton;
    public List<string> list;
    public Object[] prefabs;

    public static void Init()
    {
        prefab = Resources.Load<GamePrefabsManager>("Prefabs/GamePrefabsManager");
        singleton = Instantiate<GamePrefabsManager>(prefab);
        singleton.gameObject.name = "GamePrefabsManager";
        singleton.prefabs = Resources.LoadAll("Prefabs");
        singleton.InitContent();
    }
    public void InitContent()
    {
        list = new List<string>();
        list.Add("Content");

        for (int i = 0; i < list.Count; i++)
        {
            TemplateManager.LoadTemplates("system", list[i] + "/Systems");
            TemplateManager.LoadTemplates("galaxy", list[i] + "/Galaxies");
            TemplateManager.LoadTemplates("universe", list[i] + "/Universe");
            TemplateManager.LoadTemplates("start", list[i] + "/GameStarts");
            TemplateManager.LoadTemplates("ship", list[i] + "/Ships");
            TemplateManager.LoadTemplates("pilot", list[i] + "/Pilots");
            TemplateManager.LoadTemplates("gate", list[i] + "/Gates");
            TemplateManager.LoadTemplates("object", list[i] + "/Objects");
            TemplateManager.LoadTemplates("hardpoints", list[i] + "/Hardpoints");
            TemplateManager.LoadTemplates("hardpoint_type", list[i] + "/HardpointTypes");
        }
    }
    public T LoadPrefab<T>(string name = null)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject gm = prefabs[i] as GameObject;
            if (gm.GetComponent<T>() != null && name != null && gm.gameObject.name == name)
            {
                return gm.GetComponent<T>();
            }
            else if (gm.GetComponent<T>() != null && name == null)
            {
                return gm.GetComponent<T>();
            }
        }
        return default(T);
    }
}
