using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OurAudioSource.instance.ChangeTrack(Track.Menu);   
    }
}
