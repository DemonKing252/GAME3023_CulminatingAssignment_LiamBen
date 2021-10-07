using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Canvas encounterCanvas;
    public GameObject playerui; // disable overworld ui while the player is in this encounter.
    // Animation event
    public void ShowEncounter()
    {
        encounterCanvas.gameObject.SetActive(true);
        playerui.SetActive(false);
    }
}
