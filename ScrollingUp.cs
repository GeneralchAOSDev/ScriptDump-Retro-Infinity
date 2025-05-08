using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ScrollUp : MonoBehaviour
{
    public float speed = 20f; // time to move
    public float startPos;
    Transform trans;
    public float movePos = 3600f;

    void Awake()
    {
        startPos = transform.position.y;
        trans = transform;

        moveIt();
    }

    void moveIt(){
        
        Vector3 tarPos = transform.position;
        tarPos.y = startPos - movePos;
        transform.position = tarPos;

        trans.DOMoveY(startPos, speed).SetEase(Ease.InSine);
    }
}
