using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

public class CustomNotif : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite airplane;
    [SerializeField] private Sprite alert;
    [SerializeField] private Sprite warning;
    [SerializeField] private Sprite route;
    [SerializeField] private Sprite airport;

    [Header("Components")]
    [SerializeField] private TMP_Text messageUI;
    [SerializeField] private Image iconUI;
    [SerializeField] private Image panelBase;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button actionButton;

    private Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

    public float createdTime;

    public float objY;

    private bool _fadingOut = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spriteDict["airplane"] = airplane;
        spriteDict["alert"] = alert;
        spriteDict["warning"] = warning;
        spriteDict["route"] = route;
        spriteDict["airport"] = airport;

        

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    public void UpdateContents(string message, string type, string color, UnityAction onClickFunc = null)
    {
        Color newColor;

        switch (color)
        {
            case "red":
                newColor = new Color(1f, 0f, 0f, 0.5f);
                break;
            case "blue":
                newColor = new Color(0.35f, 0.6f, 1f, 0.5f);
                break;
            case "orange":
                newColor = new Color(1f, 0.62f, 0f, 0.5f);
                break;
            case "green":
                newColor = new Color(0f, 1f, 0.4f, 0.5f);
                break;
            default:
                newColor = new Color(0.35f, 0.6f, 1f, 0.8f);
                break;
        }

        panelBase.color = newColor;
        messageUI.text = message;
        iconUI.sprite = spriteDict[type];

        if (onClickFunc is not null)
        {
            actionButton.onClick.AddListener(onClickFunc);
        }
        
        actionButton.onClick.AddListener(ExpireNotification);
    }

    private void ExpireNotification()
    {
        createdTime = -100000;
    }

    private void UpdatePosition()
    {
        RectTransform rt = gameObject.GetComponent<RectTransform>();

        if (Mathf.Abs(rt.anchoredPosition.y - objY) >= 0.1 ){
            float newY = Mathf.Lerp(rt.anchoredPosition.y, objY, Time.fixedDeltaTime);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, newY);
            
        }
    }

    public void FadeOutAndDestroy(float duration = 0.25f)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        _fadingOut = true;

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }

    public void SetHidden()
    {
        //createdTime += Time.deltaTime;
        canvasGroup.alpha = 0;
    }

    public void SetShow()
    {
        if (!_fadingOut)
            canvasGroup.alpha = 1;
    }
}
