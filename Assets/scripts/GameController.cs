using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameController : MonoBehaviour
{
    public static GameController gameController;
    
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
        
    }
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        Destroy(playerInput.gameObject);
    }
}
