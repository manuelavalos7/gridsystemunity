using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerManager : MonoBehaviour
{
    movementSystemObject[] playerCharacters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool all_done = true;
        playerCharacters = GetComponentsInChildren<movementSystemObject>();
        foreach (movementSystemObject o in playerCharacters) {
            if (!o.move_done) {
                all_done = false;
                break;
            }
        }
        
    }
}
