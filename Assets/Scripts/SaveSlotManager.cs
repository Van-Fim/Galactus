using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data;
using UnityEngine;
using UnityEngine.Networking;

public class SaveSlotManager : MonoBehaviour
{
    public HudController hud;
    private SaveSlotController slotPrefab;
    private List<SaveSlotController> saveSlotControllers = new List<SaveSlotController>();
    public static SaveSlotManager saves;
    public static SaveSlotManager loads;

    public void Awake()
    {
        slotPrefab = GamePrefabsManager.LoadPrefab<SaveSlotController>("save_slot");
    }
    public void ReloadSaves(bool lastSavedOnly = false)
    {
        if (slotPrefab == null)
        {
            slotPrefab = GamePrefabsManager.LoadPrefab<SaveSlotController>("save_slot");
        }
        for (int i = 0; i < SaveManager.saveSlots.Count; i++)
        {
            GameSaveData gsd = SaveManager.saveSlots[i];
            SaveSlotController slot = null;
            if (hud.hudData.adv_hud_type == "save")
            {
                slot = saves.saveSlotControllers.Find(x => x.id == i);
            }
            else if (hud.hudData.adv_hud_type == "load")
            {
                slot = loads.saveSlotControllers.Find(x => x.id == i);
            }
            string imgName = $"";
            if (slot == null)
            {
                slot = Instantiate(slotPrefab, transform);
                slot.id = i;
                if (hud.hudData.adv_hud_type == "save")
                {
                    slot.title.text = $"Save {i + 1}";
                    slot.saveBtn.onClick.AddListener(delegate { MenuActions.SaveGame(slot.id); });
                }
                else if (hud.hudData.adv_hud_type == "load")
                {
                    slot.title.text = $"Load {i + 1}";
                    slot.saveText.text = "Load";
                    slot.saveBtn.onClick.AddListener(delegate { MenuActions.LoadGame(slot.id); });
                }
                slot.deleteText.text = $"Delete";
                saveSlotControllers.Add(slot);
            }
            if (slot != null)
            {
                slot.system.text = gsd.name;
                slot.time.text = gsd.date;
                imgName = $"/savegame{slot.id}.jpg";
                if ((lastSavedOnly && gsd.lastSave) || (!lastSavedOnly))
                {
                    SaveManager.singleton.StartCoroutine(SaveManager.singleton.LoadImageFromPersistentDataPath(slot, SaveManager.dir + "/" + imgName));
                    if (hud.hudData.adv_hud_type != "save")
                        gsd.lastSave = false;
                }
            }
        }
    }
}
