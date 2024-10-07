using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClicked : MonoBehaviour
{
    public bool isClicked = false;
    public void OnButtonClicked()
    {
        isClicked = true;   
    }
}
