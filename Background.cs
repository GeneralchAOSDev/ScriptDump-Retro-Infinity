using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private SpriteRenderer spriteRen;
    [SerializeField] const int NumOfSprites = 29;
    [SerializeField] private float bgTimer = 60f;
    [SerializeField] private Sprite[] sprites = new Sprite[NumOfSprites];
    // Start is called before the first frame update
    void Start()
    {
        spriteRen = this.GetComponent<SpriteRenderer>();
        spriteRen.sprite = sprites[Random.Range(0,NumOfSprites-1)];
        StartCoroutine(changeBG());
    }


    IEnumerator changeBG(){
        transform.DOScale(1.42f, bgTimer);
        transform.DOMoveY(-6.5f, bgTimer);
        yield return new WaitForSeconds(bgTimer);
        transform.DOScale(1.25f, 0.1f);
        transform.DOMoveY(-5.12f ,0.1f);
        spriteRen.sprite = sprites[Random.Range(0,NumOfSprites-1)];
        StartCoroutine(changeBG());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
