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
    public LayerMask playerMask;
    private MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        if (!playerInput)
            Initialize();
    }
    private void Initialize()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        StartPauseAction = playerInput.actions["StartPause"];
        AttackAction = playerInput.actions["Attack"];
        AttackAction.performed +=
        context =>
        {
            if (context.interaction is PressInteraction && SceneManager.GetActiveScene().name == "Main")
                Attack();
        };
        rb = GetComponent<Rigidbody>();
        AudioManager.Instance.SetupRunEffect(gameObject, speedUpdate);
        DontDestroyOnLoad(gameObject);
        mr = GetComponent<MeshRenderer>();
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
        Collider[] hits=Physics.OverlapBox(rb.position+transform.forward, Vector3.one * 0.5f, transform.rotation,playerMask);
        foreach(Collider hit in hits)
        {
            if(hit.gameObject!=gameObject)
            hit.gameObject.GetComponent<Player>().Die();
        }
    }
    public void changeColor(Color col)
    {
        if (!mr)
            Initialize();
        mr.material.color = col;
    }
    public void makeEvilBean()
    {
        mr.material.color = evilBeanColor;
        isEvil = true;
        //GetComponent<MeshRenderer>().enabled = false;
    }

    public void Die() // This or ondestroyed, whatever you prefer
    {
        speedUpdate.RemoveAllListeners();
        AudioManager.Instance.RevokeRunEffect(gameObject);
        mr.enabled = false;
        playerInput.DeactivateInput();
        rb.GetComponent<Collider>().enabled = false;
        GameController.gameController.RegisterPlayerDeath(this);
    }
    public void reset()
    {
        isEvil = false;
        mr.enabled = true;
        playerInput.ActivateInput();
        rb.GetComponent<Collider>().enabled = true;
    }
}
