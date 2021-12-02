using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    public GameObject selectedEnemy;
    public MasterBattleManager battleManager;
    public GameObject enemyNameUI;

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
        do
        {
            int _rand = Random.Range(0, enemies.Count); // end is inclusive so its fine.

            selectedEnemy = enemies[_rand];
            enemyUI.GetComponent<UIAnimationController>().path = enemies[_rand].GetComponent<Enemy>().path;
            enemyUI.GetComponent<UIAnimationController>().Refresh();
            FindObjectOfType<PlayerUI>().selectedEncounter = this;


            battleManager.GetComponent<MasterBattleManager>().initialEncounter = this;
            battleManager.GetComponent<MasterBattleManager>().enemyRef = selectedEnemy;
            battleManager.GetComponent<MasterBattleManager>().SetUpNewBattle(selectedEnemy, this);
            enemyNameUI.GetComponent<GetName>().SetEnemyName(selectedEnemy.GetComponent<CombatAttributes>().GetName());

        } while (selectedEnemy.GetComponent<CombatAttributes>().GetHealth() <= 0f);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        camera.Priority -= 2;
    }

}
