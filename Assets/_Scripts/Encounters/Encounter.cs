using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera camera;
    public Animator crossfadeTransition;

    public List<Enemy> enemies;
    public GameObject enemyUI;

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //GetComponent<Cinemachine.CinemachineTriggerAction>().

        // change priority of the encounter camera


        ChooseRandomEnemy();
        camera.Priority += 2;
        crossfadeTransition.SetTrigger("Start");

    }

    public void ChooseRandomEnemy()
    {
        int _rand = Random.Range(0, enemies.Count); // end is inclusive so its fine.
        enemyUI.GetComponent<UIAnimationController>().path = enemies[_rand].path;
        enemyUI.GetComponent<UIAnimationController>().Refresh();
        
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        camera.Priority -= 2;
    }

}
