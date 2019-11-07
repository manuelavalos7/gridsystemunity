using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public gridHandler gridManager;
    public turnHandler turnManager;
    public Vector3Int playerPos;
    private bool moving = false;
    private Queue<Vector3Int> move_queue = new Queue<Vector3Int>();
    

    void Start()
    {
        turnManager = GameObject.Find("turnManager").GetComponent<turnHandler>();
        gridManager = GameObject.Find("gridManager").GetComponent<gridHandler>();
        playerPos = new Vector3Int(0,gridManager.board_height / 2, 0);
        transform.position = gridManager.matrixPosToWorld(playerPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (move_queue.Count > 0 && !moving) {
            Vector3Int next_move = move_queue.Dequeue();
            StartCoroutine(smoothMovement(playerPos, next_move));
            playerPos = next_move;
        }
        
    }

    public bool tempMovement(HashSet<Vector3Int> move_options) {
        Vector3Int newPos = playerPos;
        
        if (Input.GetMouseButtonDown(0)){ //&& move_options.Contains(gridManager.mousePositionToTileMapPosition(Input.mousePosition))) {
            Vector3Int matrix_pos = gridManager.tilemapGridToMatrixPos( gridManager.mousePositionToTileMapPosition(Input.mousePosition));
            Debug.Log("Moving " + matrix_pos);
            moveTo(matrix_pos);
        }
        /*
        if (Input.GetKeyDown(KeyCode.W))
        {
            newPos.y++;
        }
        else if (Input.GetKeyDown(KeyCode.A)) {
            newPos.x--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            newPos.y--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            newPos.x++;
        }

        if (playerPos != newPos && validMatrixPos(newPos) && moving == false)
        {
            moving = true;
            StartCoroutine(smoothMovement(playerPos, newPos));
            playerPos = newPos;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            gridManager.clearPaths();
            
        }*/
        return false;

    }

    void moveTo(Vector3Int matrixpos) {
        ArrayList path = gridManager.BFS_Path(playerPos, matrixpos);
        foreach(Vector3Int o in path) {
            move_queue.Enqueue((Vector3Int)o);
        }
    }


    IEnumerator smoothMovement(Vector3Int original_matrix_pos, Vector3Int new_matrix_pos) {
        
        Vector3 movement = new_matrix_pos - original_matrix_pos;
        Vector3 unit = movement / 10;
        while (movement != Vector3.zero) {
            
            transform.position += (unit);
            movement -= unit;
            yield return new WaitForSeconds(0.01f);
            
        }
        moving = false;
    }

    public bool validMatrixPos(Vector3Int matrix_pos) {

        if(matrix_pos.x < 0 || matrix_pos.y < 0 || matrix_pos.x >= gridManager.board_width || matrix_pos.y>= gridManager.board_height){
            return false;
        }

        //TODO: add other constraints. i.e. cannot move onto an enemy or an obstacle

        return true;
    }

}
