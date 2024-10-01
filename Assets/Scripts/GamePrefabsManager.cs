using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GamePrefabsManager : MonoBehaviour
{
    public static GamePrefabsManager singleton;
    public List<string> list;
    public static Object[] prefabs;

    public static void Init()
    {
        singleton = GameManager.singleton.AddComponent<GamePrefabsManager>();
        prefabs = Resources.LoadAll("Prefabs");
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
            TemplateManager.LoadTemplates("sun", list[i] + "/Suns");
            TemplateManager.LoadTemplates("planet", list[i] + "/Planets");
            TemplateManager.LoadTemplates("start", list[i] + "/GameStarts");
            TemplateManager.LoadTemplates("ship", list[i] + "/Ships");
            TemplateManager.LoadTemplates("pilot", list[i] + "/Pilots");
            TemplateManager.LoadTemplates("gate", list[i] + "/Gates");
            TemplateManager.LoadTemplates("spaceobject", list[i] + "/Objects");
            TemplateManager.LoadTemplates("hardpoints", list[i] + "/Hardpoints");
            TemplateManager.LoadTemplates("hardpoint_type", list[i] + "/HardpointTypes");
            TemplateManager.LoadTemplates("menu", list[i] + "/Menu/Menus");
        }
    }
    public static T LoadPrefab<T>(string name = null)
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
