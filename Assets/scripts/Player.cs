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

    public GameObject playerMesh;
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
    private Gun m_gun;
    private Transform m_gunAttach;
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
        SetupGun();
    }
    private void FixedUpdate()
    {
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();
        worldMovementDirection = moveAction.ReadValue<Vector2>();
        worldMovementDirection = worldMovementDirection.y * camForward+worldMovementDirection.x*camRight;
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
        if (isEvil)
        {
            Collider[] hits = Physics.OverlapBox(rb.position + transform.forward, Vector3.one * 0.5f, transform.rotation, playerMask);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject != gameObject)
                    hit.gameObject.GetComponent<Player>().Die();
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
        //GetComponent<MeshRenderer>().enabled = false;
        GetComponentInChildren<FootPrintMaker>().enabled = true;
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
        playerMesh.SetActive(true);

        mr.enabled = true;
        playerInput.ActivateInput();
        rb.GetComponent<Collider>().enabled = true;
        GetComponentInChildren<FootPrintMaker>().enabled = false;
    }

    private void SetupGun()
    {
        m_gunAttach = transform.Find("gun_attach");
        var gunPrefab = Resources.Load<GameObject>("gun");
        var gunObject = Instantiate(gunPrefab, m_gunAttach.position, transform.rotation);
        m_gun = gunObject.GetComponent<Gun>();
        m_gun.SetAttachPoint(m_gunAttach);
    }
}
