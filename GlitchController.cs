using System;
using UnityEngine;

public class GlitchController : MonoBehaviour
{
    public static GlitchController Instance;

    public Color baseColor = new(0.1f, 1f, 0.1f, 1f);
    public float minTimeBetweenGlitches = 0.1f;
    public float maxTimeBetweenGlitches = 1f;
    public float maxColorShift = 0.3f;
    public float maxAlphaShift = 0.4f;

    public event Action<Color> OnGlitch;

    private float nextGlitchTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ScheduleNextGlitch();
    }

   void Update()
    {
        if (Time.time >= nextGlitchTime)
        {
            Color glitchedColor = GenerateGlitchColor();
            Debug.Log($"[GlitchController] Triggering glitch with color: {glitchedColor}");
            OnGlitch?.Invoke(glitchedColor);
            ScheduleNextGlitch();
        }
    }
    void ScheduleNextGlitch()
{
    // Set a smaller time window between glitches for quicker, more frequent dips.
    float randomFactor = UnityEngine.Random.Range(0f, 1f);
    float exponentialRandom = -Mathf.Log(1f - randomFactor);
    nextGlitchTime = Time.time + Mathf.Lerp(minTimeBetweenGlitches * 0.2f, maxTimeBetweenGlitches * 0.4f, exponentialRandom);
}

Color GenerateGlitchColor()
{
    // Generate a more intense glitch color change.
    Color color = baseColor;

    // Increase the maximum shift for more noticeable and rapid color changes.
    color.r += UnityEngine.Random.Range(-maxColorShift * 1.5f, maxColorShift * 1.5f);
    color.g += UnityEngine.Random.Range(-maxColorShift * 1.5f, maxColorShift * 1.5f);
    color.b += UnityEngine.Random.Range(-maxColorShift * 1.5f, maxColorShift * 1.5f);
    color.a -= UnityEngine.Random.Range(0f, maxAlphaShift * 0.7f); // More sudden fade.

    // Occasionally, apply a more intense, random color shift.
    if (UnityEngine.Random.value < 0.2f)  // Increase the chance of this effect.
    {
        color = new Color(
            UnityEngine.Random.value,
            UnityEngine.Random.value,
            UnityEngine.Random.value,
            UnityEngine.Random.Range(0.3f, 0.8f) // Faster fade out
        );
    }

    return color;
}
}
