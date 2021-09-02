using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{

    public Button startButton;
    public Button quitButton;
    public Button returnToMenuButton;
    public Button returnToGameButton;
    public Button saveGame;
    public Button loadGame;

    public GameObject mainMenu;
    public GameObject pauseMenu;
    public bool gamePaused;

    public Text ammoText;
    public Text scoreText;


    public Character cRef;
    public Enemy eRef;


    // Start is called before the first frame update
    void Start()
    {
        cRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        eRef = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();

        if (startButton == true)
        {
            startButton.onClick.AddListener(() => GameManager.Instance.StartGame());
            Debug.Log("Start button pressed 1");
            Time.timeScale = 1f;
        }

        if(quitButton == true)
        {
            quitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());
        }

        if (returnToGameButton == true)
        {
            returnToGameButton.onClick.AddListener(() => ReturnToGame());
            Time.timeScale = 1f;
        }

        if (returnToMenuButton == true)
        {
            returnToMenuButton.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());
        }

        if(saveGame == true)
        {
            saveGame.onClick.AddListener(() => GameManager.Instance.SaveGame());
        }

        if(loadGame == true)
        {
            loadGame.onClick.AddListener(() => GameManager.Instance.LoadGame());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
                PauseGame();
        }

        if (ammoText)
        {
            ammoText.text = GameManager.Instance.ammo.ToString();
        }

        if (scoreText)
        {
            scoreText.text = GameManager.Instance.score.ToString();
        }
    }
    

    public void PauseGame()
    {
        //cRef.isPaused = gamePaused;
        gamePaused = !gamePaused;
        pauseMenu.SetActive(gamePaused);

        if (gamePaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ReturnToGame()
    {

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SaveGame()
    {
        cRef.SaveGamePrepare();

        //foreach (Enemy enem in eRef)
        //{
        //    if(eRef != null)
        //    {
        //        eRef.SaveGamePrepare();
        //    }
        //}
        eRef.SaveGamePrepare();

        GameManager.Instance.SaveGame();
    }

    public void LoadGame()
    {
        cRef.LoadGameComplete();
        eRef.LoadGameComplete();

        GameManager.Instance.LoadGame();
    }
}
