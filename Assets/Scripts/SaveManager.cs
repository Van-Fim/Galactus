using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Data;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static int maxSlots = 20;
    public static List<GameSaveData> saveSlots = new List<GameSaveData>();
    public static string dir;
    void Awake()
    {
        dir = XMLF.GetDirPatch() + "/saves";
        for (int i = 0; i < maxSlots; i++)
        {
            string fileName = $"savegame{i}.data";

            Data.GameSaveData saveData = new GameSaveData();
            saveData.id = i;
            saveData.name = "-";
            saveData.date = "-";
            FileStream file = null;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(dir + "/" + fileName))
            {
                file = File.Open(dir + "/" + fileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                saveData = (Data.GameSaveData)bf.Deserialize(file);
                file.Close();
            }
            saveSlots.Add(saveData);
        }
    }

    public static void SaveGame(int id)
    {
        string fileName = $"/savegame{id}.data";
        string imgName = $"/savegame{id}.jpg";
        FileStream file = File.Create(dir + fileName);
        // CanvasController.singleton.gameObject.SetActive(false);
        // ScreenCapture.CaptureScreenshot(dir + imgName);
        // CanvasController.singleton.gameObject.SetActive(true);
        Data.GameSaveData saveData = saveSlots.Find(f => f.id == id);
        saveData.name = $"System {LocalClient.systemId}";
        saveData.date = $"{DateTime.Now}";
        for (int i = 0; i < SpaceObjectManager.spaceObjects.Count; i++)
        {
            SpaceObjectData spaceObjectData = SpaceObjectManager.spaceObjects[i].GetSpaceObjectData();
            saveData.spaceObjectDatas.Add(spaceObjectData);
        }
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, saveData);
        file.Close();
    }

    public static void LoadGame(int id){

    }
}
