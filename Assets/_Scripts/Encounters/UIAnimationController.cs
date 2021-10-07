using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpriteLoadBy
{
    SpriteSheet,
    SeperateSprites

}
public class UIAnimationController : MonoBehaviour
{
    public Sprite[] sprites;

    public float frameDuration = 0.3f;
    private int _spriteFrame = 0;
    private Image img;
    public string path;

    // Sprite sheet / Series of sprites
    public SpriteLoadBy method;
    public string[] paths;

    // Start is called before the first frame update
    void Start()
    {
        timestart = Time.time;
        img = GetComponent<Image>();
        
        if (method == SpriteLoadBy.SpriteSheet)
            sprites = Resources.LoadAll<Sprite>(path);
        else
        {
            sprites = new Sprite[paths.Length];
            
            for(int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = Resources.Load<Sprite>(paths[i]);
            }
        }
    
    }


    private float timestart = 0f;
    // Update is called once per frame
    void Update()
    {
        if (Time.time - timestart >= frameDuration)
        {
            timestart = Time.time;

            _spriteFrame = (_spriteFrame + 1) % sprites.Length;
            img.sprite = sprites[_spriteFrame];
        }
        


        
    }
}
