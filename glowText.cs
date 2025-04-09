using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetroTextGlow : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshPro worldSpaceText;
    public TextMeshProUGUI uiText;

    [Header("Sprite References")]
    public SpriteRenderer spriteOne;
    public SpriteRenderer spriteTwo;

    [Header("Image References")]
    public GameObject img1;
    public GameObject img2;

    [Header("Retro Glitch Settings")]
    public Color baseColor = new Color(0.1f, 1f, 0.1f, 1f); // CRT green
    public float minTimeBetweenGlitches = 0.1f;
    public float maxTimeBetweenGlitches = 1f;
    public float maxColorShift = 0.3f;
    public float maxAlphaShift = 0.4f;

    private float nextGlitchTime;

    void Start()
    {
        // Initial randomization of first glitch time
        ScheduleNextGlitch();
    }

    void Update()
    {
        // Check if it's time to glitch
        if (Time.time >= nextGlitchTime)
        {
            ApplyGlitch();
            ScheduleNextGlitch();
        }
    }

    void ScheduleNextGlitch()
    {
        // Completely random timing with exponential-like distribution
        float randomFactor = Random.Range(0f, 1f);
        float exponentialRandom = -Mathf.Log(1f - randomFactor);
        nextGlitchTime = Time.time + Mathf.Lerp(minTimeBetweenGlitches, maxTimeBetweenGlitches, exponentialRandom);
    }

    void ApplyGlitch()
    {
        Color glitchedColor = baseColor;

        // Dramatic color shifts
        glitchedColor.r += Random.Range(-maxColorShift, maxColorShift);
        glitchedColor.g += Random.Range(-maxColorShift, maxColorShift);
        glitchedColor.b += Random.Range(-maxColorShift, maxColorShift);

        // Opacity variations
        glitchedColor.a -= Random.Range(0, maxAlphaShift);

        // Occasional extreme glitch
        if (Random.value < 0.1f)
        {
            // Extreme color inversion or complete distortion
            glitchedColor = new Color(
                Random.value, 
                Random.value, 
                Random.value, 
                Random.Range(0.3f, 1f)
            );
        }

        // Apply to text
        if (worldSpaceText != null)
            worldSpaceText.color = glitchedColor;
        
        if (uiText != null)
            uiText.color = glitchedColor;

        // Apply to sprites
        if (spriteOne != null)
            spriteOne.color = glitchedColor;
        
        if (spriteTwo != null)
            spriteTwo.color = glitchedColor;


        if (img1 != null)
            img1.GetComponent<Image>().color = glitchedColor;
        
        if (img2 != null)
            img2.GetComponent<Image>().color = glitchedColor;

        // Optionally reset after a brief moment
        Invoke(nameof(ResetColor), Random.Range(0.05f, 0.2f));
    }

    void ResetColor()
    {
        if (worldSpaceText != null)
            worldSpaceText.color = baseColor;
        
        if (uiText != null)
            uiText.color = baseColor;

        if (spriteOne != null)
            spriteOne.color = baseColor;
        
        if (spriteTwo != null)
            spriteTwo.color = baseColor;

        if (img1 != null)
            img1.GetComponent<Image>().color = baseColor;
        
        if (img2 != null)
            img2.GetComponent<Image>().color = baseColor;

    }
}