using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dismiss : MonoBehaviour
{
    public void DismissDialog()
    {
        EventSystem.current.SetSelectedGameObject(Menu.lastButtonObject);
        Destroy(transform.parent.gameObject);
    }

}
