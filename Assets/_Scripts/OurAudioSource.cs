using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Track
{
    Menu,
    Overworld
}

public class OurAudioSource : MonoBehaviour
{
    [SerializeField]
    AudioSource mainMenuClip;
    
    [SerializeField]
    AudioSource overworldClip;

    Track myTrack = Track.Menu;

    public static OurAudioSource instance = null;
    private int trackNum = 1;


    void Awake()
    {
        if (instance == null)
            instance = this;

        OurAudioSource[] gos = FindObjectsOfType<OurAudioSource>();
        if (gos.Length > 1)
            Destroy(gos[1]);
        else
            DontDestroyOnLoad(gameObject);    
    }

    // Start is called before the first frame update
    void Start()
    {
        mainMenuClip.Play();
    }

    public void ChangeTrack(Track newTrack)
    {

        switch (newTrack)
        {
            case Track.Menu:
                StartCoroutine(FadeBetweenTracks(mainMenuClip, overworldClip, 1.5f));
                break;
            case Track.Overworld:
                StartCoroutine(FadeBetweenTracks(overworldClip, mainMenuClip, 1.5f));
                break;
        }

        myTrack = newTrack;
    }
    public IEnumerator FadeBetweenTracks(AudioSource a, AudioSource b, float fadeTime)
    {
        float time = 0f;

        a.Play();
        
        while (time < fadeTime)
        {
            a.volume = Mathf.Lerp(0f, 0.6f, time / fadeTime);
            b.volume = Mathf.Lerp(0.6f, 0f, time / fadeTime);

            time += Time.deltaTime;
            // Wait for one frame
            yield return null;
        }
        b.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
