using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{

        private TMP_Text m_TextComponent;

     
    // Start is called before the first frame update
    void Start()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change the text on the text component.
        m_TextComponent.text = $"{GridScript.score}";
    }
}
