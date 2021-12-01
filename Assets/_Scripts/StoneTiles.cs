using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTiles : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D pRigidbody;


    [SerializeField]
    AudioSource footsteps;

    [SerializeField]
    AudioClip grassTrack;

    [SerializeField]
    AudioClip stoneTrack;


    void Start()
    {
        footsteps.loop = true;
        footsteps.Play();    
    }
    void Update()
    {
        //Debug.Log("vel1" + pRigidbody.velocity.magnitude.ToString());
        
        if (pRigidbody.velocity.magnitude > 0.1f && !footsteps.isPlaying)
        {
            footsteps.loop = true;
            footsteps.Play();
        }
        else if (pRigidbody.velocity.magnitude < 0.1f && footsteps.isPlaying)
        {
            footsteps.loop = false;
            footsteps.Stop();
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            footsteps.clip = stoneTrack;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            footsteps.clip = grassTrack;
        }
    }
}
