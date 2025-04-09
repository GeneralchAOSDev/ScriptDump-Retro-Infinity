using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILoadScene : MonoBehaviour
{
    [SerializeField] private AudioClip press;
    [SerializeField] private Sprite clicked;
    [SerializeField] private AudioSource audioSource;
    
    private Button button;
    private Image buttonImage;
    private Sprite defaultSprite;
    
    void Start()
    {
        // Get component references
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        
        // Store the default sprite
        defaultSprite = buttonImage.sprite;
        
        // Setup button click event
        button.onClick.AddListener(OnButtonClick);
        
        // Ensure we have an AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void OnButtonClick()
    {
        StartCoroutine(LoadDelay());
    }
    
    IEnumerator LoadDelay()
    {
        // Play sound
        if (press != null)
        {
            audioSource.clip = press;
            audioSource.Play();
        }
        
        // Change sprite
        if (clicked != null)
        {
            buttonImage.sprite = clicked;
        }
        
        // Wait and load scene
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }
}