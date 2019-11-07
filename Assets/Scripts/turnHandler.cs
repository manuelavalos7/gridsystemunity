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
        //playerScript = GameObject.Find("Player").GetComponent<playerMovement>();
        enemyScript = GameObject.Find("Enemy").GetComponent<enemyMovement>();
        playerTurn = true;
        boardSetup = false;
        Debug.Log("Player Turn");
    }

    
    void Update()
    {

        
    }

    public void nextTurn() {
        playerTurn =!playerTurn;
        if (playerTurn)
        {
            enemyScript.enemyTurn = false;
        }
        else {
            enemyScript.enemyTurn = true;
        }
    }



}
