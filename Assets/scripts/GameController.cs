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
    public void StartGame()
    {
        print("starting");
        if (SceneManager.GetActiveScene().name == "join")
        {
            livingPlayerCount = players.Count;
            SceneManager.LoadScene("Main");
            selectEvilBean();
        }
    }
    public void RegisterPlayerDeath(Player dead)
    {
        if (dead.isEvil)
        {
            GameOver();
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
        }
    }
}
