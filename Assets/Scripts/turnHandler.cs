using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnHandler : MonoBehaviour
{
    private playerMovement playerScript;
    private enemyMovement enemyScript;
    HashSet<Vector3Int> move_options;

    public bool playerTurn;
    private bool boardSetup;
    private float timer = 0;

    void Start()
    {
        playerTurn = true;
        boardSetup = false;
        Debug.Log("Player Turn");
    }

    
    void Update()
    {
        playerScript = GameObject.Find("Player").GetComponent<playerMovement>();
        enemyScript = GameObject.Find("Enemy").GetComponent<enemyMovement>();

        if (playerTurn)
        {

            timer -= Time.deltaTime * 20f;
            if (timer < 0) {
                
                if (!boardSetup)
                {
                    move_options = playerScript.gridManager.possible_moves(playerScript.playerPos, 2);
                    boardSetup = true;
                }
            
                if (playerScript.tempMovement(move_options))
                {
                    playerTurn = false;
                    Debug.Log("Enemy Turn");
                    timer = 10f;
                    boardSetup = false;
                    playerScript.gridManager.clearPaths();
                }
            }
        }
        else
        {
            timer -= Time.deltaTime * 10f;
            if (timer < 0)
            {
                if (enemyScript.tempMovement())
                {
                    playerTurn = true;
                    Debug.Log("Player Turn");
                    timer = 10f;
                }
            }
            
        }
        
    }


}
