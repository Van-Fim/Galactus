using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
    public HudController hud;
    private SaveSlotController slotPrefab;
    private List<SaveSlotController> saveSlotControllers = new List<SaveSlotController>();
    void Start()
    {
        slotPrefab = GamePrefabsManager.LoadPrefab<SaveSlotController>("save_slot");
        for (int i = 0; i < SaveManager.saveSlots.Count; i++)
        {
            GameSaveData gsd = SaveManager.saveSlots[i];
            SaveSlotController slot = Instantiate(slotPrefab, transform);
            slot.id = i;
            slot.title.text = $"Save {i + 1}";
            slot.system.text = gsd.name;
            slot.time.text = gsd.date;
            saveSlotControllers.Add(slot);
            slot.deleteText.text = $"Delete";
            if (hud.hudData.adv_hud_type == "save")
            {
                slot.saveBtn.onClick.AddListener(delegate{MenuActions.SaveGame(slot.id);});
            }
            else if (hud.hudData.adv_hud_type == "load")
            {
                slot.saveText.text = $"Load";
                slot.saveBtn.onClick.AddListener(delegate{MenuActions.LoadGame(slot.id);});
            }
        }
    }
}
