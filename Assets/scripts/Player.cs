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
    private InputAction QuitAction;
    private Rigidbody rb;
    private UnityEvent<float> speedUpdate = new UnityEvent<float>();
    private Vector3 worldMovementDirection;
    private Vector3 worldLookDirection;
    public Color evilBeanColor;
    public bool isEvil = false;
    public LayerMask playerMask;
    public MeshRenderer mr;
    private Gun m_gun;
    public Transform m_gunAttach;
    public int score = 0;
    public bool m_canMove = true;
    public Animator m_playerAnimator;
    private ParticleSystem m_snailTrail;
    private Vector2 m_animVelocity;
    [HideInInspector]
    public float LastAnalogInput;

    Vector3 camForward;
    Vector3 camRight;
    // Start is called before the first frame update
    void Start()
    {
        m_animVelocity = Vector2.zero;
    }
    private void Awake()
    {
        if (!playerInput)
            Initialize();
    }
    private void Initialize()
    {
        m_snailTrail = GetComponent<ParticleSystem>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["Look"];
        StartPauseAction = playerInput.actions["StartPause"];
        AttackAction = playerInput.actions["Attack"];
        QuitAction = playerInput.actions["Quit"];
        AttackAction.performed +=
        context =>
        {
            if (context.interaction is PressInteraction && SceneManager.GetActiveScene().name == "Main")
                Attack();
        };
        StartPauseAction.performed +=
        context =>
        {
            if (context.interaction is PressInteraction)
                GameController.gameController.StartOrPauseGame();
        };
        QuitAction.performed +=
        context =>
        {
            if (context.interaction is PressInteraction)
                GameController.quit();

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

        ApplyRunEffect(true);
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Vector3 playerSpaceVelocity =  transform.InverseTransformVector(rb.velocity);
        m_animVelocity = Vector2.Lerp(m_animVelocity, new Vector2(playerSpaceVelocity.x, playerSpaceVelocity.z), Time.fixedDeltaTime * 10.0f);
        m_playerAnimator.SetFloat("zVel", m_animVelocity.y / moveSpeed);
        m_playerAnimator.SetFloat("xVel", m_animVelocity.x / moveSpeed);


        Vector2 analogInput = moveAction.ReadValue<Vector2>();
        LastAnalogInput = analogInput.magnitude;
        speedUpdate?.Invoke(LastAnalogInput);

        if (!m_canMove)
            return;

        worldMovementDirection = analogInput;
        worldMovementDirection.z = worldMovementDirection.y;
        worldMovementDirection.y = 0;
        //worldMovementDirection = worldMovementDirection.y * camForward+worldMovementDirection.x*camRight;
        rb.velocity=-worldMovementDirection * moveSpeed;

        worldLookDirection = lookAction.ReadValue<Vector2>();
        worldLookDirection.z = worldLookDirection.y;
        worldLookDirection.y = 0;
        //worldLookDirection = worldLookDirection.y * camForward + worldLookDirection.x * camRight;
        transform.LookAt(transform.position - worldLookDirection);
        
    }
    private void SetCanMove(bool move)
    {
        m_canMove = move;
        if (!m_canMove)
            rb.velocity = Vector3.zero;
    }
    public GameObject slashPs;
    float timeSinceLastAttack;
    private void Attack()
    {
        //print(gameObject.name + " attacks!");
        if (isEvil)
        {
            if (timeSinceLastAttack < 0.8f)
                return;

            timeSinceLastAttack = 0;

            Vector3 attackPos = rb.position;// + transform.forward;
            Transform ps = Instantiate(slashPs, attackPos, Quaternion.identity).transform;
            ps.transform.forward = transform.forward;
            Destroy(ps.gameObject, 1);
            Collider[] hits = Physics.OverlapSphere(attackPos, 3, playerMask);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject != gameObject)
                {
                    hit.gameObject.GetComponent<Player>().Die();
                    score++;
                }
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.TriggerSoundAttached(AudioManager.TriggerSoundType.YETI_SWIPE, gameObject);
                AudioManager.Instance.ResetInputTimer();
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
        //mr.material.color = col;
        mr.material.SetColor("ColorShift", col);
    }
    public void makeEvilBean()
    {
        if (!mr)
            Initialize();
        mr.enabled = false;
        playerMesh.SetActive(false);
        isEvil = true;
        m_gun.gameObject.SetActive(false);
        //GetComponent<MeshRenderer>().enabled = false;
        m_snailTrail.Stop();
        GetComponentInChildren<FootPrintMaker>().enabled = true;

        Villain = this;
        ApplyRunEffect(false);
    }

    public void Die() // This or ondestroyed, whatever you prefer
    {
        mr.enabled = false;
        playerInput.DeactivateInput();
        rb.GetComponent<Collider>().enabled = false;
        GameController.gameController.RegisterPlayerDeath(this);

        speedUpdate.RemoveAllListeners();

        ApplyRunEffect(false);
        if (AudioManager.Instance != null)
            AudioManager.Instance.TriggerSoundAttached(AudioManager.TriggerSoundType.DEATH, gameObject);
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
        m_snailTrail.Play();

        ApplyRunEffect(true);
    }

    private void SetupGun()
    {
        //m_gunAttach = transform.Find("gun_attach");
        var gunPrefab = Resources.Load<GameObject>("gun");
        var gunObject = Instantiate(gunPrefab, m_gunAttach.position, transform.rotation);
        GameObject.DontDestroyOnLoad(gunObject);
        m_gun = gunObject.GetComponent<Gun>();
        m_gun.SetAttachPoint(m_gunAttach);
        if (isEvil)
            m_gun.gameObject.SetActive(false);
    }


    private void ApplyRunEffect(bool apply)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RevokeRunEffect(gameObject);

            if(apply)
                AudioManager.Instance.SetupRunEffect(gameObject, speedUpdate);
        }
    }
}
