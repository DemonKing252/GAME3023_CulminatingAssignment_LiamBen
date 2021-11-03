using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResponse : MonoBehaviour
{
    [SerializeField]
    protected GameObject enemyRef;


    public void EnemyRespond()
    {
        bool considerFleeing = CheckAllowEnemyFlee(); //check if fleeing the battle is an option or not.



        //One weighted descision tree later...

        if(considerFleeing)
        {
            //for now we'll always attempt fleeing until we actually have some alternative options made.
            EnemyAttemptFlee();
        }
        else
        {
            //enemy has too much health to consider fleeing. For now there's no other state/action for it to take so for now the enemy will just sit there patiently
            Debug.Log("Enemy does nothing");
        }
    }


    private bool CheckAllowEnemyFlee()
    {
        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() / enemyRef.GetComponent<CombatAttributes>().GetMaxHealth() <      //if monster's health/maxHealth is below flee threshold, return true and allow enemy to consider fleeing.
            enemyRef.GetComponent<CombatAttributes>().GetThresholdForFleeing()) return true;   //allow fleeing
        else return false;   //disallow fleeing
    }

    private void EnemyAttemptFlee()     //enemy will attempt to flee. Returns true if attempt succeeded, false if attempt failed.
    {

        if (Random.Range(0.0f, 1.0f) <= enemyRef.GetComponent<CombatAttributes>().GetSuccessFleeingChance())    //if a random float between 0 inclusive and 1 inclusive is LESS OR EQUAL to successFleeingChance then fleeing is considered a success and battle should end.
        {
            Debug.Log("Enemy escaped");
            GetComponent<BattleManager>().ShutdownBattle();     //stop the battle immediately
        }
        else
        {
            Debug.Log("Enemy failed to escape");
        }
    }

    private void EnemyHealSelf()
    {

    }

    private void EnemyAttemptDodge()
    {

    }

    private void EnemyAttackNormal()
    {

    }

    private void EnemyAttackSpecial()
    {

    }

}
