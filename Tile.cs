using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    private bool lockout = false;
    private bool beingDragged;
    //public GameObject gridS;
    private GridScript gridScript;
    private Vector3 startDrag;
    private Vector3 endDrag;
    private Vector2 gridPos;
    private Vector3 tilePos;
    private Vector3 dragDir;
    private Vector2 iconI;
    public int iconNum;
    private float dragSpeed = 14f;
    private const float DRAG_THRESHOLD = 0.5f;
    private const float MAX_DRAG_DISTANCE = 1.0f;
    private const float ANIMATION_DURATION = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        gridScript = FindObjectOfType<GridScript>();
        //if(gridScript)
            //Debug.Log($"GridScript Found! {gridScript.name}");
    }

    private void OnMouseDown()
    {
        this.gameObject.transform.localScale += new Vector3(0.5f, 0.5f, 0f);
        if(!lockout){
    
            var renderer = this.gameObject.GetComponent<SpriteRenderer>();
            if (renderer != null){
                //renderer.color = Color.red;
                renderer.sortingOrder = 1;
            }
            //UnityEngine.Debug.Log($"Clicking on {this.name}");
    
            // Parse coordinates using Split with space as delimiter
            string temp = this.name;
            string[] parts = temp.Split(' ');
    
            // Assuming format is "Tile X Y"
            if (parts.Length >= 3) {
                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);
                //UnityEngine.Debug.Log($"At grid position ({x}, {y})");
                iconI = new Vector2(x, y);
            }
    
            startDrag = GetMouseWorldPosition();
            tilePos = transform.position;
            beingDragged = true;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }

    private void OnMouseDrag()
    {
        if(!beingDragged) 
            return;

        Vector3 currentPos = GetMouseWorldPosition();
        dragDir = currentPos - startDrag;

        if (Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
        {
            dragDir.y = 0;
        }
        else
        {
            dragDir.x = 0;
        }

        // Clamp drag distance
        dragDir = Vector3.ClampMagnitude(dragDir, MAX_DRAG_DISTANCE);

        // Animate to new position
        Vector3 targetPosition = tilePos + dragDir;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * dragSpeed
        );
    }
    private void OnMouseUp()
    {
        this.gameObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0f);
        if(!lockout){
            var renderer = this.gameObject.GetComponent<SpriteRenderer>();
            if (renderer != null){
                renderer.color = Color.white;
                renderer.sortingOrder = 0;
            }
            endDrag = GetMouseWorldPosition();
            beingDragged= false;

            Vector3 dragPos = GetMouseWorldPosition() - startDrag;
        
            transform.DOMove(tilePos, ANIMATION_DURATION);

            //Debug.Log($"The Drag Direction is {dragDir}");

            // get coffee, add bounds to restrict from moving tiles out of the grid
            Vector2 destinationI = iconI;
            destinationI.y += -dragDir.y;
            destinationI.x += dragDir.x;

            if(destinationI.x >= 0 && destinationI.x <= GridScript.COLUMNS-1){
                if(destinationI.y >= 0 && destinationI.y <= GridScript.ROWS-1){
                    gridScript.swapTiles(iconI, destinationI);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(GridScript.activeFunctions > 0)
            lockout = true;
        else 
            lockout = false;
    }
}
