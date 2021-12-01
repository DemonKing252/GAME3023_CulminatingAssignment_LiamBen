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

    public Canvas winCanvas;
    public Canvas loseCanvas;

    public Button loseMainMenuBtn;
    public Button loseQuitGameBtn;

    public Button winMainMenuBtn;
    public Button winQuitGameBtn;



    public void OnLoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    public void WinGame()
    {
        // Can't resume game if you win otherwise theres no enemies to kill
        PlayerPrefs.DeleteAll();
        Time.timeScale = 0f;
        winCanvas.gameObject.SetActive(true);
    }
    public void LoseGame()
    {
        // Can't resume game if you lose, your health is already zero!
        PlayerPrefs.DeleteAll();
        Time.timeScale = 0f;
        loseCanvas.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(gameObject);


        Time.timeScale = 1f;

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

        loseMainMenuBtn.onClick.AddListener(OnLoadMainMenu);
        winMainMenuBtn.onClick.AddListener(OnLoadMainMenu);

        loseQuitGameBtn.onClick.AddListener(OnQuitGame);
        winQuitGameBtn.onClick.AddListener(OnQuitGame);
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
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    DialogueManager.GetInstance().StartNewDialogue("Hello, World!", 0.05f);
        //}

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
