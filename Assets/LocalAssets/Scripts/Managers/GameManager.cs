using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            if (!instance)
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            return instance;
        }
    }


    public static LoadSaveManager StateManager
    {
        get
        {
            if(!stateManager)   
            {
                stateManager = instance.GetComponent<LoadSaveManager>();
            }
            return stateManager;
        }
    }
    private static GameManager instance = null;

    static LoadSaveManager stateManager = null; 

    private static bool bShouldLoad = false;

    public Vector3 lastCheckpointPosition;

    int _ammo = 10;
    int _score = 0;
    int _lives = 3;
    public int maxLives = 3;

    public int ammo
    {
        get { return _ammo; }
        set
        {
            _ammo = value;
            Debug.Log("current ammo is " + _ammo);
        }
    }

    public int score
    {
        get { return _score; }
        set
        {
            _score = value;
            Debug.Log("current score is " + _score);
        }
    }


    public int lives
    {
        get { return _lives; }
        set 
        {
            if(_lives > value)
            {
                //respawn code
                SceneManager.LoadScene("LevelScene");
            }
            _lives = value;
            if (_lives > maxLives)
            {
                _lives = maxLives;
            }
            else if(_lives <=0)
            {
                _lives = 0;
                SceneManager.LoadScene("GameOver");
            }
            Debug.Log("Current lives are " + _lives);
        }
    }

    private void Awake()
    {
        if((instance) && (instance.GetInstanceID() != GetInstanceID()))
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //GetSingleton();

        if(bShouldLoad)
        {
            StateManager.Load(Application.persistentDataPath + "/SaveGame.xml");

            bShouldLoad = false;
        }

        //PlayerPrefs.SetString("Name", "Jim");
        //PlayerPrefs.SetFloat("Volume", 0.5f);
        //PlayerPrefs.Save();
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(SceneManager.GetActiveScene().name == "LevelScene")
            {
                SceneManager.LoadScene("MainMenu");
            }
            else if(SceneManager.GetActiveScene().name == "MainMenu")
            {
                SceneManager.LoadScene("LevelScene");
            }
            else if(SceneManager.GetActiveScene().name == "GameOver")
            {
                SceneManager.LoadScene("MainMenu");
            }
            else if(SceneManager.GetActiveScene().name == "LevelComplete")
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }


    public void StartGame()
    {
        Debug.Log("Start button pressed 2");
        SceneManager.LoadScene("LevelScene");
    }

    public void QuitGame()
    {
//#if UNITY_EDITOR
//        EditorApplication.isPlaying = false;
//#else
//        Application.Quit()
//#endif

        
    }


    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void SaveGame()
    {

        Debug.Log(Application.persistentDataPath);

        StateManager.Save(Application.persistentDataPath + "/SaveGame.xml");

    }


    public void LoadGame()
    {
        bShouldLoad = true;

        StateManager.Load(Application.persistentDataPath + "/SaveGame.xml");

        //Restart game
    }


    /*
    public void GetSingleton()
    {
        if (instance == true)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }*/
}
