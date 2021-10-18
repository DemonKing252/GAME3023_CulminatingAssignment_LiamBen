using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAttributes : MonoBehaviour
{

    protected string entityName = "ParentEntity";   //name of this entity

    #region healthVars
    [Header("Health Vars")]
    [SerializeField]
    protected float health = 3.0f;    //amount of health enemy has - using a float for health so a slow regen can take place

    [SerializeField]
    protected float maxHealth = 3.0f;    //max amount of health enemy could have (used for checking if need to regen)

    [SerializeField]
    protected float healthPassiveRegen = 0.25f;   //amount of health to passivly regen during turn (<=0 for no regen)

    [SerializeField]
    protected float healAmount = 1.0f;     //amount of health to heal if turn is spent ONLY healing
    #endregion

    #region combatReceivedVars
    [Header("Combat Recived Vars")]
    [SerializeField]
    protected float likelihoodOfDodgeAttack = 0.3f;   //how likely the monster is to dodge player's attack (>= 1 for always, <= 0 for never)

    [SerializeField]
    protected float likelihoodOfReceivedCriticalHit = 0.2f;   //how likely the opponent's attack is to land a critical hit (does extra damage) (>= 1 for awlays, <= 0 for never)

    [SerializeField]
    protected float criticalHitReceivedMultiplier = 1.5f;     //how much the crittical hit attack should be multiplied
    #endregion

    #region combatDealVars
    [Header("Combat Deal Vars")]
    [SerializeField]
    protected float damageDealNormal = 1.0f;      //how much damage to deal during a normal attack

    [SerializeField]
    protected float damageDealSpecial = 2.0f;   //how much damage to deal during a special attack (will want to move this to another script for specific abilities)
    #endregion

    #region fleeVars
    [Header("Fleeing Vars")]
    //may want to move these to a enemy-only child class, unless we want the player to only be able to flee if injured
    [SerializeField]
    protected float likelihoodOfFleeing = 0.1f;   //how likely it is the entity will attempt to flee (>= 1 for always, <= 0 for never)

    [SerializeField]
    protected float thresholdForFleeing = 0.5f;     //how little health entity will require before the option to flee becomes available.
    #endregion

    #region messageSuffixVars
    [Header("Message Suffix Vars")]
    //add these to the end of the ParentEntity string
    [SerializeField]
    protected string deathMessage = " has died!";  //message for when this entity dies

    [SerializeField]
    protected string damageDealNormalMessage = " has attacked!";    //message for when this entity attacks normally

    [SerializeField]
    protected string damageDealSpecialMessage = " has used Special attack!";    //message for when this entity attacks using a special attack (will want to move this to another script for specific abilities)

    [SerializeField]
    protected string healMessage = " is healing!";    //message for when this entity uses the turn to heal

    [SerializeField]
    protected string criticalHitMessage = " has recieved a Critical Hit!";    //message for when this entity receives a critical hit

    [SerializeField]
    protected string fleeFailedMessage = " has tried to run away, but failed!";    //message for when this entity attempts to flee and doesn't get lucky with their dice roll

    [SerializeField]
    protected string fleeSuccessMessage = " has run away!";    //message for when this entity attempts to flee and escapes
    #endregion

    #region healthManagment
    //checks if health is within >0 and max health, and returns accordingly. True if within bounds, false if not
    private bool ManageHealthBounds() 
    {
        //check if health is between 0 (exclusive) and max health (inclusive). If passed, just return the hell outta the check.
        if (health > 0 && health <= maxHealth) return true;

        //If this runs then we gotta check if the health is negative or 0.
        if (health <= 0)
        {
            //player is dead. Set health to 0.
            health = 0;
            Debug.Log("Player is dead rip");//run some death function here<<<<<<<<<<<<<<<<<<
            return false;//return of true also means nothing here.
        }
        else//this only ever runs if health is greater than maxHealth
        {
            health = maxHealth;
            return true;
        }
    }

    //decrease health by passed amount. Returns true for if alive, false for dead.
    protected bool DecreaseHealth(float decreaseAmount)
    {
        health -= decreaseAmount;
        return ManageHealthBounds();
    }

    //attempts to increase health by the passed Amount. Returns true if the consumable has been picked up and used, false if already at full health.
    protected bool IncreaseHealth(float incrementAmount)
    {
        //if health is at max, return false disallowing player from consuming health pickup/item
        if (health == maxHealth) return false;
        health += incrementAmount;
        ManageHealthBounds();
        return true;
    }

    public void PassivlyRegenHealth()
    {
        IncreaseHealth(healthPassiveRegen);
    }

    public float GetHealth()
    {
        return health;
    }
    #endregion

    #region fleeManagment
    public bool allowFleeing()
    {
        if (health > (health * thresholdForFleeing)) return false;
        else return true;
    }
    #endregion
}