using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int health = 3;
    [SerializeField]
    private int maxHealth = 5;



    private bool ManageHealthBounds() //checks if health is within 0 and max health, and returns accordingly. True if within bounds, false if not
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
    public bool DecreaseHealth(int decreaseAmount)
    {
        health -= decreaseAmount;
        return ManageHealthBounds();
    }

    public bool IncrementHealth(int incrementAmount)
    {
        //if health is at max, return false disallowing player from consuming health pickup/item
        if (health == maxHealth) return false;
        health += incrementAmount;
        ManageHealthBounds();
        return true;
    }

    public int GetHealth()
    {
        return health;
    }

}
