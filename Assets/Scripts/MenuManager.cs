using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;
[System.Serializable]
public class MenuManagerEvent : UnityEvent<List<string>, List<List<string>>>
{
    public List<string> actions;
    public List<List<string>> args;
}
public class MenuManager : MonoBehaviour
{
    public static MenuManager singleton;
    public static List<Hud> huds = new List<Hud>();
    public static List<HudData> hudDataList = new List<HudData>();
    public static MenuManagerEvent OnSendActionAction;
    void Awake()
    {
        singleton = this;
        OnSendActionAction = new MenuManagerEvent();
        OnSendActionAction.AddListener(OnSendAction);
    }
    void Update()
    {
        UiTextController.InvokeUpdate();
    }
    public static void OnSendAction(List<string> actions, List<List<string>> args)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i] == "swich_camera" && args[i].Count > 0)
            {
                CameraManager.SwitchCamera(args[i][0]);
            }
            else if (actions[i] == "show_menu" && args[i].Count > 0)
            {
                Hud hud = MenuManager.GetHud(args[i][0]);
                hud.ShowSingle();
            }
            else if (actions[i] == "block_player_control" && args[i].Count > 0)
            {
                SOController.blocked = bool.Parse(args[i][0]);
            }
            else if (actions[i] == "warp_player")
            {
                Warp();
            }
            else if (actions[i] == "swich_item" && args[i].Count > 2)
            {
                Hud hud = MenuManager.GetHud(args[i][0]);

                if (hud == null)
                {
                    Debug.LogError($"Hud {args[i][0]} not found");
                    return;
                }
                hud.SwichHudDataFromGroup(args[i][1], args[i][2]);
            }
        }
    }
    public void ButtonAction(Button btn, string action)
    {
        List<string> commands = new List<string>();
        List<List<string>> allargs = new List<List<string>>();
        string[] exp0 = action.Split(';');
        for (int e = 0; e < exp0.Length; e++)
        {
            string[] exp = exp0[e].Split(' ');
            List<string> args = new List<string>();
            for (int j = 0; j < exp.Length; j++)
            {
                string vstr = exp[j].Trim();

                if (j == 0)
                {
                    commands.Add(vstr);
                    continue;
                }
                args.Add(vstr);
            }
            allargs.Add(args);
        }


        if (btn != null)
        {
            HudData hudData = hudDataList.Find(x => x.GMObject.gameObject == btn.gameObject);
            Image img0 = hudData.GMObject.GetComponent<Image>();

            if (hudData != null && hudData.group != null)
            {
                List<HudData> hds = hudDataList.FindAll(x => x.group == hudData.group);
                for (int i = 0; i < hds.Count; i++)
                {
                    if (hds[i].GMObject == null)
                    {
                        continue;
                    }
                    Image img = hds[i].GMObject.GetComponent<Image>();
                    if (img == null)
                    {
                        continue;
                    }
                    img.color = hds[i].bgColor1;
                }
            }
            if (img0 != null && hudData.bgColor2isActive)
            {
                img0.color = hudData.bgColor2;
            }
        }

        OnSendActionAction.Invoke(commands, allargs);
    }
    public void BuildAll()
    {
        List<Template> templates = TemplateManager.FindTemplates("menu");
        for (int i = 0; i < templates.Count; i++)
        {
            Template template = templates[i];
            BuildHudData(template);
        }
        BuildHuds();
    }
    public void BuildHuds()
    {
        Hud hud = null;
        for (int i = 0; i < hudDataList.Count; i++)
        {
            HudData hudData = hudDataList[i];
            if (hudData.name != null && hudData.type == "main")
            {
                hudData.GMObject.name = $"{hudData.name}_{hudData.id}";
                hud = hudData.GMObject.AddComponent<Hud>();
                hud.hudName = hudData.name;
                hud.isHideInds = hudData.isHideInds;
                hud.freezeTime = hudData.freezeTime;
                huds.Add(hud);
            }
            hudData.GMObject.gameObject.SetActive(hudData.isActive);
            if (hudData.swichGroup != null)
            {
                Hud.AddHudSwichGroupData(hud.hudName, hudData);
            }
            if (hudData.customPos)
            {
                RectTransform rt = hudData.GMObject.GetComponent<RectTransform>();
                rt.localPosition = rt.localPosition + hudData.position;
            }
        }
    }
    public void BuildHudData(Template template)
    {
        int depth = 1;
        List<TemplateNode> tempNodes = template.TemplateNodes.FindAll(x => x.Depth == depth);
        while (tempNodes.Count > 0)
        {
            for (int i = 0; i < tempNodes.Count; i++)
            {
                if (tempNodes[i].Node == "params")
                {
                    continue;
                }
                string itname = tempNodes[i].GetValue("item", false);

                HudData hudData = new HudData();
                hudData.item = itname;
                hudData.type = tempNodes[i].Node;
                hudData.id = tempNodes[i].Id;
                hudData.depth = depth;

                for (int t = 0; t < tempNodes[i].TemplateItems.Count; t++)
                {
                    TemplateItem templateItem = tempNodes[i].TemplateItems[t];
                    if (templateItem.Value != null)
                    {
                        ParamData paramData = new ParamData();
                        paramData.name = templateItem.ValueName;
                        paramData.value = templateItem.Value;
                        hudData.paramsData.Add(paramData);
                        if (templateItem.ValueName == "bg_color" && templateItem.Value != null)
                        {
                            string[] exp1 = templateItem.Value.Split(' ');
                            hudData.bgColor1 = new Color32(byte.Parse(exp1[0]), byte.Parse(exp1[1]), byte.Parse(exp1[2]), byte.Parse(exp1[3]));
                        }
                        if (templateItem.ValueName == "selected_bg_color" && templateItem.Value != null)
                        {
                            string[] exp1 = templateItem.Value.Split(' ');
                            hudData.bgColor2 = new Color32(byte.Parse(exp1[0]), byte.Parse(exp1[1]), byte.Parse(exp1[2]), byte.Parse(exp1[3]));
                            hudData.bgColor2isActive = true;
                        }
                    }
                }

                string stringName = tempNodes[i].GetValue("name", false);
                if (stringName != null)
                {
                    hudData.name = stringName;
                }

                string stringIcon = tempNodes[i].GetValue("icon", false);
                if (stringIcon != null)
                {
                    ParamData paramData = new ParamData();
                    paramData.name = "icon";
                    paramData.value = stringIcon;
                    hudData.paramsData.Add(paramData);
                }

                stringIcon = tempNodes[i].GetValue("btn_icon", false);
                if (stringIcon != null)
                {
                    ParamData paramData = new ParamData();
                    paramData.name = "btn_icon";
                    paramData.value = stringIcon;
                    hudData.paramsData.Add(paramData);
                }

                string action = tempNodes[i].GetValue("action", false);
                if (action != null)
                {
                    ParamData paramData = new ParamData();
                    paramData.name = "action";
                    paramData.value = action;
                    hudData.paramsData.Add(paramData);
                }
                TemplateNode paramsNode = tempNodes[i].GetChildNode("params");
                if (paramsNode != null)
                {
                    for (int j = 0; j < paramsNode.TemplateItems.Count; j++)
                    {
                        TemplateItem ti = paramsNode.TemplateItems[j];
                        ParamData paramData = new ParamData();
                        paramData.name = ti.ValueName;
                        paramData.value = ti.Value;

                        hudData.paramsData.Add(paramData);
                    }
                }
                if (tempNodes[i].ParentNode != null)
                {
                    HudData phd = hudDataList.Find(x => x.id == tempNodes[i].ParentNode.Id && x.name == tempNodes[i].ParentNode.GetValue("name", false));
                    if (phd != null)
                    {
                        phd.childList.Add(hudData);
                        hudData.parent = phd;
                        hudData.parentHudName = phd.parentHudName;

                        if (tempNodes[i].ParentNode.Node == "main" && phd.depth == 1)
                        {
                            hudData.parentHudName = tempNodes[i].ParentNode.GetValue("name", false);
                        }
                    }
                }

                hudDataList.Add(hudData);
            }
            depth++;
            tempNodes = template.TemplateNodes.FindAll(x => x.Depth == depth);
        }
        depth = 1;
        List<HudData> hd = hudDataList.FindAll(x => x.depth == depth);
        while (hd.Count > 0)
        {
            for (int i = 0; i < hudDataList.Count; i++)
            {
                if (!hudDataList[i].GMObject)
                {
                    Transform parentTransform = MenuManager.singleton.transform;
                    if (hudDataList[i].parent != null)
                    {
                        parentTransform = hudDataList[i].parent.GMObject.transform;
                    }
                    Transform obj = null;
                    if (hudDataList[i].item != null)
                    {
                        obj = GamePrefabsManager.LoadPrefab<Transform>(hudDataList[i].item);
                        hudDataList[i].GMObject = GameObject.Instantiate(obj, parentTransform).AddComponent<HudController>();
                        hudDataList[i].GMObject.hudData = hudDataList[i];
                    }
                    else
                    {
                        hudDataList[i].GMObject = new GameObject().AddComponent<HudController>();
                        hudDataList[i].GMObject.hudData = hudDataList[i];
                        obj = hudDataList[i].GMObject.transform;
                        obj.SetParent(parentTransform);
                        hudDataList[i].GMObject.AddComponent<RectTransform>();
                    }
                    if (hudDataList[i].name == null)
                    {
                        hudDataList[i].GMObject.name = $"{hudDataList[i].type}_{hudDataList[i].id}";
                    }
                    else
                    {
                        hudDataList[i].GMObject.name = hudDataList[i].name;
                    }

                    hudDataList[i].isActive = false;

                    Image image = hudDataList[i].GMObject.GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = hudDataList[i].bgColor1;
                    }
                    Transform gmSh = hudDataList[i].GMObject.transform.Find("ICON");
                    Image btnImg = null;
                    if (gmSh != null)
                    {
                        btnImg = gmSh.GetComponent<Image>();
                    }
                    string alignSelf = "center";
                    for (int y = 0; y < hudDataList[i].paramsData.Count; y++)
                    {
                        ParamData paramData = hudDataList[i].paramsData[y];
                        if (paramData.name == "align-self")
                        {
                            alignSelf = paramData.value;
                        }
                    }
                    bool customActive = false;
                    for (int y = 0; y < hudDataList[i].paramsData.Count; y++)
                    {
                        ParamData paramData = hudDataList[i].paramsData[y];
                        RectTransform rctr = hudDataList[i].GMObject.GetComponent<RectTransform>();
                        Vector2 anchorMin = rctr.anchorMin;
                        Vector2 anchorMax = rctr.anchorMax;
                        Vector2 anchorsSB = anchorMax - anchorMin;
                        Vector2 anchorMinNew = rctr.anchorMin;
                        Vector2 anchorMaxNew = rctr.anchorMax;
                        Vector2 dirDefX = new Vector2(0, 1);
                        Vector2 dirDefY = new Vector2(0, 1);
                        if (paramData.name == "align-items")
                        {
                            if (paramData.value == "horizontal")
                            {
                                HorizontalLayoutGroup hg = hudDataList[i].GMObject.AddComponent<HorizontalLayoutGroup>();
                                hg.childAlignment = TextAnchor.MiddleCenter;
                                hg.childControlHeight = false;
                                hg.childControlWidth = false;
                                hg.childScaleHeight = true;
                                hg.childScaleWidth = true;
                                hg.childForceExpandHeight = false;
                                hg.childForceExpandWidth = false;
                            }
                            else if (paramData.value == "vertical")
                            {
                                VerticalLayoutGroup vg = hudDataList[i].GMObject.AddComponent<VerticalLayoutGroup>();
                                vg.childAlignment = TextAnchor.MiddleCenter;
                                vg.childControlHeight = false;
                                vg.childControlWidth = false;
                                vg.childScaleHeight = true;
                                vg.childScaleWidth = true;
                                vg.childForceExpandHeight = false;
                                vg.childForceExpandWidth = false;
                            }
                        }
                        if (paramData.name == "height" || paramData.name == "width")
                        {
                            string[] exp = paramData.value.Split('%');
                            if (exp.Length == 2)
                            {
                                if (paramData.name == "height")
                                {
                                    float fmin = anchorMin.y + (anchorsSB.y / 100 * ((100 - float.Parse(exp[0])) / 2));
                                    float fmax = anchorMax.y - (anchorsSB.y / 100 * ((100 - float.Parse(exp[0])) / 2));
                                    if (fmax < fmin)
                                    {
                                        float fm = fmin;
                                        fmin = fmax;
                                        fmax = fm;
                                    }
                                    else if (fmax == fmin)
                                    {
                                        fmin = 0;
                                        fmax = 1;
                                    }
                                    anchorMinNew = new Vector2(anchorMin.x, fmin);
                                    anchorMaxNew = new Vector2(anchorMax.x, fmax);
                                    rctr.anchorMin = anchorMinNew;
                                    rctr.anchorMax = anchorMaxNew;
                                    anchorMin = anchorMinNew;
                                    anchorMax = anchorMaxNew;
                                    anchorsSB = anchorMax - anchorMin;
                                }
                                else if (paramData.name == "width")
                                {
                                    float fmin = dirDefX.x + ((dirDefX.y - dirDefX.x) / 100 * ((100 - float.Parse(exp[0])) / 2));
                                    float fmax = dirDefX.y - ((dirDefX.y - dirDefX.x) / 100 * ((100 - float.Parse(exp[0])) / 2));
                                    if (alignSelf == "left")
                                    {
                                        fmin = 0;
                                        fmax = dirDefX.y - ((dirDefX.y - dirDefX.x) / 100 * (float.Parse(exp[0])));
                                    }
                                    else if (alignSelf == "right")
                                    {
                                        fmin = dirDefX.x + ((dirDefX.y - dirDefX.x) / 100 * (float.Parse(exp[0])));
                                        fmax = 1;
                                    }
                                    if (fmax < fmin)
                                    {
                                        float fm = fmin;
                                        fmin = fmax;
                                        fmax = fm;
                                    }
                                    anchorMinNew = new Vector2(fmin, anchorMin.y);
                                    anchorMaxNew = new Vector2(fmax, anchorMax.y);
                                    rctr.anchorMin = anchorMinNew;
                                    rctr.anchorMax = anchorMaxNew;
                                    anchorMin = anchorMinNew;
                                    anchorMax = anchorMaxNew;
                                    anchorsSB = anchorMax - anchorMin;
                                }
                            }
                            else
                            {
                                float v = int.Parse(paramData.value);

                                if (paramData.name == "height")
                                {
                                    rctr.sizeDelta = new Vector2(rctr.sizeDelta.x, v);
                                    if (alignSelf == "top")
                                    {
                                        rctr.anchoredPosition = new Vector2(rctr.anchoredPosition.x, -v / 2);
                                        rctr.anchorMin = new Vector2(rctr.anchorMin.x, 1);
                                        rctr.anchorMax = new Vector2(rctr.anchorMax.x, 1);
                                    }
                                    else if (alignSelf == "middlecenter")
                                    {
                                        rctr.anchoredPosition = new Vector2(rctr.anchoredPosition.x, 0);
                                        rctr.anchorMin = new Vector2(rctr.anchorMin.x, 0.5f);
                                        rctr.anchorMax = new Vector2(rctr.anchorMax.x, 0.5f);
                                    }
                                    else if (alignSelf == "topleft")
                                    {
                                        rctr.anchoredPosition = new Vector2(rctr.anchoredPosition.x, -v / 2);
                                        rctr.anchorMin = new Vector2(0, 1);
                                        rctr.anchorMax = new Vector2(0, 1);
                                    }
                                    else if (alignSelf == "topright")
                                    {
                                        rctr.anchoredPosition = new Vector2(rctr.anchoredPosition.x, -v / 2);
                                        rctr.anchorMin = new Vector2(1, 1);
                                        rctr.anchorMax = new Vector2(1, 1);
                                    }
                                    else if (alignSelf == "bottomleft")
                                    {
                                        rctr.anchoredPosition = new Vector2(rctr.anchoredPosition.x, -v / 2);
                                        rctr.anchorMin = new Vector2(0, 0);
                                        rctr.anchorMax = new Vector2(0, 0);
                                    }
                                    else if (alignSelf == "bottomright")
                                    {
                                        rctr.anchoredPosition = new Vector2(rctr.anchoredPosition.x, -v / 2);
                                        rctr.anchorMin = new Vector2(1, 0);
                                        rctr.anchorMax = new Vector2(1, 0);
                                    }
                                    else if (alignSelf == "bottom")
                                    {
                                        rctr.anchoredPosition = new Vector2(rctr.anchoredPosition.x, v / 2);
                                        rctr.anchorMin = new Vector2(rctr.anchorMin.x, 0);
                                        rctr.anchorMax = new Vector2(rctr.anchorMax.x, 0);
                                    }
                                }
                                else if (paramData.name == "width")
                                {
                                    rctr.sizeDelta = new Vector2(v, rctr.sizeDelta.y);

                                    if (alignSelf == "left")
                                    {
                                        rctr.anchoredPosition = new Vector2(v / 2, rctr.anchoredPosition.y);
                                        rctr.anchorMin = new Vector2(0, rctr.anchorMin.y);
                                        rctr.anchorMax = new Vector2(0, rctr.anchorMax.y);
                                    }
                                    else if (alignSelf == "right")
                                    {
                                        rctr.anchoredPosition = new Vector2(-v / 2, rctr.anchoredPosition.y);
                                        rctr.anchorMin = new Vector2(1, rctr.anchorMin.y);
                                        rctr.anchorMax = new Vector2(1, rctr.anchorMax.y);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (paramData.name == "btn_icon" && paramData.value != null && btnImg != null)
                            {
                                btnImg.sprite = Resources.Load<Sprite>("Icons/" + paramData.value);
                            }
                            else if (paramData.name == "data-value" && paramData.value != null && hudDataList[i].type == "label")
                            {
                                if (hudDataList[i].GMObject.TryGetComponent<UiTextController>(out UiTextController uiText))
                                {
                                    uiText.SetLabel(paramData.value);
                                }
                            }
                            else if (paramData.name == "aligment" && paramData.value != null && hudDataList[i].type == "label")
                            {
                                if (hudDataList[i].GMObject.TryGetComponent<UiTextController>(out UiTextController uiText))
                                {
                                    uiText.AlignText(paramData.value);
                                }
                            }
                            else if (paramData.name == "fontsize" && paramData.value != null && hudDataList[i].type == "label")
                            {
                                if (hudDataList[i].GMObject.TryGetComponent<UiTextController>(out UiTextController uiText))
                                {
                                    uiText.SetFontSize(int.Parse(paramData.value));
                                }
                            }
                            else if (paramData.name == "color" && paramData.value != null && hudDataList[i].type == "label")
                            {
                                if (hudDataList[i].GMObject.TryGetComponent<UiTextController>(out UiTextController uiText))
                                {
                                    uiText.SetColor(paramData.value);
                                }
                            }
                            else if (paramData.name == "action" && paramData.value != null)
                            {
                                Button btn = hudDataList[i].GMObject.GetComponent<Button>();
                                if (btn != null)
                                {
                                    List<HudData> hds = hudDataList.FindAll(x => x.group == hudDataList[i].group);
                                    if (hds.Count == 1)
                                    {
                                        hudDataList[i].bgColor2isActive = true;
                                        Image iii = hudDataList[i].GMObject.GetComponent<Image>();
                                        iii.color = hudDataList[i].bgColor2;
                                    }
                                    btn.onClick.AddListener(() => ButtonAction(btn, paramData.value));
                                }
                            }
                            else if (paramData.name == "margin" && paramData.value != null)
                            {
                                string[] exp1 = paramData.value.Split(' ');
                                rctr.offsetMin = new Vector2(float.Parse(exp1[3]), float.Parse(exp1[2]));
                                rctr.offsetMax = new Vector2(-float.Parse(exp1[1]), -float.Parse(exp1[0]));
                            }
                            else if (paramData.name == "flags" && paramData.value != null)
                            {
                                string[] exp1 = paramData.value.Split(' ');
                                for (int k = 0; k < exp1.Length; k++)
                                {
                                    if (exp1[k] == "100")
                                    {
                                        hudDataList[i].isHideInds = true;
                                    }
                                    else if (exp1[k] == "200")
                                    {
                                        hudDataList[i].freezeTime = true;
                                    }
                                }
                            }
                            else if (paramData.name == "group" && paramData.value != null)
                            {
                                hudDataList[i].group = paramData.value;
                            }
                            else if (paramData.name == "swich-group" && paramData.value != null)
                            {
                                hudDataList[i].swichGroup = paramData.value;
                            }
                            else if (paramData.name == "active" && paramData.value != null)
                            {
                                hudDataList[i].isActive = bool.Parse(paramData.value);
                                customActive = true;
                            }
                            else if (paramData.name == "position" && paramData.value != null)
                            {
                                string[] exp = paramData.value.Split(' ');
                                hudDataList[i].customPos = true;
                                hudDataList[i].position = new Vector3(float.Parse(exp[0]), float.Parse(exp[1]), float.Parse(exp[2]));
                            }
                            else if (paramData.name == "hud_adv_type" && paramData.value != null)
                            {
                                hudDataList[i].adv_hud_type = paramData.value;
                                Transform vp = hudDataList[i].GMObject.transform.Find("Viewport");
                                SaveSlotManager ct = vp.transform.Find("Content").GetComponent<SaveSlotManager>();
                                if (ct)
                                {
                                    ct.hud = hudDataList[i].GMObject;
                                    if (paramData.value == "save")
                                    {
                                        SaveSlotManager.saves = ct;
                                    }
                                    else if (paramData.value == "load")
                                    {
                                        SaveSlotManager.loads = ct;
                                    }
                                    ct.ReloadSaves();
                                }
                            }
                        }
                    }

                    if (hudDataList[i].swichGroup == null && !customActive)
                    {
                        hudDataList[i].isActive = true;
                    }
                }
            }
            depth++;
            hd = hudDataList.FindAll(x => x.depth == depth);
        }
    }
    public static Hud GetHud(string name)
    {
        Hud ret = huds.Find(x => x.hudName == name);
        return ret;
    }

    public static void Warp()
    {
        if (SpaceUiObj.selectedObj != null)
        {
            SpaceSystem spaceSystem = (SpaceSystem)SpaceUiObj.selectedObj.space;
            LocalClient.galaxyId = spaceSystem.galaxyId;
            LocalClient.systemId = spaceSystem.id;
            LocalClient.controlledObject.WarpSystem(spaceSystem, 1);
            SpaceManager.LoadSystem(LocalClient.SpaceSystem);
            LocalClient.SetSectorIndexes(Vector3.zero);
            SpaceObject.InvokeRender();
        }
    }
}
