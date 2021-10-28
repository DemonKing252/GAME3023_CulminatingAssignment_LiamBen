using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    
    
    private Rigidbody2D rb;
    private Animator anim;

    public Canvas pauseCanvas;
    bool isPaused = false;


    public Animator cinemachineAnimator;

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(gameObject);

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        switch (PlayerPreference.load)
        {
            case LoadType.NewGame:
                GameDataManager.Instance.ClearAllPlayerPrefs();
                break;
            case LoadType.ResumeGame:
                GameDataManager.Instance.LoadGame();
                break;
        }

    }

    public void OnSaveAndQuit()
    {
        // save functionality here . . .

        GameDataManager.Instance.SaveGame();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void OnExitWithoutSaving()
    {
        // skip saving the game

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseCanvas.gameObject.SetActive(false);
        isPaused = false;
    }



    // Update is called once per frame
    void Update()
    {

        // Example
        if (Input.GetKeyDown(KeyCode.S))
        {
            DialogueManager.GetInstance().StartNewDialogue("Hello, World!", 0.05f);
        }

        // Add to Diary: Didn't realize that delta time was already used in velocity 

        var horiz = Input.GetAxisRaw("Horizontal");
        var vert = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(horiz * speed, vert * speed);

        float lateralSpeed = new Vector2(rb.velocity.x / speed, rb.velocity.y / speed).magnitude;
        anim.SetFloat("_Speed", lateralSpeed);
        if (rb.velocity.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = (isPaused ? false : true);

            if (isPaused)
            {
                Time.timeScale = 0f;
                pauseCanvas.gameObject.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                pauseCanvas.gameObject.SetActive(false);

            }


            //Debug.Log("lateral: " + lateralSpeed.ToString());

        }
    }
}
