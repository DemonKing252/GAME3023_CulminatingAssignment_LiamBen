using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ShutdownGame()
    {
#if UNITY_EDITOR
        // Editor mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Build mode
        Application.Quit();
#endif
    }
}
