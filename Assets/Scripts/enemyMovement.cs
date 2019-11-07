using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{
    
    private playerMovement playerScript;
    private gridHandler gridManager;
    private turnHandler turnManager;

    public bool enemyTurn = false;
    public Vector3Int enemyPos;
    private bool moving = false;
    private bool turnDone = false;
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<playerMovement>();
        gridManager = GameObject.Find("gridManager").GetComponent<gridHandler>();
        turnManager = GameObject.Find("turnManager").GetComponent<turnHandler>();
        enemyPos = new Vector3Int(gridManager.board_width-1, gridManager.board_height/2, 0);
        transform.position = gridManager.matrixPosToWorld(enemyPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyTurn && !turnDone)
        {
            tempMovement();
        }
        else if( enemyTurn && turnDone && !moving){
            enemyTurn = false;
            turnDone = false;

            turnManager.nextTurn();
        }
    }

    public void tempMovement()
    {
        Vector3Int newPos = enemyPos;
        int move = (int)Mathf.Round(Random.Range(0, 3));
        if (move == 0)
        {
            newPos.y++;
        }
        else if (move ==1)
        {
            newPos.x--;
        }
        else if (move ==2)
        {
            newPos.y--;
        }
        else if (move==3)
        {
            newPos.x++;
        }

        if (enemyPos != newPos && playerScript.validMatrixPos(newPos) && moving == false)
        {
            moving = true;
            StartCoroutine(smoothMovement(enemyPos, newPos));
            enemyPos = newPos;
        }

    }

    IEnumerator smoothMovement(Vector3Int original_matrix_pos, Vector3Int new_matrix_pos)
    {
        moving = true;
        Vector3 movement = new_matrix_pos - original_matrix_pos;
        Vector3 unit = movement / 10;
        while (movement != Vector3.zero)
        {

            transform.position += (unit);
            movement -= unit;
            yield return new WaitForSeconds(0.01f);

        }
        moving = false;
        turnDone = true;
    }


}
