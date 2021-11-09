using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    public GameObject selectedEnemy;

    public Cinemachine.CinemachineVirtualCamera camera;
    public Animator crossfadeTransition;

    public List<GameObject> enemies;
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

        selectedEnemy = enemies[_rand];
        enemyUI.GetComponent<UIAnimationController>().path = enemies[_rand].GetComponent<Enemy>().path;
        enemyUI.GetComponent<UIAnimationController>().Refresh();
        FindObjectOfType<PlayerUI>().selectedEncounter = this;
        
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        camera.Priority -= 2;
    }

}
