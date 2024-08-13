using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class IND_target : MonoBehaviour, IPointerClickHandler
{
    public static IND_target selectedTarget;
    public static Color32 defColor = Color.white;
    public static Color32 selectedColor = Color.green;
    public static int defaultScale = 1;
    public Image selfImage;
    public float scaleFactor = 1;
    public SpaceObject spaceObject;
    public GameObject topLeftBorder;
    public GameObject bottomLeftBorder;
    public GameObject topRightBorder;
    public GameObject bottomRightBorder;

    public GameObject objCollider;

    Image topLeftImage;
    Image bottomLeftImage;
    Image topRightImage;
    Image bottomRightImage;
    Image colliderImage;

    public void Init()
    {
        selfImage = gameObject.GetComponent<Image>();
        topLeftImage = topLeftBorder.GetComponent<Image>();
        bottomLeftImage = bottomLeftBorder.GetComponent<Image>();
        topRightImage = topRightBorder.GetComponent<Image>();
        bottomRightImage = bottomRightBorder.GetComponent<Image>();
        colliderImage = objCollider.GetComponent<Image>();
        SetColor(defColor);
        FixObject();
    }
    public void FixObject()
    {
        float sc = defaultScale * 40;
        float sc2 = sc * 2 + defaultScale * 2;
        colliderImage.rectTransform.sizeDelta = new Vector2(sc2, sc2);
        topLeftBorder.transform.localPosition = new Vector3(-sc, sc, 0);
        bottomLeftBorder.transform.localPosition = new Vector3(-sc, -sc, 0);
        topRightBorder.transform.localPosition = new Vector3(sc, sc, 0);
        bottomRightBorder.transform.localPosition = new Vector3(sc, -sc, 0);
    }

    public void SetColor(Color32 color)
    {
        if (topLeftImage && bottomLeftImage && topRightImage && bottomRightImage)
        {
            topLeftImage.color = color;
            bottomLeftImage.color = color;
            topRightImage.color = color;
            bottomRightImage.color = color;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        IND_target.selectedTarget = this;
    }
}
