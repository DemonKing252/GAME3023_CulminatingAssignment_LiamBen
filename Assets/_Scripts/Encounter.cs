using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera camera;
    public Animator crossfadeTransition;
    public Canvas battleCanvas;
    
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //GetComponent<Cinemachine.CinemachineTriggerAction>().

        // change priority of the encounter camera

        camera.Priority += 2;
        crossfadeTransition.SetTrigger("Start");

        battleCanvas.gameObject.SetActive(true);
    }


}
