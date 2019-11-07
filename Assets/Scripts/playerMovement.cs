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
    public bool playerTurn = true;
    private bool turnDone = false;

    public const int player_moves = 4;

    private bool select_started=false;
    private Vector3Int last_position;
    private bool mousePressed = false;
    private int spaces_selected = 0;
    

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
        if (!select_started && !turnDone) {
            gridManager.possible_moves(playerPos, player_moves);
        }
        if (turnManager.playerTurn && !turnDone) {
            //tempMovement(gridManager.possible_moves(playerPos, 3));
            selectMove();
        }
        else if (turnManager.playerTurn && turnDone)
        {
            makeMove();
        }

    }

    void selectMove() {
        if (Input.GetMouseButton(0) && !select_started && gridManager.matrixPosToWorld(playerPos) == gridManager.mousePositionToTileMapPosition(Input.mousePosition)) {
            mousePressed = true;
            Vector3Int next = gridManager.mousePositionToTileMapPosition(Input.mousePosition);
            last_position = gridManager.tilemapGridToMatrixPos(next);
            gridManager.board_tilemap.SetTile(next, gridManager.red_tile);
            select_started = true;
        }

        if (Input.GetMouseButton(0) && select_started && spaces_selected < player_moves) {
            Vector3Int next = gridManager.mousePositionToTileMapPosition(Input.mousePosition);
            Vector3Int nextCoord = gridManager.tilemapGridToMatrixPos(next);
            if (gridManager.are_adjacent(last_position, nextCoord))
            {
                gridManager.board_tilemap.SetTile(next, gridManager.red_tile);
                move_queue.Enqueue(nextCoord);
                last_position = nextCoord;
                spaces_selected++;
            }
            else if(last_position != nextCoord){
                select_started = false;
                mousePressed = false;
                spaces_selected = 0;
                gridManager.clearPaths();
                move_queue.Clear();
            }
        }

        if (mousePressed && !Input.GetMouseButton(0)) {
            turnDone = true;
            mousePressed = false;
            select_started = false;
            spaces_selected = 0;
            turnManager.nextTurn();
        }
        
    }

    public void tempMovement(HashSet<Vector3Int> move_options) {
        Vector3Int newPos = playerPos;
        Debug.Log(gridManager.mousePositionToTileMapPosition(Input.mousePosition));
        if (Input.GetMouseButtonDown(0) && move_options.Contains(gridManager.tilemapGridToMatrixPos(gridManager.mousePositionToTileMapPosition(Input.mousePosition)))) {
            Vector3Int matrix_pos = gridManager.tilemapGridToMatrixPos( gridManager.mousePositionToTileMapPosition(Input.mousePosition));
            Debug.Log("Moving " + matrix_pos);
            moveTo(matrix_pos);
        }

    }

    void makeMove() {
        if (move_queue.Count > 0 && !moving)
        {
            Vector3Int next_move = move_queue.Dequeue();
            StartCoroutine(smoothMovement(playerPos, next_move));
            playerPos = next_move;
        }
        else if (move_queue.Count ==0 && turnDone) {
            gridManager.clearPaths();
            turnDone = false;
            turnManager.nextTurn();
        }
    }

    void moveTo(Vector3Int matrixpos) {
        ArrayList path = gridManager.BFS_Path(playerPos, matrixpos);
        path.Reverse();
        foreach(Vector3Int o in path) {
            move_queue.Enqueue((Vector3Int)o);
        }
        turnDone = true;
    
    }


    IEnumerator smoothMovement(Vector3Int original_matrix_pos, Vector3Int new_matrix_pos) {
        moving = true;
        Vector3 movement = new_matrix_pos - original_matrix_pos;
        Vector3 unit = movement / 10;
        while (movement != Vector3.zero) {
            
            transform.position += (unit);
            movement -= unit;
            yield return new WaitForSeconds(0.01f);
            
        }
        moving = false;
    }



}
