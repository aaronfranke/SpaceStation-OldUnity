using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public static bool debugmenu = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            debugmenu = !debugmenu;
        } // Causes F3 to act as a toggle for the debug UI. 

        if (debugmenu)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localPosition = new Vector3(0, -100000, 0);
        }

    }
}
