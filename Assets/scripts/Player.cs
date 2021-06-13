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
    public static Player Villain;

    public GameObject playerMesh;
    public float moveSpeed;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction AttackAction;
    private InputAction StartPauseAction;
    private Rigidbody rb;
    private UnityEvent<float> speedUpdate = new UnityEvent<float>();
    private Vector3 worldMovementDirection;
    private Vector3 worldLookDirection;
    public Color evilBeanColor;
    public bool isEvil = false;
    public LayerMask playerMask;
    public MeshRenderer mr;
    private Gun m_gun;
    private Transform m_gunAttach;
    public int score = 0;
    public bool m_canMove = true;
    public float Velocity { get => rb.velocity.magnitude; }
    Vector3 camForward;
    Vector3 camRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        if (!playerInput)
            Initialize();
    }
    private void Initialize(bool evilAudio = false)
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["Look"];
        StartPauseAction = playerInput.actions["StartPause"];
        AttackAction = playerInput.actions["Attack"];
        AttackAction.performed +=
        context =>
        {
            if (context.interaction is PressInteraction && SceneManager.GetActiveScene().name == "Main")
                Attack();
        };
        rb = GetComponent<Rigidbody>();
        DontDestroyOnLoad(gameObject);
        SetupGun();
        camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RevokeRunEffect(gameObject); // Just in case
            if(!evilAudio)
                AudioManager.Instance.SetupRunEffect(gameObject, speedUpdate);
        }
    }
    private void FixedUpdate()
    {
        if (!m_canMove)
            return;
        worldMovementDirection = moveAction.ReadValue<Vector2>();
        worldMovementDirection.z = worldMovementDirection.y;
        worldMovementDirection.y = 0;
        //worldMovementDirection = worldMovementDirection.y * camForward+worldMovementDirection.x*camRight;
        rb.velocity=-worldMovementDirection * Time.fixedDeltaTime * moveSpeed;

        worldLookDirection = lookAction.ReadValue<Vector2>();
        worldLookDirection.z = worldLookDirection.y;
        worldLookDirection.y = 0;
        //worldLookDirection = worldLookDirection.y * camForward + worldLookDirection.x * camRight;
        transform.LookAt(transform.position - worldLookDirection);
        
        speedUpdate?.Invoke(Velocity);
    }
    // Update is called once per frame
    void Update()
    {
        if (StartPauseAction.ReadValue<float>() != 0)
        {
            GameController.gameController.StartOrPauseGame();
        }
    }
    private void SetCanMove(bool move)
    {
        m_canMove = move;
        if (!m_canMove)
            rb.velocity = Vector3.zero;
    }
    private void Attack()
    {
        //print(gameObject.name + " attacks!");
        if (isEvil)
        {
            Collider[] hits = Physics.OverlapSphere(rb.position + transform.forward, 1, playerMask);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject != gameObject)
                {
                    hit.gameObject.GetComponent<Player>().Die();
                    score++;
                }
            }
        }
        else
        {
            m_gun.TryShoot();
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
        if (!mr)
            Initialize();
        mr.enabled = false;
        playerMesh.SetActive(false);
        isEvil = true;
        Villain = this;
        m_gun.gameObject.SetActive(false);
        //GetComponent<MeshRenderer>().enabled = false;
        GetComponentInChildren<FootPrintMaker>().enabled = true;
    }

    public void Die() // This or ondestroyed, whatever you prefer
    {
        mr.enabled = false;
        playerInput.DeactivateInput();
        rb.GetComponent<Collider>().enabled = false;
        GameController.gameController.RegisterPlayerDeath(this);

        speedUpdate.RemoveAllListeners();
        if (AudioManager.Instance != null)
            AudioManager.Instance.RevokeRunEffect(gameObject);
    }
    public void reset()
    {
        isEvil = false;
        playerMesh.SetActive(true);

        mr.enabled = true;
        playerInput.ActivateInput();
        rb.GetComponent<Collider>().enabled = true;
        GetComponentInChildren<FootPrintMaker>().enabled = false;
        m_gun.gameObject.SetActive(true);
    }

    private void SetupGun()
    {
        m_gunAttach = transform.Find("gun_attach");
        var gunPrefab = Resources.Load<GameObject>("gun");
        var gunObject = Instantiate(gunPrefab, m_gunAttach.position, transform.rotation);
        GameObject.DontDestroyOnLoad(gunObject);
        m_gun = gunObject.GetComponent<Gun>();
        m_gun.SetAttachPoint(m_gunAttach);
        if (isEvil)
            m_gun.gameObject.SetActive(false);
    }
}
