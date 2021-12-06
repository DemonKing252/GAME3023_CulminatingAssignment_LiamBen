using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAttributes : MonoBehaviour
{
    #region name
    [SerializeField]
    protected string entityName = "ParentEntity";   //name of this entity
    public string GetName()
    {
        return entityName;
    }

    #endregion

    [SerializeField]
    public CombatAttributeModifier defaultBehaviourModifier;
    [SerializeField]
    public CombatAttributeModifier behaviourModifier;

    #region healthVars
    [Header("Health Vars")]
    [SerializeField]
    protected float health = 3.0f;              //amount of health enemy has - using a float for health so a slow regen can take place

    [SerializeField]
    protected float maxHealth = 3.0f;           //max amount of health enemy could have (used for checking if need to regen)

    [SerializeField]
    protected float healthPassiveRegen = 0.25f; //amount of health to passivly regen during turn (<=0 for no regen)

    [SerializeField]
    protected float healAmount = 1.0f;          //amount of health to heal if turn is spent ONLY healing
    #endregion

    #region combatReceivedVars
    [Header("Combat Recived Vars")]
    [SerializeField]
    protected float likelihoodOfDodgeAttack = 0.3f;             //how likely the monster is to dodge player's attack (>= 1 for always, <= 0 for never)

    [SerializeField]
    protected float likelihoodOfReceivedCriticalHit = 0.2f;     //how likely the opponent's attack is to land a critical hit (does extra damage) (>= 1 for awlays, <= 0 for never)
    [SerializeField]
    protected float likelihoodOfDodgeAttempt = 0.3f;             //how likely the monster is to dodge player's attack (>= 1 for always, <= 0 for never)
    [SerializeField]
    protected float criticalHitReceivedMultiplier = 1.5f;       //how much the crittical hit attack should be multiplied
    #endregion

    #region dodging
	
	public bool canDodge = true;                                     //can this enemy dodge at all?
	
    protected bool attemptDodgeAttack = false;                          //whether or not to dodge an incoming attack.

    protected bool successfullyDodgedAttack = false;                    //the randomly generated dice toss for if character WILL SUCCESSFULLY dodge an attack (seperated from dodging mechanic so it can be read in BattleManager BEFORE decreaseHealth is called)

    public bool RollDiceForDodgeSuccess()                                    //Sets up the dodging of an attack. Returns outcome of whether dodge succeeded
    {

        float diceRoll = Random.Range(0.0f, 1.5f);
        if (diceRoll <= likelihoodOfDodgeAttack)  //roll dice... Based on likelihoodOfDodgeAttack, generate random num from 0-1. If generated num is greater than likelihoodOfDodgeAttack, dodge succeeded. Fails if attemptDodgeAttack is false
            return true;                          
        else
            return false;                         
    }

    public bool RollDiceForDodgeAttempt()                                  
    {

        float diceRoll = Random.Range(0.0f, 1.5f);

        Debug.Log("AttempingDodge Dice: " + diceRoll + " / " + likelihoodOfDodgeAttempt);
        if (diceRoll <= likelihoodOfDodgeAttempt)
        {
            Debug.Log("Allow Dodge");
            return true;
        }
        else
        {
            Debug.Log("Disallow Dodge");
            return false;
        }
    }

    public void SetAttemptDodgeAttack(bool b)
    {
        attemptDodgeAttack = b;
        Debug.Log("SetAttemptDodgeAttack " + attemptDodgeAttack);
    }

    public bool GetattemptDodgeAttack()
    {
        return attemptDodgeAttack;
    }
    #endregion

    #region combatDealVars
    [Header("Combat Deal Vars")]
    [SerializeField]
    protected float damageDealNormal = 1.0f;            //how much damage to deal during a normal attack

    [SerializeField]
    protected float damageDealSpecial = 2.0f;           //how much damage to deal during a special attack (will want to move this to another script for specific abilities)

    protected int specialWindUp = 0;                    //how much a special attack has currently wound up (WILL NEED TO REDO THIS)

    [SerializeField]
    protected int specialWindUpRequiredForAttack = 1;   //minimum amount of windup for special attack (WILL NEED TO REDO THIS)

    [SerializeField]
    protected int specialWindUpCapacity = 1;            //maximum amount of windup for special attack (WILL NEED TO REDO THIS)

    public float GetDamageDealNormal()
    {
        return damageDealNormal;
    }

    public float GetDamageDealSpecial()
    {
        return damageDealSpecial;
    }

    public int GetSpecialWindUp()
    {
        return specialWindUp;
    }

    public bool GetSpecialAttackAllowed()
    {
        if (specialWindUp >= specialWindUpRequiredForAttack) return true;
        else return false;
    }

    public void SpecialWindUpReset()
    {
        specialWindUp = 0;
    }

    public void WindUpSpecial()     //allows special attack to wind up in power. Each time chosen, damage of special attack (when finally used) increases.
    {
        if (specialWindUp < specialWindUpCapacity)
            specialWindUp++;
    }
    #endregion

    #region fleeing
    [Header("Fleeing Vars")]
    
    [SerializeField]
    protected float likelihoodOfChoosingFlee = 0.3f;    //how likely it is the entity will attempt to flee assuming the option is available (>= 1 for always, <= 0 for never)

    [SerializeField]
    protected float thresholdForFleeing = 0.5f;         //how little health entity will require before the option to flee becomes available.

    [SerializeField]
    protected float successFleeingChance = 0.6f;       //how likely it is the entity will sucessfully flee and end the battle (>= 1 for always, <= 0 for never)

    public float GetLikelihoodOfChoosingFlee()
    {
        return likelihoodOfChoosingFlee;
    }

    public float GetThresholdForFleeing()
    {
        return thresholdForFleeing; 
    }

    public float GetSuccessFleeingChance()
    {
        return successFleeingChance;
    }
    #endregion

    #region messageSuffixs
    [Header("Message Suffix Vars")]                                 //add these to the end of the ParentEntity string when dictating an action to the player
    
    [SerializeField]
    protected string deathMessage = " has died!";                   //message for when this entity dies

    [SerializeField]
    protected string damageDealNormalMessage = " has attacked!";    //message for when this entity attacks normally

    [SerializeField]
    protected string damageDealSpecialMessage = " has used Special attack!";    //message for when this entity attacks using a special attack (will want to move this to another script for specific abilities)

    [SerializeField]
    protected string healMessage = " is healing!";                  //message for when this entity uses the turn to heal

    [SerializeField]
    protected string criticalHitMessage = " has recieved a Critical Hit!";    //message for when this entity receives a critical hit

    [SerializeField]
    protected string fleeFailedMessage = " has tried to run away, but failed!";    //message for when this entity attempts to flee and doesn't get lucky with their dice roll

    [SerializeField]
    protected string fleeSuccessMessage = " has run away!";         //message for when this entity attempts to flee and escapes
    #endregion

    #region animation
    public bool GetShouldPlayDamageReceivedAnim()
    {
        if (!attemptDodgeAttack || !successfullyDodgedAttack)   //if either attemptDodgeAttack or successfullyDodgedAttack is false, return true saying that the damage recived anim should play. Otherwise return false and disable anim from playing.
            return true;
        else
            return false;
    }
    #endregion

    #region healthManagment
    
    private bool ManageHealthBounds()               //checks if health is within >0 and max health, and returns accordingly. True if within bounds, false if not
    {
        
        if (health > 0 && health <= maxHealth) return true;     //check if health is between 0 (exclusive) and max health (inclusive). If passed, just return the hell outta the check.
        if (health <= 0)                            //If this runs then we gotta check if the health is negative or 0.
        {
            health = 0;                             //player is dead. Set health to 0.
            return false;                           //return of true also means nothing here.
        }
        else                                        //this only ever runs if health is greater than maxHealth
        {
            health = maxHealth;
            return true;
        }
    }

    
    public bool DecreaseHealth(float decreaseAmount)    //decrease health by passed amount. Returns true for if alive, false for dead.
    {
        if (decreaseAmount <= 0) return true;           //exit early is decreseAmount is 0 or less. Don't allow negative numbers to be passed.
            
        if (!successfullyDodgedAttack || !attemptDodgeAttack) //To be damaged you must have failed to dodge the attack (successfullyDodgedAttack) or not have tried to dodge the attack (attemptDodgeAttack)
            health -= decreaseAmount;

        attemptDodgeAttack = false;                     //Regardless of outcome, no longer let entity attempt to dodge attacks. Entity must use another turn to dodge again
        successfullyDodgedAttack = false;               //set dodging to false as a failsafe.
        return ManageHealthBounds();                    //returns true if this entity is alive, false if dead
    }

   
    

    
    public bool IncreaseHealth(float incrementAmount)                       //attempts to increase health by the passed Amount. Returns true if the consumable has been picked up and used, false if already at full health.
    {
        if (health == maxHealth || incrementAmount <= 0) return false;      //if health is at max, return false disallowing player from consuming health pickup/item. Do the same if increment amount is 0 or less
        health += incrementAmount;              //add passed health amount to health var
        ManageHealthBounds();                   //ensure health is within bounds
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

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool GetIsAtFullHealth()
    {
        if (health == maxHealth) return true;
        else return false;
    }

    public float GetHealAmount()
    {
        return healAmount;
    }


    // Saving and loading purposes
    public void SetHealth(float health)
    {
        this.health = health;
    }


    #endregion



}
