using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeActiveScene : MonoBehaviour
{
    [SerializeField]
    //name of next scene to load        --why is there no inspector ref to a scene in Unity?
    public string newSceneName;

    //loads a new base scene
    public void ChangeScene()
    {

        Time.timeScale = 1f;
        SceneManager.LoadScene(newSceneName, LoadSceneMode.Single);
    }


    //loads a Scene which appears in the Hierarchy window while another is active.      <-stolen documentation from the Unity wiki at https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.html
    public void AddAdditiveScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(newSceneName, LoadSceneMode.Additive);
    }
}
