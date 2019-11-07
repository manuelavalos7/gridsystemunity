using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementSystemObject : MonoBehaviour
{
    gridHandler gridManager;
    movementSystem movementScript;

    public Vector3Int posOnMatrix = Vector3Int.zero;
    public int player_moves = 4 ;
    public bool move_done = false;

    public bool selected = false;
    public bool released_after_selected = false;


    // Start is called before the first frame update
    void Start()
    {
        movementScript = GameObject.Find("movementSystem").GetComponent<movementSystem>();
        gridManager = GameObject.Find("gridManager").GetComponent<gridHandler>();
        transform.position = gridManager.matrixPosToWorld(posOnMatrix);
    }

    // Update is called once per frame
    void Update()
    {
        if (!move_done)
        {
            if (selected && !Input.GetMouseButton(0)) {
                released_after_selected = true;
            }

            if (selected && released_after_selected)
            {
                movementScript.selectMove(this);
            }
            if (Input.GetMouseButton(0) && gridManager.tilemapGridToMatrixPos(gridManager.mousePositionToTileMapPosition(Input.mousePosition)) == posOnMatrix)
            {
                if (movementScript.last_object != null && movementScript.last_object != this)
                {
                    movementScript.last_object.selected = false;
                    movementScript.last_object.released_after_selected = false;
                    gridManager.clearPaths();
                }
                selected = true;
                
            }

        }
    }
}
