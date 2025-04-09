using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class clickButton : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    [SerializeField] private byte buttonType;
    [SerializeField] private bool hold;
    [SerializeField] private Sprite[] states = new Sprite[2];
    [SerializeField] private AudioClip press;
    [SerializeField] private AudioClip release;
    GameObject tapeDeck;
    private bool pressed = false;
    // Start is called before the first frame update
    void Start()
    {
        tapeDeck = GameObject.Find("tape deck");
        if ( tapeDeck == null )
            Debug.Log("Tape Deck Not Found");
    }

    private void OnMouseDown()
    {
        tapeDeck.GetComponent<MediaControls>().buttonPress(buttonType);
        if (pressed) {
            pressed = false;
            overlay.SetActive(false);
            this.GetComponent<SpriteRenderer>().sprite = states[0];
            AudioSource.PlayClipAtPoint(release, new Vector3(1, 1, 1));
        }else{
            if(!hold){
                StartCoroutine(noHold());
            }else{
                overlay.SetActive(true);
                AudioSource.PlayClipAtPoint(press, new Vector3(1, 1, 1));
                this.GetComponent<SpriteRenderer>().sprite = states[1];
                pressed = true;
            }
        }
    }

    IEnumerator noHold(){
        AudioSource.PlayClipAtPoint(press, new Vector3(1, 1, 1));
        this.GetComponent<SpriteRenderer>().sprite = states[1];
        pressed = true;
        yield return new WaitForSeconds(0.12f);
        this.GetComponent<SpriteRenderer>().sprite = states[0];
        pressed = false;
        AudioSource.PlayClipAtPoint(release, new Vector3(1, 1, 1));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
