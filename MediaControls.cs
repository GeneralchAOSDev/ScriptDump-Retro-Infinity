using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MediaControls : MonoBehaviour
{
    //private bool isPlaying = false;
    private bool started = false;
    private bool isShuffle = false;
    private bool isPlay = false;
    const int NUMBER_TRACKS = 12;
    private string componentName;  // Renamed variable
    [SerializeField] private AudioClip[] library = new AudioClip[0];
    private int[] trackHistory = new int[NUMBER_TRACKS];
    private int trackIndex = 0;
    private int songNumber = 0;
    AudioSource music;

    void Start()
    {
        music = GameObject.Find("Music").GetComponent<AudioSource>();
        if(music == null )
            Debug.Log("Audio Source not Found");
        componentName = this.GetComponent<MediaControls>().GetType().Name;
    }

    private void Update()
    {
        
        if(!music.isPlaying && isPlay){
            playNext();
        }
    }
    public void buttonPress(byte type){
        Debug.Log($"button pressed: {type}");
        switch(type){
            case 0:
                shuffleToggle();
                break;
            case 1:
                playPrevious();
                break;
            case 2:
                playNext();
                break;
            case 3:
                playToggle();
                break;
            default:
                Debug.Log("Error wrong Button Type");
                break;
        }
    }

    private void playToggle(){ 
    if(music.isPlaying){
        isPlay = false;
        music.Pause();
    } else if(!music.isPlaying) {
        if(!started){
            if(isShuffle){
                songNumber = Random.Range(0, NUMBER_TRACKS);
                music.clip = (library[songNumber]);
            }
            isPlay = true;  // Add this line
            music.Play();
            started = true;
        }
        else{
            isPlay = true;
            music.UnPause();
        }
    }
}
    private void playNext(){
        Debug.Log($"Track Index is: {trackIndex}");
        
            if(trackIndex >= NUMBER_TRACKS)
                trackIndex = 0;
            if(trackIndex < 0)
                trackIndex = NUMBER_TRACKS-1;
            trackHistory[trackIndex] = songNumber;
            trackIndex++;


        if(isShuffle)
            songNumber = Random.Range(0, NUMBER_TRACKS);
        else
            songNumber++;

        if(songNumber >= NUMBER_TRACKS)
            songNumber = 0;

        music.clip = (library[songNumber]);
        started = true;
        isPlay = true;
        music.Play();
    }
    private void shuffleToggle(){
        if(isShuffle){
            isShuffle = false;
        } else {
            isShuffle = true;
        }
    }
    private void playPrevious(){
        Debug.Log($"Track Index is: {trackIndex}");
        trackIndex--;
        if(trackIndex < 0)
            trackIndex = NUMBER_TRACKS-1;


        music.clip = (library[trackIndex]);

        
        isPlay = true;
        music.Play();
    }
    

    private void OnMouseDown()
    {
        Debug.Log($"DEBUG: Clicking On {componentName}");    
    }
}