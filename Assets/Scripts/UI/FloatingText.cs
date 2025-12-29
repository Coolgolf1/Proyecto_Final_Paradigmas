using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("Float")]
    public float floatAmplitude = 10f; 
    public float floatSpeed = 1f;

    [Header("Scale")]
    public float scaleAmplitude = 0.05f; 
    public float scaleSpeed = 1f;

    private RectTransform rect;
    private Vector3 startPos;
    private Vector3 startScale;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        startPos = rect.anchoredPosition;
        startScale = rect.localScale;
    }

    void Update()
    {
        float t = Time.unscaledTime;

        // Float
        float yOffset = Mathf.Sin(t * floatSpeed) * floatAmplitude;
        rect.anchoredPosition = startPos + new Vector3(0, yOffset, 0);

        // Scale pulse
        float scaleOffset = 1 + Mathf.Sin(t * scaleSpeed) * scaleAmplitude;
        rect.localScale = startScale * scaleOffset;
    }
}
