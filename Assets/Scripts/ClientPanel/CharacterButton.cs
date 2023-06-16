using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public string charName;
    private bool selected;
    public Color32 selectedColor = new Color32(0, 255, 0, 255);
    public Color32 defaultColor = new Color32(255, 255, 255, 255);
    public Button button;

    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
        }
    }

    public void Init()
    {
        button = GetComponent<Button>();
        UpdateText();
    }
    public void UpdateText()
    {
        TMPro.TMP_Text txt = button.GetComponentInChildren<TMPro.TMP_Text>();
        txt.text = charName;
        if (Selected)
        {
            txt.color = selectedColor;
        }
        else
        {
            txt.color = defaultColor;
        }
    }
    public void Destroy()
    {
        charName = null;
        selected = false;
        DestroyImmediate(button);
        DestroyImmediate(this.gameObject);
    }
}