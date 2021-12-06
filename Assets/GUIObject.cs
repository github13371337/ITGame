using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

[RequireComponent(typeof(RectTransform))]
public class GUIObject : MonoBehaviour
{
    public const string baseElementExceptionMessage = "No base element was found in children and base element field is not assigned";

    [SerializeField] protected TextAnchor anchor = TextAnchor.UpperLeft;
    [SerializeField] bool hiddenOnStart = false;

    protected Player player;
    protected Control control;
    protected Image[] images = new Image[0];
    protected Text[] texts = new Text[0];

    public bool hasImages { get; private set; }
    public RectTransform rect { get; private set; }
    public Canvas canvas { get; private set; }
    public RectTransform canvasRect { get; private set; }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        control = FindObjectOfType<Control>();
        if (hiddenOnStart) Hide();
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        images = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<Text>();
        AwakeCustom();
    }
    protected virtual void AwakeCustom() { }

    public virtual void Show() => gameObject.SetActive(true);
    public virtual void Hide() => gameObject.SetActive(false);

    public void SetColorAll(Color color) { for (int i = 0; i < images.Length; i++) images[i].color = color; }
    public void SetTextAll(string text) { for (int i = 0; i < texts.Length; i++) texts[i].text = text; }
    public void SetText(string text, int index = 0) { if (index >= 0 && index < texts.Length) texts[index].text = text; }
    public void SetTexts(string[] texts) { for (int i = 0; i < this.texts.Length && i < texts.Length; i++) this.texts[i].text = texts[i]; }

    public void SetImageAll(Sprite image) 
    { 
        for (int i = 0; i < images.Length; i++) images[i].sprite = image;
        CheckImages();
    }
    public void SetImage(Sprite image, int index = 0) 
    { 
        if (index >= 0 && index < images.Length) images[index].sprite = image;
        CheckImages();
    }
    public void SetImages(Sprite[] images) 
    { 
        for (int i = 0; i < this.images.Length && i < images.Length; i++) this.images[i].sprite = images[i];
        CheckImages();
    }

    public void SetImageEnabledAll(bool enabled) { for (int i = 0; i < images.Length; i++) images[i].enabled = enabled; }
    public void SetTextEnabledAll(bool enabled) { for (int i = 0; i < texts.Length; i++) texts[i].enabled = enabled; }

    private void CheckImages()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].sprite != null)
            {
                hasImages = true;
                return;
            }
        }
        hasImages = false;
    }

    protected bool IsWithinWindow(Vector3 mouseCoordinates, TextAnchor windowAnchor = TextAnchor.UpperLeft)
    {
        Vector2 windowCenter = new Vector2(rect.position.x, rect.position.y);
        Vector2 size = new Vector2(rect.sizeDelta.x * rect.localScale.x * canvasRect.localScale.x, rect.sizeDelta.y * rect.localScale.y * canvasRect.localScale.y);
        switch (windowAnchor)
        {
            case TextAnchor.LowerCenter: return window(new Vector2(windowCenter.x - (size.x / 2f), windowCenter.y));
            case TextAnchor.LowerLeft: return window(windowCenter);
            case TextAnchor.LowerRight: return window(new Vector2(windowCenter.x - size.x, windowCenter.y));
            case TextAnchor.MiddleCenter: return window(new Vector2(windowCenter.x - (size.x / 2), windowCenter.y - (size.y / 2)));
            case TextAnchor.MiddleLeft: return window(new Vector2(windowCenter.x, windowCenter.y - (size.y / 2)));
            case TextAnchor.MiddleRight: return window(new Vector2(windowCenter.x - size.x, windowCenter.y - (size.y / 2)));
            case TextAnchor.UpperCenter: return window(new Vector2(windowCenter.x - (size.x / 2), windowCenter.y - size.y));
            case TextAnchor.UpperLeft: return window(new Vector2(windowCenter.x, windowCenter.y - size.y));
            case TextAnchor.UpperRight: return window(new Vector2(windowCenter.x - size.y, windowCenter.y - size.y));
            default: throw new System.Exception("how did this even happen");
        }
        bool window(Vector2 center)
        {
            return (mouseCoordinates.x >= center.x &&
                    mouseCoordinates.x <= center.x + size.x &&
                    mouseCoordinates.y >= center.y &&
                    mouseCoordinates.y <= center.y + size.y);
        }
    }
}