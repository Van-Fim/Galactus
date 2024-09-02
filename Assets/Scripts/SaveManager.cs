using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Data;
using UnityEngine;
using UnityEngine.Networking;

public class SaveManager : MonoBehaviour
{
    static int maxSlots = 20;
    public static List<GameSaveData> saveSlots = new List<GameSaveData>();
    public static string dir;
    public static SaveManager singleton;
    public void Init()
    {
        singleton = this;
        dir = XMLF.GetDirPatch() + "/saves";
        ReloadSaves();
    }
    public void ReloadSaves()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            string fileName = $"savegame{i}.data";
            string imgName = $"savegame{i}.jpg";

            Data.GameSaveData saveData = saveSlots.Find(x => x.id == i);
            if (saveData == null)
            {
                saveData = new GameSaveData();
                saveData.id = i;
                saveSlots.Add(saveData);
            }
            saveSlots[i].name = "-";
            saveSlots[i].date = "-";
            FileStream file = null;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(dir + "/" + fileName))
            {
                file = File.Open(dir + "/" + fileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                saveSlots[i] = (Data.GameSaveData)bf.Deserialize(file);
                file.Close();
            }
        }
    }
    public IEnumerator LoadImageFromPersistentDataPath(SaveSlotController saveSlot, string filePath)
    {
        float tms = Time.timeScale;
        Time.timeScale = 1;
        if (File.Exists(filePath))
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + filePath);
            yield return www.SendWebRequest();
            GameSaveData gsd = SaveManager.saveSlots.Find(x => x.id == saveSlot.id);
            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(texture.width / 2 - 500, 0, texture.height * 1.2f, texture.height), Vector2.zero, 100);
                    saveSlot.image.sprite = sprite;
                    saveSlot.image.color = new Color32(255, 255, 255, 255);
                    saveSlot.imageText.gameObject.SetActive(false);
                }
                else
                {
                    saveSlot.image.color = new Color32(50, 50, 50, 100);
                    saveSlot.imageText.gameObject.SetActive(true);
                }
            }
        }
        Time.timeScale = tms;
    }
    void Awake()
    {
        Init();
    }
    IEnumerator TakeScreenAndSaveGame(int id)
    {
        float tms = Time.timeScale;
        Time.timeScale = 1;
        string imgName = $"/savegame{id}.jpg";
        CanvasController.singleton.gameObject.SetActive(false);
        CameraManager.mainCamera.SaveCameraView(dir + imgName);
        singleton.ReloadSaves();
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = tms;
        SaveGame(id);
    }
    public void SaveGameCoroutine(int id)
    {
        StartCoroutine(TakeScreenAndSaveGame(id));
    }
    public static void SaveGame(int id)
    {
        string fileName = $"/savegame{id}.data";
        string imgName = $"/savegame{id}.jpg";
        FileStream file = File.Create(dir + fileName);
        Data.GameSaveData saveData = saveSlots.Find(f => f.id == id);
        saveData.name = $"System {LocalClient.systemId}";
        saveData.date = $"{DateTime.Now}";
        saveData.lastSave = true;
        for (int i = 0; i < SpaceObjectManager.spaceObjects.Count; i++)
        {
            SpaceObjectData spaceObjectData = SpaceObjectManager.spaceObjects[i].GetSpaceObjectData();
            saveData.spaceObjectDatas.Add(spaceObjectData);
        }
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, saveData);
        file.Close();

        SaveSlotManager.saves.ReloadSaves(true);
        SaveSlotManager.loads.ReloadSaves(true);

        CanvasController.singleton.gameObject.SetActive(true);
    }

    public static void LoadGame(int id)
    {

    }
}
