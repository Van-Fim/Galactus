using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
    public static Hud activeHud;
    public string hudName;
    public bool isHideInds;
    public bool freezeTime;

    public List<HudSwichGroupData> hudSwichGroupData = new List<HudSwichGroupData>();

    public static void AddHudSwichGroupData(string mainHudName, HudData hudData)
    {
        Hud hud = MenuManager.huds.Find(x => x.hudName == mainHudName);
        if (hud == null)
        {
            return;
        }
        HudSwichGroupData hd = hud.hudSwichGroupData.Find(x => x.name == hudData.swichGroup);
        if (hd == null)
        {
            hd = new HudSwichGroupData();
            hd.name = hudData.swichGroup;
            hud.hudSwichGroupData.Add(hd);
        }
        hd.list.Add(hudData);
    }
    public HudData GetHudDataFromGroup(string groupName, string hudDataName)
    {
        HudData ret = null;
        HudSwichGroupData hd = hudSwichGroupData.Find(x => x.name == groupName);
        if (hd != null)
        {
            ret = hd.list.Find(x => x.name == hudDataName);
        }
        return ret;
    }
    public void SwichHudDataFromGroup(string groupName, string hudDataName)
    {
        HudData h = GetHudDataFromGroup(groupName, hudDataName);
        
        HudSwichGroupData hd = hudSwichGroupData.Find(x => x.name == groupName);
        if (hd != null)
        {
            HudData hhd = hd.list.Find(x => x.isActive == true);
            if (hhd != h && h != null)
            {
                hhd.isActive = false;
                hhd.GMObject.gameObject.SetActive(false);

                h.isActive = true;
                h.GMObject.gameObject.SetActive(true);
            }
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        if (freezeTime)
        {
            Time.timeScale = 1;
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsFirstSibling();
        activeHud = this;
        if (freezeTime)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        UiTextController.InvokeUpdate();
    }
    public void ShowSingle()
    {
        for (int i = 0; i < MenuManager.huds.Count; i++)
        {
            Hud hud = MenuManager.huds[i];
            if (hud.hudName != hudName)
            {
                hud.Hide();
            }
            else
            {
                hud.Show();
            }
        }
    }
}
