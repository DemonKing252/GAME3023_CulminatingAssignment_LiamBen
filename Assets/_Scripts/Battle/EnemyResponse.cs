using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResponse : MonoBehaviour
{
    [SerializeField]
    public GameObject enemyRef;
    private GameObject playerRef;

    #region conditionalAttributes that affect descision making
    private float playerHealth;
    private float playerMaxHealth;
    private bool playerIsChargingAttack;
    private float enemyHealth;
    private float enemyMaxHealth;
    private bool enemyIsChargingAttack;
    private bool enemyConsiderFleeing;
    private bool enemyHealsPassively;
    #endregion

    private float descisionWeightFlee = 0;
    private float descisionWeightDodge = 0;
    private float descisionWeightAttack = 0;
    private float descisionWeightAttackSpecial = 0;
    private float descisionWeightHeal = 0;

    private void EnemyDescideAction()
    {
        //Each descision has a float from 0 to 10. Higher the float, the more likely it is to be picked.
        ResetDescisionVals();


        #region Handle Healing Possibility
        //if enemy is NOT at full health
        if (enemyHealth < enemyMaxHealth)
        {
            //Health Comparison Influences
                if (enemyHealth < enemyMaxHealth / 2) descisionWeightHeal++;            //enemy under 1/2 health, add single point
                if (enemyHealth < enemyMaxHealth / 3) descisionWeightHeal += 2.0f;        //enemy under 1/3 health, add total of 3 points
                if (enemyHealth < enemyMaxHealth / 4) descisionWeightHeal += 2.0f;        //enemy under 1/4 health, add total of 5 points


            if (enemyHealth < playerHealth) descisionWeightHeal += 1.5f;              //enemy has less health than player and is thus losing. Add 1.5 more points.
            
            if (!enemyHealsPassively) descisionWeightHeal *= 1.25f;   //enemy doesn't regen health passively, multiply heal likelyhood by 1.25. If all if have passed thus far, Enemy MUST heal as its descisionWeightHeal will be 10!
        }
        #endregion

        #region Handle Attacking Possibility
        //Health Comparison Influences
            if (playerHealth < playerMaxHealth / 1.5) descisionWeightAttack++;              //player under  2/3 health, add single point
            if (playerHealth < playerMaxHealth / 2) descisionWeightAttack++;                //player under under 1/2 health, add total of 2 points
            if (playerHealth < playerMaxHealth / 3) descisionWeightAttack += 2;             //player under under 1/3 health, add total of 4 points
            if (playerHealth < playerMaxHealth / 4) descisionWeightAttack += 2;             //player under under 1/4 health, add total of 6 points
           

        if (enemyHealth < playerHealth)
            descisionWeightAttack += 3;                                                 //enemy has less health than player. Add 2 points to attacking.
        else
            descisionWeightAttack += 2;                                                    //enemy has more health than player. Add 2 points to attacking. (guarentees at least a weight of 2 for attacking)
        #endregion


        #region Handle Fleeing Possibility
        if (enemyConsiderFleeing)
        {
            descisionWeightFlee++;          //if an option, add a single point right off the bat.

            if (enemyHealth < enemyMaxHealth / 3) descisionWeightFlee += 2;    //if enemy under 1/3 health, add another 2 points
            if (enemyHealth < enemyMaxHealth / 4) descisionWeightFlee += 2;    //if enemy under 1/4 health, add another 1 point

            //NOTE: fleeing doesn't sum to 10! Instead it'll sum to 4! This is so there is at most a 4/10 chance of enemy ATTEMPTING to flee!
        }
        #endregion


        //THIS ORDER IS IMPORTANT! FLEE->HEAL->ATTACK->DODGE->ATTACKSPECIAL
        //Each descion weight maxes at 10 (except flee that maxes out at 5). Highest range of random maxes at 15 to ensure enemies can make stategically wrong descisons!
        if (enemyConsiderFleeing && Random.Range(0.0f, 15.0f) <= descisionWeightFlee)//first action to consider is fleeing.
        {
            //attempt to flee
            EnemyAttemptFlee();
            return;
        }
        if (descisionWeightHeal > 0 && Random.Range(0.0f, 15.0f) <= descisionWeightHeal)       //second action to consider is healing.
        {
            EnemyHealSelf();
            return;
        }
        if (Random.Range(0.0f, 15.0f) <= descisionWeightAttack)     //third action to consider is attacking.
        {
            EnemyAttackNormal();
            return;
        }


        //failsafe. If all else failed, attack anyways. This doesn't make sense now as this is basically just an else to the health if, but when more options become implemented it'll make more sense.
        EnemyAttackNormal();
    }





    public void EnemyRespond()
    {
        //first we'll get all the information we require to make a descision
        GetConditionalAttributes();

        EnemyDescideAction();
        PrintDescisionStats();
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

            DialogueManager.GetInstance().StartNewDialogue("Enemy Flees and Escaped");
            //Debug.Log("Enemy escaped");
            GetComponent<BattleManager>().ShutdownBattle();     //stop the battle immediately
        }
        else
        {

            DialogueManager.GetInstance().StartNewDialogue("Enemy Failed to Flee");
            
            //Debug.Log("Enemy failed to escape");
        }
    }

    private void EnemyHealSelf()
    {
        DialogueManager.GetInstance().StartNewDialogue("Enemy healed!");
        enemyRef.GetComponent<CombatAttributes>().IncreaseHealth(enemyRef.GetComponent<CombatAttributes>().GetHealAmount());
    }

    private void EnemyAttemptDodge()
    {

    }

    private void EnemyAttackNormal()
    {
        DialogueManager.GetInstance().StartNewDialogue("Enemy attacked!");
        playerRef.GetComponent<CombatAttributes>().DecreaseHealth(enemyRef.GetComponent<CombatAttributes>().GetDamageDealNormal());
    }

    private void EnemyAttackSpecial()
    {

    }

    private void GetConditionalAttributes()
    {
        playerHealth = GetComponent<BattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().GetHealth();
        playerMaxHealth = GetComponent<BattleManager>().GetPlayerRef().GetComponent<CombatAttributes>().GetMaxHealth();
        playerIsChargingAttack = false; //unimplemented
        enemyHealth = enemyRef.GetComponent<CombatAttributes>().GetHealth();
        enemyMaxHealth = enemyRef.GetComponent<CombatAttributes>().GetMaxHealth();
        enemyIsChargingAttack = false; //unimplemented
        enemyConsiderFleeing = CheckAllowEnemyFlee(); //check if fleeing the battle is an option or not.
        enemyHealsPassively = false; //unimplemented

        playerRef = GetComponent<BattleManager>().GetPlayerRef();
    }  

    private void PrintDescisionStats()
    {
        Debug.Log("FLEE: " + descisionWeightFlee + "  ATTCK: " + descisionWeightAttack + "  HEAL: " + descisionWeightHeal);
    }

    private void ResetDescisionVals()
    {
        descisionWeightFlee = 0;
        descisionWeightDodge = 0;
        descisionWeightAttack = 0;
        descisionWeightAttackSpecial = 0;
        descisionWeightHeal = 0;
    }
}
