using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public float moveSpeed;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction AttackAction;
    private InputAction StartPauseAction;
    private Rigidbody rb;
    private UnityEvent<float> speedUpdate = new UnityEvent<float>();
    private Vector3 worldMovementDirection;
    public Color evilBeanColor;
    public bool isEvil = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        StartPauseAction= playerInput.actions["StartPause"];
        AttackAction= playerInput.actions["Attack"];
        AttackAction.performed +=
        context =>
        {
            if (context.interaction is PressInteraction && SceneManager.GetActiveScene().name == "Main")
                Attack();
        };
        rb = GetComponent<Rigidbody>();
        AudioManager.Instance.SetupRunEffect(gameObject, speedUpdate);
        DontDestroyOnLoad(gameObject);
    }
    private void FixedUpdate()
    {
        worldMovementDirection = moveAction.ReadValue<Vector2>();
        worldMovementDirection.z = worldMovementDirection.y;
        worldMovementDirection.y = 0;
        rb.velocity=worldMovementDirection * Time.fixedDeltaTime * moveSpeed;
        transform.LookAt(transform.position + worldMovementDirection);

        speedUpdate?.Invoke(Mathf.Abs(worldMovementDirection.magnitude));
    }
    // Update is called once per frame
    void Update()
    {
        if (StartPauseAction.ReadValue<float>() != 0)
        {
            GameController.gameController.StartGame();
        }
    }
    private void Attack()
    {
        print(gameObject.name + " attacks!");
    }
    public void changeColor(Color col)
    {
        GetComponent<MeshRenderer>().material.color = col;
    }
    public void makeEvilBean()
    {
        GetComponent<MeshRenderer>().material.color = evilBeanColor;
        isEvil = true;
        //GetComponent<MeshRenderer>().enabled = false;
    }

    public void Die() // This or ondestroyed, whatever you prefer
    {
        speedUpdate.RemoveAllListeners();
        AudioManager.Instance.RevokeRunEffect(gameObject);
    }
}
