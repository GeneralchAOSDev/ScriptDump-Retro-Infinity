using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RetroGlo : MonoBehaviour
{
    [Header("Retro Glitch Settings")]
    public Color baseColor = new Color(0.1f, 1f, 0.1f, 1f); // CRT green
    public float minTimeBetweenGlitches = 0.1f;
    public float maxTimeBetweenGlitches = 1f;
    public float maxColorShift = 0.3f;
    public float maxAlphaShift = 1f;

    private float nextGlitchTime;

    // Cached components
    private List<TMP_Text> tmpTexts = new();
    private List<SpriteRenderer> sprites = new();
    private List<Image> images = new();

    void Awake()
    {
        // Auto-detect relevant components on this object and its children
        tmpTexts.AddRange(GetComponentsInChildren<TMP_Text>(true));
        sprites.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
        images.AddRange(GetComponentsInChildren<Image>(true));
    }

    void Start()
    {
        ScheduleNextGlitch();
    }

    void Update()
    {
        if (Time.time >= nextGlitchTime)
        {
            ApplyGlitch();
            ScheduleNextGlitch();
        }
    }

    void ScheduleNextGlitch()
    {
        float randomFactor = Random.Range(0f, 1f);
        float exponentialRandom = -Mathf.Log(1f - randomFactor);
        nextGlitchTime = Time.time + Mathf.Lerp(minTimeBetweenGlitches, maxTimeBetweenGlitches, exponentialRandom);
    }

    void ApplyGlitch()
    {
        Color glitchedColor = baseColor;

        glitchedColor.r += Random.Range(-maxColorShift, maxColorShift);
        glitchedColor.g += Random.Range(-maxColorShift, maxColorShift);
        glitchedColor.b += Random.Range(-maxColorShift, maxColorShift);
        glitchedColor.a -= Random.Range(0f, maxAlphaShift);

        if (Random.value < 0.1f)
        {
            glitchedColor = new Color(
                Random.value,
                Random.value,
                Random.value,
                Random.Range(0.3f, 1f)
            );
        }

        // Apply to all detected elements
        foreach (var text in tmpTexts)
            text.color = glitchedColor;

        foreach (var sprite in sprites)
            sprite.color = glitchedColor;

        foreach (var image in images)
            image.color = glitchedColor;

        Invoke(nameof(ResetColor), Random.Range(0.05f, 0.2f));
    }

    void ResetColor()
    {
        foreach (var text in tmpTexts)
            text.color = baseColor;

        foreach (var sprite in sprites)
            sprite.color = baseColor;

        foreach (var image in images)
            image.color = baseColor;
    }
}
