using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Canvas encounterCanvas;
    // Animation event
    public void ShowEncounter()
    {
        encounterCanvas.gameObject.SetActive(true);
    }
}
