using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplay : MonoBehaviour
{
    const int MAX_LIVES = 3;
    [SerializeField] Sprite heart;
    [SerializeField] Sprite heartnt;
    [SerializeField] Image[] lifeDisplay = new Image[MAX_LIVES];
    private int lives = 0;
    static public bool refresh = false;
    // Start is called before the first frame update
    void Start()
    {
        updateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if(refresh)
            updateDisplay();
    }

    void updateDisplay() {
        lives = Lives.getLives();
        Debug.Log($"Getting Lives");
        for (int i = 0; i < Lives.getMax; i++)
        {
            Debug.Log($"Getting life {i} lives={lives}");
            if (lives > i)
                lifeDisplay[i].sprite = heart;
            else
                lifeDisplay[i].sprite = heartnt;
        }
    }
}
