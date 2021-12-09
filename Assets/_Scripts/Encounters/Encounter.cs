using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    public bool guarenteeEncounter = false;
    public GameObject selectedEnemy;
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
            if (!guarenteeEncounter && collision.gameObject.GetComponent<PlayerController>() != null)
            {
                // 50% chance to happen
                if (Random.Range(1, 3) == 2)
                    return;

                // If the player is moving at a decent speed
                if (collision.gameObject.GetComponent<PlayerController>().GetComponent<Rigidbody2D>().velocity.magnitude < 0.4f)
                    return;

                // If the above (inverse) condtions are met, we let the battle happen
            }

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

            FindObjectOfType<PlayerController>().referencedEncounter = this;
            FindObjectOfType<PlayerController>().masterBattleMgr.GetComponent<MasterBattleManager>().SetUpNewBattle(selectedEnemy, this);
            enemyNameUI.GetComponent<GetName>().SetEnemyName(selectedEnemy.GetComponent<CombatAttributes>().GetName());

        } while (selectedEnemy.GetComponent<CombatAttributes>().GetHealth() <= 0f);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        camera.Priority -= 2;
    }

}
