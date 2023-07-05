using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class CharactersClientPanel : ClientPanel
{
    [SerializeField] private List<CharacterButton> charactersButtonsList = new List<CharacterButton>();
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createCharButton;
    [SerializeField] private Button deleteCharButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMPro.TMP_Text infoText;
    public CharacterButton selectedButton;
    public void UpdateAccount(ServerData serverData)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        GamePrefabsManager gmpr = GamePrefabsManager.singleton;
        ServerDataManager.singleton.serverData = serverData;
    }
    public void UpdateCharacters(ServerData serverData)
    {
        if (ServerDataManager.singleton == null)
        {
            return;
        }
        GamePrefabsManager gmpr = GamePrefabsManager.singleton;
        CharacterButton characterButtonPrefab = gmpr.LoadPrefab<CharacterButton>();
        Transform par = createCharButton.transform.parent;
        for (int i = 0; i < charactersButtonsList.Count; i++)
        {
            CharacterButton b = charactersButtonsList[i];
            if (b == null || b.gameObject == null)
            {
                continue;
            }
            b.Destroy();
        }
        for (int i = 0; i < serverData.characters.Count; i++)
        {
            CharacterData ch = serverData.characters[i];
            if (NetClient.singleton.accountData.id != ch.accountId)
            {
                continue;
            }
            GameStartData gameStartData = serverData.gameStarts.Find(f => f.templateName == ch.gameStart);
            CharacterButton cbtn = Instantiate(characterButtonPrefab, createCharButton.transform.parent);
            cbtn.characterData = ch;
            cbtn.Init();

            cbtn.button.onClick.AddListener(() =>
            {
                for (int i = 0; i < charactersButtonsList.Count; i++)
                {
                    CharacterButton b = charactersButtonsList[i];
                    if (b == null || b.gameObject == null)
                    {
                        continue;
                    }
                    if (b == cbtn)
                    {
                        List<Template> rtemps = TemplateManager.FindTemplates("resource_type");
                        infoText.text = $"{LangSystem.ShowText(1100, 1, 1)}: {ch.login}\n";
                        infoText.text += $"{LangSystem.ShowText(1100, 1, 2)}: {LangSystem.ShowText(gameStartData.name)}\n";
                        AccountData account = ServerDataManager.singleton.serverData.GetAccountById(ch.accountId);
                        for (int i2 = 0; i2 < rtemps.Count; i2++)
                        {
                            Template tm = rtemps[i2];
                            string subtype = tm.GetValue("resource_type", "subtype");
                            if (subtype != "0")
                            {
                                continue;
                            }
                            ResourcesData res = account.GetResource(tm.TemplateName, subtype);
                            string str = LangSystem.ShowText(res.name);
                            infoText.text += $"{str}: {res.value}\n";
                        }
                        for (int i2 = 0; i2 < rtemps.Count; i2++)
                        {
                            Template tm = rtemps[i2];
                            string subtype = tm.GetValue("resource_type", "subtype");
                            if (subtype != "1")
                            {
                                continue;
                            }
                            ResourcesData res = ch.GetResource(tm.TemplateName, subtype);
                            string str = LangSystem.ShowText(res.name);
                            infoText.text += $"{str}: {res.value}\n";
                        }
                        cbtn.Selected = true;
                        if (selectedButton != null)
                        {
                            selectedButton.Selected = false;
                            selectedButton.UpdateText();
                        }
                        selectedButton = cbtn;
                    }
                    else
                    {
                        cbtn.Selected = false;
                    }
                    b.UpdateText();
                }
            });
            charactersButtonsList.Add(cbtn);
        }
        createCharButton.transform.SetParent(null);
        createCharButton.transform.SetParent(par);

        deleteCharButton.transform.SetParent(null);
        deleteCharButton.transform.SetParent(par);
    }
    public override void UpdateText()
    {
        TMPro.TMP_Text txt = createCharButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 3, 1);

        txt = deleteCharButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 3, 2);

        txt = startGameButton.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = LangSystem.ShowText(1000, 3, 3);
    }

    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }

    public override void Init()
    {
        base.Init();
        backButton.onClick.AddListener(() =>
        {
            Back();
        });
        closeButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<CharactersClientPanel>();
        });
        createCharButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Show<NewCharacterClientPanel>();
        });
        deleteCharButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Show<DialogClientPanel>();
        });
        startGameButton.onClick.AddListener(() =>
        {
            ClientPanelManager.Close<CharactersClientPanel>();
            ClientPanelManager.Show<HudClientPanel>();
        });
        UpdateText();
    }
    public override void Back()
    {
        base.Back();
    }
}