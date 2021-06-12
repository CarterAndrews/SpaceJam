using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public float moveSpeed;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction StartPauseAction;
    private Rigidbody rb;
    private Vector3 worldMovementDirection;
    public Color evilBeanColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        StartPauseAction= playerInput.actions["StartPause"];
        rb = GetComponent<Rigidbody>();
        DontDestroyOnLoad(gameObject);
    }
    private void FixedUpdate()
    {
        worldMovementDirection = moveAction.ReadValue<Vector2>();
        worldMovementDirection.z = worldMovementDirection.y;
        worldMovementDirection.y = 0;
        rb.velocity=worldMovementDirection * Time.fixedDeltaTime * moveSpeed;
        transform.LookAt(transform.position + worldMovementDirection);
        
    }
    // Update is called once per frame
    void Update()
    {
        if (StartPauseAction.ReadValue<float>() != 0)
        {
            GameController.gameController.StartGame();
        }
    }
    public void changeColor(Color col)
    {
        GetComponent<MeshRenderer>().material.color = col;
    }
    public void makeEvilBean()
    {
        GetComponent<MeshRenderer>().material.color = evilBeanColor;
        //GetComponent<MeshRenderer>().enabled = false;
    }
}
