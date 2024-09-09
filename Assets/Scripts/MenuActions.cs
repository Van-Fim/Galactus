using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActions : MonoBehaviour
{
    public static void SaveGame(int id)
    {
        SaveManager.singleton.SaveGameCoroutine(id);
    }
    public static void LoadGame(int id)
    {
        SaveManager.LoadGame(id);
    }
}
