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
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnPlayerJoined(PlayerInput playerInput)
    {
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
            PauseGame();
        }
    }
    public void StartGame()
    {
        livingPlayerCount = players.Count;
        SceneManager.LoadScene("Main");
        selectEvilBean();
        SpawnPlayers();
    }
    public void PauseGame()
    {
        if (Time.timeScale == 1)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void RegisterPlayerDeath(Player dead)
    {
        if (dead.isEvil)
        {
            GameOver();
            dead.score--;
        }
        livingPlayerCount--;
        if (livingPlayerCount <= 1)
        {
            GameOver();
        }
    }
    private void GameOver()
    {
        SceneManager.LoadScene("join");
        RespawnAllPlayers();
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
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "load")
        {
            SceneManager.LoadScene("menu");
        }
    }
    public static void LoadLobby()
    {
        SceneManager.LoadScene("join");
        gameController.SpawnPlayers();
    }
}
