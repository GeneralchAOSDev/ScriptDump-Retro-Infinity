using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    const int MAX_LIVES = 3;
    static private int lives;
    static private DateTime[] lostTimes = new DateTime[MAX_LIVES];
    static public int getMax = MAX_LIVES;
    static public int getLives(){
        byte tempLives = 0;
        if (PlayerPrefs.GetString($"life0") == "") { // first time?
            for (int i = 0; i < MAX_LIVES; i++) {
                var time = DateTime.UtcNow.AddMinutes(-1);
                PlayerPrefs.SetString($"life{i}", time.ToString("yyyy-MM-ddTHH:mm:ss"));
                lostTimes[i] = time;
                tempLives = MAX_LIVES;
            }
        }else{
            for(int i = 0; i < MAX_LIVES;i++){
                lostTimes[i] = DateTime.Parse(PlayerPrefs.GetString($"life{i}"));
                TimeSpan timePassed = DateTime.UtcNow - lostTimes[i];
                int hoursPassed = (int)timePassed.TotalMinutes;
                if(hoursPassed > 1)
                    tempLives++;
            }
        }
        if(tempLives > MAX_LIVES)
            tempLives = MAX_LIVES;

        return tempLives;
    }

    public void DebugLives(){
        Debug.Log($"Debug Lives");
        //lives = MAX_LIVES;
        setLives(MAX_LIVES);
    }

    
    static public void setLives(int change)
    {
        int current = getLives();
        DateTime now = DateTime.UtcNow;

        int target = current + change;
        
        if (target < 0){
            // Subtract lives by marking timestamps as just lost
            int livesToRemove = Mathf.Min(-target, MAX_LIVES);
            int removed = 0;

            for (int i = 0; i < MAX_LIVES && removed < livesToRemove; i++) {
                TimeSpan timePassed = now - lostTimes[i];
                if (timePassed.TotalMinutes >= 60){
                    PlayerPrefs.SetString($"life{i}", now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    removed++;
                }
            }
        }
        else if (target > 0) {
            // Add lives by marking timestamps as long expired
            int livesToAdd = Mathf.Min(target, MAX_LIVES);
            int added = 0;

            for (int i = 0; i < MAX_LIVES && added < livesToAdd; i++) {
                TimeSpan timePassed = now - lostTimes[i];
                if (timePassed.TotalMinutes < 60){
                    PlayerPrefs.SetString($"life{i}", now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ss"));
                    added++;
                }
            }
        }

        PlayerPrefs.Save();
    }

    void Start(){
        getLives();
    }

    static public void addLife(){
        Debug.Log($"Adding to Lives {lives}");
        if(lives < MAX_LIVES)
            lives++;

        setLives(1);
        Debug.Log($"Added to Lives {lives}");
    }

    static public void substractLife(){
        Debug.Log($"Subtracting from Lives {lives}");
        
        if(lives > 0)
            lives--;

        setLives(-1);
        Debug.Log($"Subtracted from Lives {lives}");
    }
}
