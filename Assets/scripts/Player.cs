using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    private PlayerInputActions playerInput;
    private Rigidbody rb;
    private Vector3 worldMovementDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        playerInput = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }
    private void FixedUpdate()
    {
        worldMovementDirection= playerInput.Movement.Move.ReadValue<Vector2>();
        worldMovementDirection.z = worldMovementDirection.y;
        worldMovementDirection.y = 0;
        rb.velocity=worldMovementDirection * Time.fixedDeltaTime * moveSpeed;
        transform.LookAt(transform.position + worldMovementDirection);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
