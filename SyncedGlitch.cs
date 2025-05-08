using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SyncedGlitch : MonoBehaviour
{
    private TMP_Text[] tmpTexts;
    private SpriteRenderer[] sprites;
    private Image[] images;

    void Awake()
    {
        tmpTexts = GetComponentsInChildren<TMP_Text>(true);
        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        images = GetComponentsInChildren<Image>(true);
    }

    void OnEnable()
{
    StartCoroutine(WaitForGlitchController());
}

System.Collections.IEnumerator WaitForGlitchController()
{
    while (GlitchController.Instance == null)
    {
        yield return null; // wait one frame
    }

    GlitchController.Instance.OnGlitch += ApplyGlitch;
    Debug.Log($"[SyncedGlitch] Subscribed to GlitchController on {gameObject.name}");
}

    void OnDisable()
    {
        if (GlitchController.Instance != null)
        {
            GlitchController.Instance.OnGlitch -= ApplyGlitch;
        }
    }

    void ApplyGlitch(Color glitchedColor)
{
    Debug.Log($"[SyncedGlitch] Applying glitch color: {glitchedColor} to {gameObject.name}");

    foreach (var text in tmpTexts)
        text.color = glitchedColor;

    foreach (var sprite in sprites)
        sprite.color = glitchedColor;

    foreach (var image in images)
        image.color = glitchedColor;
}
}
