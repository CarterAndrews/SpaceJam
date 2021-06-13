using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public static GameController gameController;
    public List<Color> playerColors;
    public List<PlayerInput> players;
    private int livingPlayerCount;
    public List<Transform> playerStartingSpawns;
    public List<Transform> lobbySpawns;
    private bool isGameOver;
    public GameObject hunterWin;
    public GameObject monsterWin;
    public GameObject pauseScreen;
    public GameObject ScoreUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        if (gameController)
            Destroy(gameObject);
        else
        {
            gameController = this;
            DontDestroyOnLoad(gameObject);
            players = new List<PlayerInput>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if (SceneManager.GetActiveScene().name == "menu")
        {
            SceneNavigator.GoToScene("join");
        }
        players.Add(playerInput);
        playerInput.GetComponent<Player>().changeColor(playerColors[players.Count-1]);
        playerInput.gameObject.name = "player" + players.Count.ToString();
        
    }
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        players.Remove(playerInput);
        Destroy(playerInput.gameObject);
        
    }
    private void selectEvilBean()
    {
        PlayerInput evilBean = players[Random.Range(0, players.Count)];
        evilBean.GetComponent<Player>().makeEvilBean();
    }
    public void StartOrPauseGame()
    {
        print("starting");
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "join")
        {
            StartGame();
        }
        else if(currentScene == "menu")
        {
            LoadLobby();
        }
        else if (currentScene=="Main")
        {
            if(!isGameOver)
                PauseGame();
            else
            {
                LoadLobby();
            }
        }
    }
    public void StartGame()
    {
        gameController.GetComponent<PlayerInputManager>().DisableJoining();
        livingPlayerCount = players.Count;
        SceneNavigator.GoToScene("Main");
        selectEvilBean();
        SpawnPlayers();
    }
    public void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void RegisterPlayerDeath(Player dead)
    {
        if (dead.isEvil)
        {
            hunterWin.SetActive(true);
            isGameOver = true;
            dead.score--;
        }
        else
        {
            livingPlayerCount--;
            if (livingPlayerCount <= 1)
            {
                monsterWin.SetActive(true);
                isGameOver = true;

            }
        }

        if (isGameOver)
        {
            if (!dead.isEvil)
            {
                // Player cheers managed in Gun DoBlast
                AudioManager.Instance.TriggerSoundAttached(AudioManager.TriggerSoundType.YETI_ROAR, gameObject);
            }
        }
    }
    private void RespawnAllPlayers()
    {
        foreach(PlayerInput p in players)
        {
            p.GetComponent<Player>().reset();
            SpawnPlayers();
            
        }
    }
    private void SpawnPlayers()
    {
        List<Transform> availiableSpawns = new List<Transform>(playerStartingSpawns);
        foreach(PlayerInput p in players)
        {
            int index = Random.Range(0, availiableSpawns.Count);
            p.transform.position = availiableSpawns[index].position;
            availiableSpawns.RemoveAt(index);
        }
    }
    private void SpawnLobby()
    {
        List<Transform> availiableSpawns = new List<Transform>(lobbySpawns);
        foreach (PlayerInput p in players)
        {
            int index = Random.Range(0, availiableSpawns.Count);
            p.transform.position = availiableSpawns[index].position;
            availiableSpawns.RemoveAt(index);
        }
    }

    public static void LoadLobby()
    {
        gameController.GetComponent<PlayerInputManager>().EnableJoining();
        gameController.hunterWin.SetActive(false);
        gameController.monsterWin.SetActive(false);
        SceneNavigator.GoToScene("join");
        gameController.RespawnAllPlayers();
        gameController.SpawnLobby();
    }
}
