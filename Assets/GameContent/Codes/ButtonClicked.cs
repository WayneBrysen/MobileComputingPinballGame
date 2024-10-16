using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClicked : MonoBehaviour
{
    public bool isClicked = false;
    public string buttonName;

    public void OnButtonClicked()
    {
        isClicked = true;   
        buttonName = EventSystem.current.currentSelectedGameObject.name;
    }
}
