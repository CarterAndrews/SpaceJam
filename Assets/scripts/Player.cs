using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public float moveSpeed;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Rigidbody rb;
    private Vector3 worldMovementDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        rb = GetComponent<Rigidbody>();
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
        
    }
}
