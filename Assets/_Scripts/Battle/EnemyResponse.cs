using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResponse : MonoBehaviour
{

    //[SerializeField]
    //private CombatAttributeModifier defaultBehaviourModifier;
    //[SerializeField]
    //public CombatAttributeModifier behaviourModifier;

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
    private float decisionWeightAttack = 0;
    private float decisionWeightHeal = 0;

    private void ResetdecisionVals()
    {
        decisionWeightFlee = 0;
        decisionWeightAttack = 0;
        decisionWeightHeal = 0;
    }
    #endregion

    public choiceAction EnemyRespond()
    {

        //second we'll get all the information we require to make a decision
        GetConditionalAttributes();



        //then we decide what action to take
        choiceAction enemyDecision = EnemyDecideAction();
        return enemyDecision;
    }

    private void EnemyCalculateHealing()
    {
        if (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.fleeConsideration == modifierStates.neverBehaviour)
        {
            decisionWeightHeal = -1;
            return;
        }


        //if enemy is NOT at full health
        if (enemyHealth < enemyMaxHealth)
        {
            //Health Comparison Influences
            if (enemyHealth < enemyMaxHealth / 2) decisionWeightHeal++;            //enemy under 1/2 health, add single point
            if (enemyHealth < enemyMaxHealth / 3) decisionWeightHeal += 2.0f;        //enemy under 1/3 health, add total of 3 points
            if (enemyHealth < enemyMaxHealth / 4) decisionWeightHeal += 2.0f;        //enemy under 1/4 health, add total of 5 points


            if (enemyHealth < playerHealth) decisionWeightHeal += 3.0f;              //enemy has less health than player and is thus losing. Add 3.0 more points.
        }
    }

    private void EnemyCalculateAttacking()
    {
        if (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.attackConsideration == modifierStates.neverBehaviour)
        {
            decisionWeightAttack = -1;
            return;
        }

        //Health Comparison Influences
        if (playerHealth < playerMaxHealth / 1.5) decisionWeightAttack++;              //player under  2/3 health, add single point
        if (playerHealth < playerMaxHealth / 2) decisionWeightAttack++;                //player under under 1/2 health, add total of 2 points
        if (playerHealth < playerMaxHealth / 3) decisionWeightAttack += 2;             //player under under 1/3 health, add total of 4 points
        if (playerHealth < playerMaxHealth / 4) decisionWeightAttack += 2;             //player under under 1/4 health, add total of 6 points


        if (enemyHealth < playerHealth)
            decisionWeightAttack += 3;                                                 //enemy has less health than player. Add 2 points to attacking.
        else
            decisionWeightAttack += 2;                                                    //enemy has more health than player. Add 2 points to attacking. (guarentees at least a weight of 2 for attacking)

    }

    private void EnemyCalculateFleeing()
    {
        if (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.fleeConsideration == modifierStates.neverBehaviour)
        {
            decisionWeightFlee = -1;
            return;
        }

        if (enemyConsiderFleeing)
        {
            decisionWeightFlee += 3;          //if an option, add a 3 points right off the bat.

            if (enemyHealth < enemyMaxHealth / 3) decisionWeightFlee += 2;    //if enemy under 1/3 health, add another 2 points
            if (enemyHealth < enemyMaxHealth / 4) decisionWeightFlee += 2;    //if enemy under 1/4 health, add another 2 points
        }
    }
    private void EnemyCalculateAction()
    {
        //Each decision has a float from 0 to 10. Higher the float, the more likely it is to be picked.
        ResetdecisionVals();

        EnemyCalculateHealing();

        EnemyCalculateAttacking();

        EnemyCalculateFleeing();

    }

    private choiceAction EnemyDecideAction()
    {


        if (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.attackConsideration == modifierStates.certaintyBehaviour)
            return choiceAction.attack;
        if (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.healConsideration == modifierStates.certaintyBehaviour)
            return choiceAction.heal;
        if (enemyRef.GetComponent<CombatAttributes>().behaviourModifier.fleeConsideration == modifierStates.certaintyBehaviour)
        {
            //attempt to flee
            if (Random.Range(0.0f, 1.0f) <= 0.85) //make fleeing from special flee ability very likely
                return choiceAction.flee;
            else
                return choiceAction.fleeFail;
        }

        //ok so lets figure out what each decision weights are...
        EnemyCalculateAction();


        //Now we go through all decision options one-by-one to determine an action! Each decision is weighted in the above function


        //THIS ORDER IS IMPORTANT! FLEE->HEAL->ATTACK
        //Each decision weight maxes at 10 (except flee that maxes out at 7). Highest range of random maxes at 15 to ensure enemies can make stategically wrong descisons! (Except flee that has random max set to 10 making fleeing on low enemy low health likely)
        if (decisionWeightFlee > 0 && enemyConsiderFleeing && Random.Range(0.0f, 10.0f) <= decisionWeightFlee)//first action to consider is fleeing.
        {
            //attempt to flee
            if (Random.Range(0.0f, 1.0f) <= enemyRef.GetComponent<CombatAttributes>().GetSuccessFleeingChance())    //if a random float between 0 inclusive and 1 inclusive is LESS OR EQUAL to successFleeingChance then fleeing is considered a success and battle should end.
                return choiceAction.flee;
            else
                return choiceAction.fleeFail;
        }
        if (decisionWeightHeal > 0 && Random.Range(0.0f, 15.0f) <= decisionWeightHeal)       //second action to consider is healing.
        {
            return choiceAction.heal;
        }

        //failsafe. If all else failed, attack normal.
        return choiceAction.attack;
    }


    private bool CheckAllowEnemyFlee()
    {
        if (enemyRef.GetComponent<CombatAttributes>().GetHealth() / enemyRef.GetComponent<CombatAttributes>().GetMaxHealth() <      //if monster's health/maxHealth is below flee threshold, return true and allow enemy to consider fleeing.
            enemyRef.GetComponent<CombatAttributes>().GetThresholdForFleeing())
            return true;   //allow fleeing
        else
            return false;   //disallow fleeing
    }

   

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
