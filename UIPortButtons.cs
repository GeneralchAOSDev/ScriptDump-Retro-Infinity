using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPortButtons : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private byte buttonType;
    [SerializeField] private bool hold;
    [SerializeField] private Sprite[] states = new Sprite[2];
    [SerializeField] private AudioClip press;
    [SerializeField] private AudioClip release;
    [SerializeField] private AudioSource audioSource;
    
    private GameObject tapeDeck;
    private Button button;
    private Image buttonImage;
    private bool pressed = false;
    
    void Start()
    {
        // Get references
        tapeDeck = GameObject.Find("Tape");
        if (tapeDeck == null)
            Debug.Log("Tape Deck Not Found");
            
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        
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
        tapeDeck.GetComponent<MediaControls>().buttonPress(buttonType);
        
        if (pressed)
        {
            pressed = false;
            overlay.SetActive(false);
            buttonImage.sprite = states[0];
            PlaySound(release);
        }
        else
        {
            if (!hold)
            {
                StartCoroutine(NoHold());
            }
            else
            {
                overlay.SetActive(true);
                PlaySound(press);
                buttonImage.sprite = states[1];
                pressed = true;
            }
        }
    }
    
    IEnumerator NoHold()
    {
        PlaySound(press);
        buttonImage.sprite = states[1];
        pressed = true;
        yield return new WaitForSeconds(0.12f);
        buttonImage.sprite = states[0];
        pressed = false;
        PlaySound(release);
    }
    
    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            // Play through the attached AudioSource instead of at a position
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}