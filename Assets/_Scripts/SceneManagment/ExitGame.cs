using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ShutdownGame()
    {
        Application.Quit();
        Debug.Log("Game should shut down now. Doesn't work in editor.");
    }
}
