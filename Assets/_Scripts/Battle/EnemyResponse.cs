using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResponse : MonoBehaviour
{
    private CombatAttributeModifier behaviourModifier;

    [SerializeField]
    public GameObject enemyRef;
    [SerializeField]
    private GameObject playerRef;

    choiceAction enemyActionToTake;

    #region conditionalAttributes that affect decision making
    private float playerHealth;
    private float playerMaxHealth;
    private bool playerIsChargingAttack;
    private float enemyHealth;
    private float enemyMaxHealth;
    private bool enemyIsChargingAttack;
    private bool enemyConsiderFleeing;
    private bool enemyHealsPassively;
    #endregion

    #region weightingVars
    private float decisionWeightFlee = 0;
    private float decisionWeightDodge = 0;
    private float decisionWeightAttack = 0;
    private float decisionWeightAttackSpecial = 0;
    private float decisionWeightHeal = 0;

    private void ResetdecisionVals()
    {
        decisionWeightFlee = 0;
        decisionWeightDodge = 0;
        decisionWeightAttack = 0;
        decisionWeightAttackSpecial = 0;
        decisionWeightHeal = 0;
    }
    #endregion

    public choiceAction EnemyRespond()
    {
        //return choiceAction.attack;
        if (behaviourModifier != null)
        {
            Debug.Log("ENEMY INFLUENCED!!!!");
        }

        //second we'll get all the information we require to make a decision
        GetConditionalAttributes();
        //then we decide what action to take
        return EnemyDecideAction();
    }

    private void EnemyCalculateAction()
    {
        //Each decision has a float from 0 to 10. Higher the float, the more likely it is to be picked.
        ResetdecisionVals();


        #region Handle Healing Possibility
        //if enemy is NOT at full health
        if (enemyHealth < enemyMaxHealth)
        {
            //Health Comparison Influences
            if (enemyHealth < enemyMaxHealth / 2) decisionWeightHeal++;            //enemy under 1/2 health, add single point
            if (enemyHealth < enemyMaxHealth / 3) decisionWeightHeal += 2.0f;        //enemy under 1/3 health, add total of 3 points
            if (enemyHealth < enemyMaxHealth / 4) decisionWeightHeal += 2.0f;        //enemy under 1/4 health, add total of 5 points


            if (enemyHealth < playerHealth) decisionWeightHeal += 1.5f;              //enemy has less health than player and is thus losing. Add 1.5 more points.

            if (!enemyHealsPassively) decisionWeightHeal *= 1.25f;   //enemy doesn't regen health passively, multiply heal likelyhood by 1.25. If all if have passed thus far, Enemy MUST heal as its decisionWeightHeal will be 10!
        }
        #endregion

        #region Handle Attacking Possibility
        //Health Comparison Influences
        if (playerHealth < playerMaxHealth / 1.5) decisionWeightAttack++;              //player under  2/3 health, add single point
        if (playerHealth < playerMaxHealth / 2) decisionWeightAttack++;                //player under under 1/2 health, add total of 2 points
        if (playerHealth < playerMaxHealth / 3) decisionWeightAttack += 2;             //player under under 1/3 health, add total of 4 points
        if (playerHealth < playerMaxHealth / 4) decisionWeightAttack += 2;             //player under under 1/4 health, add total of 6 points


        if (enemyHealth < playerHealth)
            decisionWeightAttack += 3;                                                 //enemy has less health than player. Add 2 points to attacking.
        else
            decisionWeightAttack += 2;                                                    //enemy has more health than player. Add 2 points to attacking. (guarentees at least a weight of 2 for attacking)
        #endregion

        #region Handle Fleeing Possibility
        if (enemyConsiderFleeing)
        {
            decisionWeightFlee++;          //if an option, add a single point right off the bat.

            if (enemyHealth < enemyMaxHealth / 3) decisionWeightFlee += 2;    //if enemy under 1/3 health, add another 2 points
            if (enemyHealth < enemyMaxHealth / 4) decisionWeightFlee += 2;    //if enemy under 1/4 health, add another 1 point

            //NOTE: fleeing doesn't sum to 10! Instead it'll sum to 4! This is so there is at most a 4/10 chance of enemy ATTEMPTING to flee!
        }
        #endregion

    }

    private choiceAction EnemyDecideAction()
    {
        //ok so lets figure out what each decision weights are...
        EnemyCalculateAction();


        //Now we go through all decision options one-by-one to determine an action! Each decision is weighted in the above function


        //THIS ORDER IS IMPORTANT! FLEE->HEAL->ATTACK->DODGE->ATTACKSPECIAL
        //Each decision weight maxes at 10 (except flee that maxes out at 5). Highest range of random maxes at 15 to ensure enemies can make stategically wrong descisons!
        if (enemyConsiderFleeing && Random.Range(0.0f, 15.0f) <= decisionWeightFlee)//first action to consider is fleeing.
        {
            //attempt to flee
            return choiceAction.flee;
        }
        if (decisionWeightHeal > 0 && Random.Range(0.0f, 15.0f) <= decisionWeightHeal)       //second action to consider is healing.
        {
            return choiceAction.heal;
        }
        if (Random.Range(0.0f, 15.0f) <= decisionWeightAttack)     //third action to consider is attacking.
        {
            return choiceAction.attack;
        }


        //failsafe. If all else failed, attack anyways. This doesn't make sense now as this is basically just an else to the health if, but when more options become implemented it'll make more sense.
        return choiceAction.attack;
    }


    //#region fleeing

    private bool CheckAllowEnemyFlee()
    {
        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() / enemyRef.GetComponent<CombatAttributes>().GetMaxHealth() <      //if monster's health/maxHealth is below flee threshold, return true and allow enemy to consider fleeing.
            enemyRef.GetComponent<CombatAttributes>().GetThresholdForFleeing()) return true;   //allow fleeing
        else return false;   //disallow fleeing
    }

    //private void EnemyAttemptFlee()     //enemy will attempt to flee. Returns true if attempt succeeded, false if attempt failed.
    //{

    //    if (Random.Range(0.0f, 1.0f) <= enemyRef.GetComponent<CombatAttributes>().GetSuccessFleeingChance())    //if a random float between 0 inclusive and 1 inclusive is LESS OR EQUAL to successFleeingChance then fleeing is considered a success and battle should end.
    //    {

    //        DialogueManager.GetInstance().StartNewDialogue("Enemy Flees and Escaped");
    //        //Debug.Log("Enemy escaped");
    //        GetComponent<BattleManager>().ShutdownBattle();     //stop the battle immediately
    //    }
    //    else
    //    {

    //        DialogueManager.GetInstance().StartNewDialogue("Enemy Failed to Flee");

    //        //Debug.Log("Enemy failed to escape");
    //    }
    //}

    //#endregion

    //private void EnemyHealSelf()
    //{
    //    DialogueManager.GetInstance().StartNewDialogue("Enemy healed!");
    //    enemyRef.GetComponent<CombatAttributes>().IncreaseHealth(enemyRef.GetComponent<CombatAttributes>().GetHealAmount());
    //    GetComponent<BattleManager>().GetEnemyAnimator().SetTrigger("Healing");
    //}

    //#region attackingAndDodging
    //private void EnemyAttemptDodge()
    //{
    //    GetComponent<BattleManager>().GetEnemyAnimator().SetTrigger("Dodge");
    //}

    //private void EnemyAttackNormal()
    //{
    //    DialogueManager.GetInstance().StartNewDialogue("Enemy attacked!");

    //    if (playerRef.GetComponent<CombatAttributes>().GetShouldPlayDamageReceivedAnim())    //if dice roll and dodge status allows playing of damaged recieved anim, play it.
    //        GetComponent<BattleManager>().GetPlayerAnimator().SetTrigger("Normal");

    //    playerRef.GetComponent<CombatAttributes>().DecreaseHealth(enemyRef.GetComponent<CombatAttributes>().GetDamageDealNormal());     //deal the damage to the player
    //}


    //#endregion

    private void GetConditionalAttributes()
    {
        playerHealth = playerRef.GetComponent<CombatAttributes>().GetHealth();
        playerMaxHealth = playerRef.GetComponent<CombatAttributes>().GetMaxHealth();
        playerIsChargingAttack = false; //unimplemented
        enemyHealth = enemyRef.GetComponent<CombatAttributes>().GetHealth();
        enemyMaxHealth = enemyRef.GetComponent<CombatAttributes>().GetMaxHealth();
        enemyIsChargingAttack = false; //unimplemented
        enemyConsiderFleeing = CheckAllowEnemyFlee(); //check if fleeing the battle is an option or not.
        enemyHealsPassively = false; //unimplemented
    }

    private void PrintdecisionStats()
    {
        Debug.Log("FLEE: " + decisionWeightFlee + "  ATTCK: " + decisionWeightAttack + "  HEAL: " + decisionWeightHeal);
    }


}
