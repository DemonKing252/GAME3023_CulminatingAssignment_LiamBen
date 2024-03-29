using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Track
{
    Menu,
    OverworldFromMenu,
    Battle,
    OverworldFromBattle,
    BattleToMenu
}

public class OurAudioSource : MonoBehaviour
{
    [SerializeField]
    AudioSource mainMenuClip;
    
    [SerializeField]
    AudioSource overworldClip;

    [SerializeField]
    AudioSource battleClip;

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
                StartCoroutine(FadeBetweenTracks(mainMenuClip, overworldClip, 0.5f, newTrack));
                break;
            case Track.OverworldFromMenu:
                StartCoroutine(FadeBetweenTracks(overworldClip, mainMenuClip, 0.5f, newTrack));
                break;
            case Track.Battle:
                StartCoroutine(FadeBetweenTracks(battleClip, overworldClip, 0.5f, newTrack));
                break;
            case Track.OverworldFromBattle:
                StartCoroutine(FadeBetweenTracks(overworldClip, battleClip, 0.5f, newTrack));
                break;
        }

        myTrack = newTrack;
    }
    public IEnumerator FadeBetweenTracks(AudioSource a, AudioSource b, float fadeTime, Track newTrack)
    {
        float time = 0f;

        a.loop = true;
        a.Play();
        
        while (time < fadeTime)
        {
            a.volume = Mathf.Lerp(0f, 0.6f, time / fadeTime);
            b.volume = Mathf.Lerp(0.6f, 0f, time / fadeTime);

            time += Time.deltaTime;
            // Wait for one frame
            yield return null;
        }
        b.loop = false;
        b.Stop();

        if (newTrack == Track.Menu)
        {
            if (overworldClip.isPlaying)
            {
                overworldClip.loop = false;
                overworldClip.Stop();
            }
            if (battleClip.isPlaying)
            {
                battleClip.loop = false;
                battleClip.Stop();
            }
        }
    }

}
