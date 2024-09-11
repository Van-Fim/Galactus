using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiTextController : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] private TMPro.TMP_Text text;
    public static UnityAction OnUpdateAction;
    void Start()
    {
        OnUpdateAction += OnUpdate;
    }
    public static void InvokeUpdate()
    {
        OnUpdateAction?.Invoke();
    }
    void OnUpdate()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (label == "player.galaxy.id")
        {
            text.text = $"{LocalClient.galaxyId}";
        }
        else if (label == "player.system.id")
        {
            text.text = $"{LocalClient.systemId}";
        }
        else if (label == "player.sector.id")
        {
            text.text = $"{LocalClient.sectorId}";
        }
        else if (label == "player.speed")
        {
            text.text = $"{SOController.currentSpeed}";
        }
        else if (label == "player.target.distance.text")
        {
            if (SOController.distanceToTarget <= 0)
            {
                text.enabled = false;
            }
            else
            {
                text.enabled = true;
            }
            text.text = $"Distance:";
        }
        else if (label == "player.target.distance")
        {
            if (SOController.distanceToTarget <= 0)
            {
                text.enabled = false;
            }
            else
            {
                text.enabled = true;
            }
            text.text = $"{SOController.distanceToTarget.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)} km";
        }
        else
        {
            text.text = $"{label}";
        }
    }
    public void SetLabel(string text)
    {
        this.label = text;
    }
    public void SetFontSize(int size)
    {
        this.text.fontSize = size;
    }
    public void SetColor(string text)
    {
        string[] exp1 = text.Split(' ');
        this.text.color = new Color32(byte.Parse(exp1[0]), byte.Parse(exp1[1]), byte.Parse(exp1[2]), byte.Parse(exp1[3]));
    }
    public void AlignText(string text)
    {
        if (text == "left")
        {
            this.text.alignment = TextAlignmentOptions.Left;
        }
        else if (text == "right")
        {
            this.text.alignment = TextAlignmentOptions.Right;
        }
        else if (text == "topleft")
        {
            this.text.alignment = TextAlignmentOptions.TopLeft;
        }
        else if (text == "topright")
        {
            this.text.alignment = TextAlignmentOptions.TopRight;
        }
        else if (text == "bottomleft")
        {
            this.text.alignment = TextAlignmentOptions.BottomLeft;
        }
        else if (text == "bottomright")
        {
            this.text.alignment = TextAlignmentOptions.BottomRight;
        }
        else if (text == "center")
        {
            this.text.alignment = TextAlignmentOptions.Center;
        }
        else if (text == "midline")
        {
            this.text.alignment = TextAlignmentOptions.Midline;
        }
    }
    public void SetText(string text)
    {
        this.text.text = text;
    }
}
