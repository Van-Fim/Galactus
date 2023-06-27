using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.UI;

public class CharactersClientPanel : ClientPanel
{
    [SerializeField] private List<CharacterButton> charactersButtonsList = new List<CharacterButton>();
    [SerializeField] private Button backButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createCharButton;
    [SerializeField] private Button deleteCharButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMPro.TMP_Text infoText;
    private CharacterButton selectedButton;

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
            CharacterButton cbtn = Instantiate(characterButtonPrefab, createCharButton.transform.parent);
            cbtn.charName = ch.login;
            cbtn.Init();

            cbtn.button.onClick.AddListener(() =>
            {
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
            ClientPanelManager.Show<NewCharacterClientPanel>();
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