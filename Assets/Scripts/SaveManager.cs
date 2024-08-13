using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Data;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    static int maxSlots = 20;
    static List<GameSaveData> saveSlots = new List<GameSaveData>();
    void Awake()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            string fileName = $"savegame{i}.data";
            string dir = XMLF.GetDirPatch();
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
            }
            saveSlots.Add(saveData);
        }
    }

    void Update()
    {

    }
}
