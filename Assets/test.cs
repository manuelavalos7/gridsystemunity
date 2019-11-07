using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public int spaces;

    void Start()
    {
        spaces = PlayerPrefs.GetInt("SpacesCount");    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            spaces++;
            PlayerPrefs.SetInt("SpacesCount",spaces);//name and value
        }
    }
}
