using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Difficulty : MonoBehaviour
{
    [SerializeField] private AudioClip slideNoise;
    Slider slider;
    // Start is called before the first frame update
     void Start()
    {
        slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void OnSliderValueChanged(float value)
    {
        Debug.Log("Slider Value Changed: " + value);
        AudioSource.PlayClipAtPoint(slideNoise, new Vector3(1, 1, 1));
        GridScript.difficulty = (int)value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
